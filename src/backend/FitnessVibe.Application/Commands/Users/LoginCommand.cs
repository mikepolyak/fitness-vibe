using MediatR;

namespace FitnessVibe.Application.Commands.Users
{
    /// <summary>
    /// Command to authenticate a user and start their fitness session.
    /// Like checking in at the gym with your membership card - proving who you are
    /// so you can access all the equipment and track your progress.
    /// </summary>
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
        public string? DeviceInfo { get; set; }
        public string? IpAddress { get; set; }
    }

    /// <summary>
    /// Response after successful authentication - like getting your day pass and locker assignment.
    /// Contains everything you need to navigate the fitness app and track your journey.
    /// </summary>
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public bool IsEmailVerified { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string WelcomeBackMessage { get; set; } = string.Empty;
        public int DaysSinceLastLogin { get; set; }
        public bool HasUnreadNotifications { get; set; }
        public int ActiveChallenges { get; set; }
        public string CurrentStreak { get; set; } = string.Empty;
    }
}
