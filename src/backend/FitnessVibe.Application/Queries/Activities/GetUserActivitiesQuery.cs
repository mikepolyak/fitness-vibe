using MediatR;
using FitnessVibe.Application.DTOs.Activities;

namespace FitnessVibe.Application.Queries.Activities
{
    /// <summary>
    /// Query to get user's activity history and workout timeline.
    /// Like browsing through your personal fitness journal to see all your progress.
    /// </summary>
    public class GetUserActivitiesQuery : IRequest<IEnumerable<UserActivityResponse>>
    {
        public Guid UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ActivityType { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool IncludeStats { get; set; } = true;
        public string SortBy { get; set; } = "Date"; // Date, Distance, Duration, Calories
        public string SortDirection { get; set; } = "Desc"; // Asc, Desc
    }

    /// <summary>
    /// User's complete activity history with statistics.
    /// Like a comprehensive fitness report showing your entire workout journey.
    /// </summary>
    public class UserActivitiesResponse
    {
        public List<ActivitySummaryDto> Activities { get; set; } = new();
        public ActivityStatisticsDto Statistics { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
        public List<ActivityTypeStatsDto> ActivityBreakdown { get; set; } = new();
        public MonthlyProgressDto MonthlyProgress { get; set; } = new();
    }

    /// <summary>
    /// Summary of a single workout activity.
    /// Like a workout card showing the key highlights of each session.
    /// </summary>
    public class ActivitySummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public double Distance { get; set; }
        public string DistanceUnit { get; set; } = "km";
        public int CaloriesBurned { get; set; }
        public double? AverageSpeed { get; set; }
        public int? AverageHeartRate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int XpEarned { get; set; }
        public List<string> BadgesEarned { get; set; } = new();
        public bool IsPersonalRecord { get; set; }
        public string? Notes { get; set; }
        public List<string> PhotoUrls { get; set; } = new();
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsPublic { get; set; }
    }

    /// <summary>
    /// Overall activity statistics for the user.
    /// Like your personal fitness dashboard showing all your achievements.
    /// </summary>
    public class ActivityStatisticsDto
    {
        public int TotalActivities { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public double TotalDistance { get; set; }
        public int TotalCalories { get; set; }
        public int TotalXpEarned { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public double AverageActivitiesPerWeek { get; set; }
        public string MostFrequentActivity { get; set; } = string.Empty;
        public DateTime? LastActivityDate { get; set; }
        public List<PersonalRecordDto> PersonalRecords { get; set; } = new();
    }

    /// <summary>
    /// Statistics broken down by activity type.
    /// Like seeing how much time you spend on different types of workouts.
    /// </summary>
    public class ActivityTypeStatsDto
    {
        public string ActivityType { get; set; } = string.Empty;
        public int Count { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public double TotalDistance { get; set; }
        public int TotalCalories { get; set; }
        public double Percentage { get; set; }
        public DateTime? LastActivity { get; set; }
    }

    /// <summary>
    /// Monthly progress tracking.
    /// Like a month-by-month fitness report card.
    /// </summary>
    public class MonthlyProgressDto
    {
        public List<MonthlyDataDto> MonthlyData { get; set; } = new();
        public string TrendDirection { get; set; } = string.Empty; // Improving, Stable, Declining
        public double TrendPercentage { get; set; }
    }

    /// <summary>
    /// Data for a specific month.
    /// </summary>
    public class MonthlyDataDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int ActivityCount { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public double TotalDistance { get; set; }
        public int TotalCalories { get; set; }
        public int XpEarned { get; set; }
    }

    /// <summary>
    /// Personal record information.
    /// </summary>
    public class PersonalRecordDto
    {
        public string RecordType { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime AchievedAt { get; set; }
        public string ActivityName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Pagination information.
    /// </summary>
    public class PaginationDto
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
