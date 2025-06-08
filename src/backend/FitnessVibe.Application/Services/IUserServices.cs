using FitnessVibe.Domain.Entities.Users;

namespace FitnessVibe.Application.Services
{
    /// <summary>
    /// Service for secure password operations.
    /// Think of this as the security guard for our user accounts -
    /// it ensures passwords are stored and verified safely.
    /// </summary>
    public interface IPasswordHashingService
    {
        /// <summary>
        /// Hashes a plain text password securely.
        /// Like turning a key into a complex lock pattern that can't be reverse-engineered.
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a plain text password against a stored hash.
        /// Like checking if the key matches the lock pattern.
        /// </summary>
        bool VerifyPassword(string password, string hash);
    }

    /// <summary>
    /// Service for JWT token operations.
    /// Think of tokens as temporary VIP passes that prove who you are
    /// without having to show your full credentials every time.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates an access token for the user.
        /// Like issuing a temporary ID badge for building access.
        /// </summary>
        string GenerateAccessToken(User user);

        /// <summary>
        /// Generates a refresh token for the user.
        /// Like a backup key that can be used to get a new temporary pass.
        /// </summary>
        string GenerateRefreshToken(User user);

        /// <summary>
        /// Validates and extracts user information from a token.
        /// Like scanning an ID badge to verify its authenticity.
        /// </summary>
        bool ValidateToken(string token, out int userId);

        /// <summary>
        /// Refreshes an access token using a valid refresh token.
        /// Like using your backup key to get a new temporary pass.
        /// </summary>
        string RefreshAccessToken(string refreshToken);
    }

    /// <summary>
    /// Service for sending emails to users.
    /// Think of this as the automated communication system that keeps
    /// users informed and engaged with their fitness journey.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends a welcome email to new users.
        /// Like a friendly greeting card with getting started tips.
        /// </summary>
        Task SendWelcomeEmailAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an email verification message.
        /// Like sending a confirmation code to verify contact information.
        /// </summary>
        Task SendEmailVerificationAsync(User user, string verificationToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a password reset email.
        /// Like sending spare keys when someone is locked out.
        /// </summary>
        Task SendPasswordResetEmailAsync(User user, string resetToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends motivational messages and updates.
        /// Like having a personal trainer send encouragement notes.
        /// </summary>
        Task SendMotivationalEmailAsync(User user, string subject, string content, CancellationToken cancellationToken = default);
    }
}
