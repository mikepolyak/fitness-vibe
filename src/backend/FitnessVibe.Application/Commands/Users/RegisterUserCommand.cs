using MediatR;

namespace FitnessVibe.Application.Commands.Users
{
    /// <summary>
    /// Command to register a new user - the first step in their fitness journey.
    /// Think of this as signing up for a gym membership, but digital and more engaging!
    /// </summary>
    public class RegisterUserCommand : IRequest<RegisterUserResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FitnessLevel { get; set; } = "Beginner";
        public string PrimaryGoal { get; set; } = "StayActive";
    }

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
    }
}
