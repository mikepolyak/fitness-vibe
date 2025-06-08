using MediatR;

namespace FitnessVibe.Application.Queries.Users
{
    /// <summary>
    /// Query to get a user's profile information.
    /// Like checking your membership details and fitness progress at the gym's member portal.
    /// </summary>
    public class GetUserProfileQuery : IRequest<UserProfileResponse>
    {
        public int UserId { get; set; }
        public bool IncludeStatistics { get; set; } = true;
        public bool IncludeBadges { get; set; } = true;
        public bool IncludePreferences { get; set; } = true;
    }

    /// <summary>
    /// Comprehensive user profile information.
    /// Like a detailed membership report showing your fitness journey and achievements.
    /// </summary>
    public class UserProfileResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public string FitnessLevel { get; set; } = string.Empty;
        public string PrimaryGoal { get; set; } = string.Empty;
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public int ExperiencePointsToNextLevel { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime LastActiveDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public ProfileStatistics? Statistics { get; set; }
        public List<BadgeInfo> RecentBadges { get; set; } = new();
        public UserPreferences? Preferences { get; set; }
    }

    /// <summary>
    /// User's fitness statistics and achievements.
    /// Like your personal fitness report card showing how you're progressing.
    /// </summary>
    public class ProfileStatistics
    {
        public int TotalWorkouts { get; set; }
        public double TotalDistanceKm { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public int TotalCaloriesBurned { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int TotalBadges { get; set; }
        public int FriendsCount { get; set; }
        public int ClubMemberships { get; set; }
        public DateTime? LastWorkoutDate { get; set; }
        public string FavoriteActivity { get; set; } = string.Empty;
    }

    /// <summary>
    /// Information about earned badges.
    /// Like the achievement medals on your gym locker.
    /// </summary>
    public class BadgeInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public DateTime EarnedAt { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsRare { get; set; }
    }

    /// <summary>
    /// User's fitness app preferences.
    /// Like your personal settings for how you like your gym experience configured.
    /// </summary>
    public class UserPreferences
    {
        public bool IsProfilePublic { get; set; }
        public bool AllowFriendRequests { get; set; }
        public bool ShowInLeaderboards { get; set; }
        public bool EnablePushNotifications { get; set; }
        public bool EnableEmailNotifications { get; set; }
        public string PreferredUnits { get; set; } = "metric"; // metric or imperial
        public List<string> FavoriteActivities { get; set; } = new();
        public bool ShareWorkoutsWithFriends { get; set; } = true;
        public bool AllowCheering { get; set; } = true;
    }
}
