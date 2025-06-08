using System;
using System.Collections.Generic;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.ValueObjects;
using FitnessVibe.Domain.Entities.Gamification;

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
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        public string? AvatarUrl { get; private set; }
        
        // User's fitness journey settings
        public FitnessLevel FitnessLevel { get; private set; }
        public FitnessGoal PrimaryGoal { get; private set; }
        public UserPreferences Preferences { get; private set; }
        
        // Gamification elements - the user's progression in our fitness "game"
        public int ExperiencePoints { get; private set; }
        public int Level { get; private set; }
        public DateTime LastActiveDate { get; private set; }
        
        // Privacy and security
        public string PasswordHash { get; private set; }
        public bool IsEmailVerified { get; private set; }
        public bool IsActive { get; private set; }
        
        // Navigation properties - the user's relationships in our ecosystem
        public ICollection<UserGoal> Goals { get; private set; } = new List<UserGoal>();
        public ICollection<UserBadge> Badges { get; private set; } = new List<UserBadge>();
        public ICollection<UserActivity> Activities { get; private set; } = new List<UserActivity>();

        private User() { } // For EF Core

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

        public void SetAvatar(string avatarUrl)
        {
            AvatarUrl = avatarUrl;
            MarkAsUpdated();
        }

        public void UpdateFitnessProfile(FitnessLevel fitnessLevel, FitnessGoal primaryGoal)
        {
            FitnessLevel = fitnessLevel;
            PrimaryGoal = primaryGoal;
            MarkAsUpdated();
        }

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

        public void RecordActivity()
        {
            LastActiveDate = DateTime.UtcNow;
        }

        public void VerifyEmail()
        {
            IsEmailVerified = true;
            MarkAsUpdated();
        }

        public string GetDisplayName() => $"{FirstName} {LastName}";
        
        public int GetAge()
        {
            if (!DateOfBirth.HasValue) return 0;
            
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Value.Year;
            if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
            return age;
        }

        public bool IsStreakDay(DateTime date)
        {
            // Check if the user has been active on this date
            return Activities.Any(a => a.CompletedAt.Date == date.Date);
        }

        private static int CalculateLevel(int experiencePoints)
        {
            // Progressive level calculation: 100 XP for level 1, 200 for level 2, etc.
            // Think of it like skill progression in RPGs - each level requires more effort
            if (experiencePoints < 100) return 1;
            
            var level = 1;
            var totalRequired = 0;
            
            while (totalRequired < experiencePoints)
            {
                totalRequired += level * 100; // 100, 200, 300, 400, etc.
                if (totalRequired <= experiencePoints)
                    level++;
            }
            
            return level;
        }
    }

    // Enums for user characteristics
    public enum Gender
    {
        NotSpecified,
        Male,
        Female,
        Other
    }

    public enum FitnessLevel
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert
    }

    public enum FitnessGoal
    {
        LoseWeight,
        BuildMuscle,
        ImproveEndurance,
        StayActive,
        CompeteInEvents,
        Rehabilitation
    }
}
