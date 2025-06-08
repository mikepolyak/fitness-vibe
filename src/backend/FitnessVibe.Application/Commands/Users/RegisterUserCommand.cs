using MediatR;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Application.Commands.Users
{
    /// <summary>
    /// Command to register a new user in our fitness community.
    /// Like filling out a gym membership application with all your fitness goals and preferences.
    /// </summary>
    public class RegisterUserCommand : IRequest<RegisterUserResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public FitnessLevel FitnessLevel { get; set; } = FitnessLevel.Beginner;
        public FitnessGoal PrimaryGoal { get; set; } = FitnessGoal.StayActive;
        public List<string> FavoriteActivities { get; set; } = new();
        public bool AcceptTerms { get; set; }
        public bool OptInToMarketing { get; set; }
        public string? ReferralCode { get; set; }
    }

    /// <summary>
    /// Response after successfully joining our fitness community.
    /// Like receiving your new gym membership card with all your details.
    /// </summary>
    public class RegisterUserResponse
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
        public List<string> WelcomeBadges { get; set; } = new();
        public string WelcomeMessage { get; set; } = string.Empty;
        public bool NeedsOnboarding { get; set; } = true;
    }
}