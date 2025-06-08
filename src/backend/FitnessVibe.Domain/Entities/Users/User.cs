using System;
using System.Collections.Generic;
using System.Linq;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.ValueObjects;
using FitnessVibe.Domain.Entities.Gamification;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.Events;
using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Domain.Entities.Users
{
    /// <summary>
    /// User entity - the protagonist of our fitness story.
    /// Think of each user as a hero on their fitness journey, with:
    /// - A unique identity and profile
    /// - Progression levels (like RPG characters)
    /// - Goals they're working toward
    /// - A social circle for support
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// The user's email address, used for authentication and communication
        /// </summary>
        public string Email { get; private set; } = string.Empty;

        /// <summary>
        /// The user's first name
        /// </summary>
        public string FirstName { get; private set; } = string.Empty;

        /// <summary>
        /// The user's last name
        /// </summary>
        public string LastName { get; private set; } = string.Empty;

        /// <summary>
        /// The user's date of birth, used for age-appropriate content and metrics
        /// </summary>
        public DateTime? DateOfBirth { get; private set; }

        /// <summary>
        /// The user's gender, used for personalization and metrics calculations
        /// </summary>
        public Gender Gender { get; private set; }

        /// <summary>
        /// URL to the user's profile avatar image
        /// </summary>
        public string? AvatarUrl { get; private set; }
        
        /// <summary>
        /// The user's current fitness level
        /// </summary>
        public FitnessLevel FitnessLevel { get; private set; }

        /// <summary>
        /// The user's primary fitness goal
        /// </summary>
        public FitnessGoal PrimaryGoal { get; private set; }

        /// <summary>
        /// The user's preferences for the app experience
        /// </summary>
        public UserPreferences Preferences { get; private set; } = UserPreferences.Default();
        
        /// <summary>
        /// The user's total experience points earned through activities
        /// </summary>
        public int ExperiencePoints { get; private set; }

        /// <summary>
        /// The user's current level, calculated from experience points
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// The last time the user was active in the app
        /// </summary>
        public DateTime LastActiveDate { get; private set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Hash of the user's password
        /// </summary>
        public string PasswordHash { get; private set; } = string.Empty;

        /// <summary>
        /// Whether the user's email has been verified
        /// </summary>
        public bool IsEmailVerified { get; private set; }

        /// <summary>
        /// Whether the user's account is active
        /// </summary>
        public bool IsActive { get; private set; }
        
        /// <summary>
        /// The user's fitness goals
        /// </summary>
        public ICollection<UserGoal> Goals { get; private set; } = new List<UserGoal>();

        /// <summary>
        /// The badges the user has earned
        /// </summary>
        public ICollection<UserBadge> Badges { get; private set; } = new List<UserBadge>();

        /// <summary>
        /// The user's activity history
        /// </summary>
        public ICollection<UserActivity> Activities { get; private set; } = new List<UserActivity>();

        // Private constructor for EF Core
        private User() { }

        /// <summary>
        /// Creates a new user with the specified basic profile information
        /// </summary>
        /// <param name="email">The user's email address</param>
        /// <param name="firstName">The user's first name</param>
        /// <param name="lastName">The user's last name</param>
        /// <param name="passwordHash">The hash of the user's password</param>
        /// <param name="fitnessLevel">The user's initial fitness level</param>
        /// <param name="primaryGoal">The user's initial fitness goal</param>
        public User(
            string email, 
            string firstName, 
            string lastName, 
            string passwordHash,
            FitnessLevel fitnessLevel = FitnessLevel.Beginner,
            FitnessGoal primaryGoal = FitnessGoal.StayActive)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            FitnessLevel = fitnessLevel;
            PrimaryGoal = primaryGoal;
            ExperiencePoints = 0;
            Level = 1;
            IsActive = true;
            IsEmailVerified = false;
            LastActiveDate = DateTime.UtcNow;
            Preferences = UserPreferences.Default();

            // Welcome the new user to their fitness journey!
            AddDomainEvent(new UserRegisteredEvent(this));
        }

        /// <summary>
        /// Updates the user's basic profile information
        /// </summary>
        public void UpdateProfile(string firstName, string lastName, DateTime? dateOfBirth = null, Gender? gender = null)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            
            if (dateOfBirth.HasValue)
                DateOfBirth = dateOfBirth.Value;
                
            if (gender.HasValue)
                Gender = gender.Value;

            MarkAsUpdated();
        }

        /// <summary>
        /// Sets the user's profile avatar URL
        /// </summary>
        public void SetAvatar(string avatarUrl)
        {
            AvatarUrl = avatarUrl;
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates the user's fitness profile settings
        /// </summary>
        public void UpdateFitnessProfile(FitnessLevel fitnessLevel, FitnessGoal primaryGoal)
        {
            FitnessLevel = fitnessLevel;
            PrimaryGoal = primaryGoal;
            MarkAsUpdated();
        }

        /// <summary>
        /// Adds experience points to the user's profile and handles level-up logic
        /// </summary>
        public void AddExperience(int points)
        {
            if (points <= 0) return;

            var oldLevel = Level;
            ExperiencePoints += points;
            
            // Level up calculation - like gaining levels in a video game
            var newLevel = CalculateLevel(ExperiencePoints);
            if (newLevel > Level)
            {
                Level = newLevel;
                AddDomainEvent(new UserLeveledUpEvent(this, oldLevel, newLevel));
            }

            RecordActivity();
            MarkAsUpdated();
        }

        /// <summary>
        /// Records that the user has been active, updating their last active timestamp
        /// </summary>
        public void RecordActivity()
        {
            LastActiveDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the user's email address as verified
        /// </summary>
        public void VerifyEmail()
        {
            IsEmailVerified = true;
            MarkAsUpdated();
        }

        /// <summary>
        /// Gets the user's display name (first name + last name)
        /// </summary>
        public string GetDisplayName() => $"{FirstName} {LastName}";

        /// <summary>
        /// Calculates the user's age based on their date of birth
        /// </summary>
        public int GetAge()
        {
            if (!DateOfBirth.HasValue) return 0;
            
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Value.Year;
            if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
            return age;
        }

        /// <summary>
        /// Checks if the user was active on a specific date (for streak tracking)
        /// </summary>
        public bool IsStreakDay(DateTime date)
        {
            return Activities.Any(a => a.CompletedAt?.Date == date.Date);
        }

        /// <summary>
        /// Calculates level based on experience points
        /// </summary>
        private static int CalculateLevel(int experiencePoints)
        {
            const int pointsPerLevel = 100;
            var level = 1;
            var totalRequired = pointsPerLevel;

            while (true)
            {
                if (totalRequired <= experiencePoints)
                    level++;
                else
                    break;

                totalRequired += (level * pointsPerLevel);
            }
            
            return level;
        }
    }

    /// <summary>
    /// Represents the user's gender for personalized content and metrics calculations
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// User identifies as male
        /// </summary>
        Male,

        /// <summary>
        /// User identifies as female
        /// </summary>
        Female,

        /// <summary>
        /// User identifies as non-binary
        /// </summary>
        NonBinary,

        /// <summary>
        /// User prefers not to specify their gender
        /// </summary>
        PreferNotToSay
    }

    /// <summary>
    /// Represents the user's current fitness level
    /// </summary>
    public enum FitnessLevel
    {
        /// <summary>
        /// New to fitness or returning after a long break
        /// </summary>
        Beginner,

        /// <summary>
        /// Has some fitness experience and basic form knowledge
        /// </summary>
        Intermediate,

        /// <summary>
        /// Regular exerciser with good form and endurance
        /// </summary>
        Advanced,

        /// <summary>
        /// Very experienced with excellent form and conditioning
        /// </summary>
        Expert
    }

    /// <summary>
    /// Represents the user's primary fitness goal
    /// </summary>
    public enum FitnessGoal
    {
        /// <summary>
        /// Maintain an active lifestyle
        /// </summary>
        StayActive,

        /// <summary>
        /// Lose weight through exercise and proper nutrition
        /// </summary>
        WeightLoss,

        /// <summary>
        /// Build muscle mass and strength
        /// </summary>
        BuildMuscle,

        /// <summary>
        /// Improve cardiovascular endurance
        /// </summary>
        ImproveCardio,

        /// <summary>
        /// Enhance flexibility and mobility
        /// </summary>
        Flexibility,

        /// <summary>
        /// Train for a specific sport or event
        /// </summary>
        SportSpecific,

        /// <summary>
        /// Improve overall health and wellness
        /// </summary>
        GeneralHealth
    }
}
