using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Commands.Users;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Users
{
    /// <summary>
    /// Handler for email verification - the membership confirmation specialist.
    /// This is like having a gym staff member verify your contact information
    /// to ensure you're reachable for important fitness community updates and notifications.
    /// </summary>
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly IGamificationService _gamificationService;
        private readonly IEmailService _emailService;
        private readonly ILogger<VerifyEmailCommandHandler> _logger;

        public VerifyEmailCommandHandler(
            IUserRepository userRepository,
            IEmailVerificationRepository emailVerificationRepository,
            IGamificationService gamificationService,
            IEmailService emailService,
            ILogger<VerifyEmailCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _emailVerificationRepository = emailVerificationRepository ?? throw new ArgumentNullException(nameof(emailVerificationRepository));
            _gamificationService = gamificationService ?? throw new ArgumentNullException(nameof(gamificationService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing email verification with token");

            // Find the email verification record
            var emailVerification = await _emailVerificationRepository.GetByTokenAsync(request.Token);
            if (emailVerification == null)
            {
                _logger.LogWarning("Email verification failed - invalid token");
                throw new InvalidOperationException("Invalid or expired verification token");
            }

            // Check if token has expired
            if (emailVerification.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Email verification failed - expired token for user: {UserId}", emailVerification.UserId);
                throw new InvalidOperationException("Verification token has expired. Please request a new verification email.");
            }

            // Check if token has already been used
            if (emailVerification.IsUsed)
            {
                _logger.LogWarning("Email verification failed - token already used for user: {UserId}", emailVerification.UserId);
                throw new InvalidOperationException("Verification token has already been used. Your email is already verified.");
            }

            // Get the user
            var user = await _userRepository.GetByIdAsync(emailVerification.UserId);
            if (user == null)
            {
                _logger.LogError("Email verification failed - user not found: {UserId}", emailVerification.UserId);
                throw new InvalidOperationException("User account not found");
            }

            // Check if email is already verified
            if (user.IsEmailVerified)
            {
                _logger.LogInformation("Email verification attempted for already verified user: {UserId}", user.Id);
                
                // Mark token as used and return success
                emailVerification.MarkAsUsed();
                await _emailVerificationRepository.UpdateAsync(emailVerification);
                await _emailVerificationRepository.SaveChangesAsync();
                
                return;
            }

            try
            {
                // Verify the email
                user.VerifyEmail();
                await _userRepository.UpdateAsync(user);

                // Mark verification token as used
                emailVerification.MarkAsUsed();
                await _emailVerificationRepository.UpdateAsync(emailVerification);

                // Award gamification rewards for email verification
                await AwardEmailVerificationRewards(user.Id);

                // Save all changes
                await _userRepository.SaveChangesAsync();

                _logger.LogInformation("Email verified successfully for user: {UserId}", user.Id);

                // Send welcome email with app features
                try
                {
                    await _emailService.SendEmailVerificationSuccessAsync(user.Email, user.FirstName);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send email verification success email to user: {UserId}", user.Id);
                    // Don't fail the verification if welcome email fails
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify email for user: {UserId}", emailVerification.UserId);
                throw new InvalidOperationException("Failed to verify email. Please try again.");
            }
        }

        /// <summary>
        /// Awards gamification rewards for completing email verification.
        /// Like getting a welcome bonus for confirming your gym membership details!
        /// </summary>
        private async Task AwardEmailVerificationRewards(int userId)
        {
            try
            {
                // Award XP for email verification
                await _gamificationService.AwardXpAsync(userId, 50, "Email verification completed");

                // Award email verification badge
                await _gamificationService.AwardBadgeAsync(userId, "EmailVerified", "Verified email address");

                _logger.LogDebug("Awarded email verification rewards to user: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to award email verification rewards to user: {UserId}", userId);
                // Don't fail email verification if gamification fails
            }
        }
    }

    /// <summary>
    /// Represents an email verification request in the system.
    /// </summary>
    public class EmailVerification
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string Email { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsUsed { get; private set; }
        public DateTime? UsedAt { get; private set; }

        public EmailVerification(int userId, string email, string token, DateTime expiresAt)
        {
            UserId = userId;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Token = token ?? throw new ArgumentNullException(nameof(token));
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
            IsUsed = false;
        }

        public void MarkAsUsed()
        {
            IsUsed = true;
            UsedAt = DateTime.UtcNow;
        }
    }
}
