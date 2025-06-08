using MediatR;

namespace FitnessVibe.Application.Commands.Users
{
    /// <summary>
    /// Command to refresh authentication tokens.
    /// Like renewing your gym access pass before it expires.
    /// </summary>
    public class RefreshTokenCommand : IRequest<TokenRefreshResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string? DeviceInfo { get; set; }
    }

    /// <summary>
    /// Response with new authentication tokens.
    /// Like receiving your renewed gym access pass.
    /// </summary>
    public class TokenRefreshResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime RefreshedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Command to logout a user from the fitness community.
    /// Like checking out at the gym front desk.
    /// </summary>
    public class LogoutCommand : IRequest
    {
        public int UserId { get; set; }
        public string? RefreshToken { get; set; }
        public bool LogoutAllDevices { get; set; } = false;
    }

    /// <summary>
    /// Command to request a password reset.
    /// Like asking the gym staff to help you reset your locker combination.
    /// </summary>
    public class ForgotPasswordCommand : IRequest
    {
        public string Email { get; set; } = string.Empty;
        public string? ClientUrl { get; set; } // For password reset redirect
    }

    /// <summary>
    /// Command to reset password using a reset token.
    /// Like using the temporary code to set a new locker combination.
    /// </summary>
    public class ResetPasswordCommand : IRequest
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// Command to verify email address.
    /// Like confirming your contact information with the gym.
    /// </summary>
    public class VerifyEmailCommand : IRequest
    {
        public string Token { get; set; } = string.Empty;
        public int? UserId { get; set; } // Optional, can be extracted from token
    }

    /// <summary>
    /// Command to change user password.
    /// Like updating your locker combination for better security.
    /// </summary>
    public class ChangePasswordCommand : IRequest
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
