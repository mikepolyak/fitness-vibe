using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Commands.Users;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Users
{
    /// <summary>
    /// Handler for password reset requests - the helpful gym staff member who assists with lost access codes.
    /// This is like having a customer service representative who securely helps you regain access
    /// to your fitness account when you've forgotten your credentials.
    /// </summary>
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            ITokenService tokenService,
            IEmailService emailService,
            IPasswordResetRepository passwordResetRepository,
            ILogger<ForgotPasswordCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _passwordResetRepository = passwordResetRepository ?? throw new ArgumentNullException(nameof(passwordResetRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing password reset request for email: {Email}", request.Email);

            try
            {
                // Find user by email
                var user = await _userRepository.GetByEmailAsync(request.Email);
                
                // Always log the attempt, regardless of whether user exists (for security monitoring)
                _logger.LogInformation("Password reset requested for email: {Email}, User exists: {UserExists}", 
                    request.Email, user != null);

                // If user doesn't exist, still proceed silently to prevent email enumeration attacks
                if (user == null)
                {
                    _logger.LogDebug("Password reset requested for non-existent email: {Email}", request.Email);
                    
                    // Add a small delay to prevent timing attacks
                    await Task.Delay(Random.Shared.Next(100, 500), cancellationToken);
                    return;
                }

                // Check if account is active and not locked
                if (!user.IsActive || user.IsLocked)
                {
                    _logger.LogWarning("Password reset requested for inactive/locked account: {UserId}", user.Id);
                    
                    // Send a different email for locked accounts
                    if (user.IsLocked)
                    {
                        await _emailService.SendAccountLockedNotificationAsync(user.Email, user.FirstName);
                    }
                    
                    return;
                }

                // Check for recent reset requests to prevent spam
                var recentResetAttempts = await _passwordResetRepository.GetRecentResetAttemptsAsync(user.Id, TimeSpan.FromMinutes(15));
                if (recentResetAttempts >= 3)
                {
                    _logger.LogWarning("Too many password reset attempts for user: {UserId}", user.Id);
                    await _emailService.SendTooManyResetAttemptsAsync(user.Email, user.FirstName);
                    return;
                }

                // Generate secure reset token
                var resetToken = await _tokenService.GeneratePasswordResetTokenAsync(user.Id);
                
                // Create password reset record
                var passwordReset = new PasswordReset(
                    userId: user.Id,
                    token: resetToken.Token,
                    expiresAt: resetToken.ExpiresAt,
                    requestedFromIp: request.IpAddress
                );

                await _passwordResetRepository.AddAsync(passwordReset);
                await _passwordResetRepository.SaveChangesAsync();

                // Create reset URL
                var resetUrl = !string.IsNullOrEmpty(request.ClientUrl) 
                    ? $"{request.ClientUrl}/reset-password?token={resetToken.Token}"
                    : $"/reset-password?token={resetToken.Token}";

                // Send password reset email
                await _emailService.SendPasswordResetEmailAsync(
                    email: user.Email,
                    firstName: user.FirstName,
                    resetUrl: resetUrl,
                    expiresInMinutes: (int)resetToken.ExpiresAt.Subtract(DateTime.UtcNow).TotalMinutes
                );

                _logger.LogInformation("Password reset email sent successfully for user: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process password reset request for email: {Email}", request.Email);
                // Don't rethrow - we want to appear successful to prevent information leakage
            }
        }
    }

    /// <summary>
    /// Represents a password reset request in the system.
    /// </summary>
    public class PasswordReset
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? RequestedFromIp { get; private set; }
        public bool IsUsed { get; private set; }
        public DateTime? UsedAt { get; private set; }

        public PasswordReset(int userId, string token, DateTime expiresAt, string? requestedFromIp = null)
        {
            UserId = userId;
            Token = token ?? throw new ArgumentNullException(nameof(token));
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
            RequestedFromIp = requestedFromIp;
            IsUsed = false;
        }

        public void MarkAsUsed()
        {
            IsUsed = true;
            UsedAt = DateTime.UtcNow;
        }
    }
}
