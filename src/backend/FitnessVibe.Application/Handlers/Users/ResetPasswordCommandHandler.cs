using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Commands.Users;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Users
{
    /// <summary>
    /// Handler for password reset completion - the locksmith who helps you set a new combination.
    /// This is like having a security expert verify your reset code and help you create
    /// a new secure access password for your fitness account.
    /// </summary>
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordResetRepository passwordResetRepository,
            IPasswordHashingService passwordHashingService,
            ITokenService tokenService,
            IEmailService emailService,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordResetRepository = passwordResetRepository ?? throw new ArgumentNullException(nameof(passwordResetRepository));
            _passwordHashingService = passwordHashingService ?? throw new ArgumentNullException(nameof(passwordHashingService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing password reset with token");

            // Validate that passwords match
            if (request.NewPassword != request.ConfirmPassword)
            {
                _logger.LogWarning("Password reset failed - passwords don't match");
                throw new InvalidOperationException("New password and confirmation password do not match");
            }

            // Find the password reset record
            var passwordReset = await _passwordResetRepository.GetByTokenAsync(request.Token);
            if (passwordReset == null)
            {
                _logger.LogWarning("Password reset failed - invalid token");
                throw new InvalidOperationException("Invalid or expired reset token");
            }

            // Check if token has expired
            if (passwordReset.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Password reset failed - expired token for user: {UserId}", passwordReset.UserId);
                throw new InvalidOperationException("Reset token has expired. Please request a new password reset.");
            }

            // Check if token has already been used
            if (passwordReset.IsUsed)
            {
                _logger.LogWarning("Password reset failed - token already used for user: {UserId}", passwordReset.UserId);
                throw new InvalidOperationException("Reset token has already been used. Please request a new password reset.");
            }

            // Get the user
            var user = await _userRepository.GetByIdAsync(passwordReset.UserId);
            if (user == null)
            {
                _logger.LogError("Password reset failed - user not found: {UserId}", passwordReset.UserId);
                throw new InvalidOperationException("User account not found");
            }

            // Check if account is still active
            if (!user.IsActive)
            {
                _logger.LogWarning("Password reset failed - user account inactive: {UserId}", user.Id);
                throw new InvalidOperationException("User account is not active");
            }

            try
            {
                // Validate new password strength
                await ValidatePasswordStrength(request.NewPassword);

                // Hash the new password
                var newPasswordHash = await _passwordHashingService.HashPasswordAsync(request.NewPassword);

                // Update user password
                user.UpdatePassword(newPasswordHash);
                
                // Reset failed login attempts
                user.ResetFailedLoginAttempts();
                
                // Unlock account if it was locked
                if (user.IsLocked)
                {
                    user.Unlock();
                    _logger.LogInformation("Account unlocked during password reset for user: {UserId}", user.Id);
                }

                await _userRepository.UpdateAsync(user);

                // Mark reset token as used
                passwordReset.MarkAsUsed();
                await _passwordResetRepository.UpdateAsync(passwordReset);

                // Revoke all existing refresh tokens for security
                await _tokenService.RevokeAllUserRefreshTokensAsync(user.Id);

                // Save all changes
                await _userRepository.SaveChangesAsync();

                _logger.LogInformation("Password reset completed successfully for user: {UserId}", user.Id);

                // Send confirmation email
                try
                {
                    await _emailService.SendPasswordResetConfirmationAsync(user.Email, user.FirstName);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send password reset confirmation email to user: {UserId}", user.Id);
                    // Don't fail the password reset if email fails
                }
            }
            catch (InvalidOperationException)
            {
                // Re-throw validation exceptions
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset password for user: {UserId}", passwordReset.UserId);
                throw new InvalidOperationException("Failed to reset password. Please try again.");
            }
        }

        /// <summary>
        /// Validates password strength requirements.
        /// Like having a security expert ensure your new password is strong enough.
        /// </summary>
        private async Task ValidatePasswordStrength(string password)
        {
            var validationResult = await _passwordHashingService.ValidatePasswordStrengthAsync(password);
            
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors);
                _logger.LogWarning("Password reset failed - weak password: {Errors}", errors);
                throw new InvalidOperationException($"Password does not meet security requirements: {errors}");
            }
        }
    }
}
