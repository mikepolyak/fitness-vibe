using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Queries.Activities;
using FitnessVibe.Domain.Repositories;

namespace FitnessVibe.Application.Handlers.Activities
{
    /// <summary>
    /// Handler for retrieving user's activity history - the fitness librarian.
    /// This is like having a personal archivist who organizes all your workout records,
    /// compiles comprehensive statistics, and presents your fitness journey in a meaningful way.
    /// </summary>
    public class GetUserActivitiesQueryHandler : IRequestHandler<GetUserActivitiesQuery, UserActivitiesResponse>
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IPersonalRecordRepository _personalRecordRepository;
        private readonly ISocialRepository _socialRepository;
        private readonly ILogger<GetUserActivitiesQueryHandler> _logger;

        public GetUserActivitiesQueryHandler(
            IActivityRepository activityRepository,
            IBadgeRepository badgeRepository,
            IPersonalRecordRepository personalRecordRepository,
            ISocialRepository socialRepository,
            ILogger<GetUserActivitiesQueryHandler> logger)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _badgeRepository = badgeRepository ?? throw new ArgumentNullException(nameof(badgeRepository));
            _personalRecordRepository = personalRecordRepository ?? throw new ArgumentNullException(nameof(personalRecordRepository));
            _socialRepository = socialRepository ?? throw new ArgumentNullException(nameof(socialRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserActivitiesResponse> Handle(GetUserActivitiesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving activities for user {UserId}, page {Page}", request.UserId, request.Page);

            // Build filter criteria for activities
            var filterCriteria = new ActivityFilterCriteria
            {
                UserId = request.UserId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ActivityType = request.ActivityType,
                Page = request.Page,
                PageSize = request.PageSize,
                SortBy = request.SortBy,
                SortDirection = request.SortDirection
            };

            // Get paginated activities
            var activitiesResult = await _activityRepository.GetUserActivitiesAsync(filterCriteria);

            // Build activity summaries
            var activitySummaries = new List<ActivitySummaryDto>();
            foreach (var activity in activitiesResult.Activities)
            {
                var summary = await BuildActivitySummary(activity);
                activitySummaries.Add(summary);
            }

            // Build comprehensive statistics
            var statistics = request.IncludeStats 
                ? await BuildActivityStatistics(request.UserId, request.StartDate, request.EndDate)
                : new ActivityStatisticsDto();

            // Build activity type breakdown
            var activityBreakdown = await BuildActivityBreakdown(request.UserId, request.StartDate, request.EndDate);

            // Build monthly progress
            var monthlyProgress = await BuildMonthlyProgress(request.UserId, request.StartDate, request.EndDate);

            // Build pagination info
            var pagination = new PaginationDto
            {
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)activitiesResult.TotalCount / request.PageSize),
                TotalItems = activitiesResult.TotalCount,
                HasPrevious = request.Page > 1,
                HasNext = request.Page < (int)Math.Ceiling((double)activitiesResult.TotalCount / request.PageSize)
            };

            _logger.LogDebug("Retrieved {Count} activities for user {UserId}", activitySummaries.Count, request.UserId);

            return new UserActivitiesResponse
            {
                Activities = activitySummaries,
                Statistics = statistics,
                Pagination = pagination,
                ActivityBreakdown = activityBreakdown,
                MonthlyProgress = monthlyProgress
            };
        }

        /// <summary>
        /// Builds a comprehensive summary for a single activity.
        /// Like creating a workout highlight card with all the important details.
        /// </summary>
        private async Task<ActivitySummaryDto> BuildActivitySummary(Domain.Entities.Activities.Activity activity)
        {
            // Get badges earned from this activity
            var badgesEarned = await _badgeRepository.GetActivityBadgesAsync(activity.Id);

            // Get social engagement metrics
            var socialMetrics = await _socialRepository.GetActivitySocialMetricsAsync(activity.Id);

            // Check if this activity set any personal records
            var isPersonalRecord = await _personalRecordRepository.WasPersonalRecordAsync(activity.Id);

            return new ActivitySummaryDto
            {
                Id = activity.Id,
                Name = activity.Name,
                ActivityType = activity.ActivityType,
                StartTime = activity.StartTime,
                EndTime = activity.EndTime,
                Duration = activity.EndTime.HasValue ? activity.EndTime.Value - activity.StartTime : TimeSpan.Zero,
                Distance = activity.TotalDistance,
                DistanceUnit = "km",
                CaloriesBurned = activity.CaloriesBurned,
                AverageSpeed = activity.AverageSpeed,
                AverageHeartRate = activity.AverageHeartRate,
                Status = activity.Status.ToString(),
                XpEarned = activity.XpEarned,
                BadgesEarned = badgesEarned.Select(b => b.Name).ToList(),
                IsPersonalRecord = isPersonalRecord,
                Notes = activity.Notes,
                PhotoUrls = activity.PhotoUrls.ToList(),
                LikesCount = socialMetrics.LikesCount,
                CommentsCount = socialMetrics.CommentsCount,
                IsPublic = activity.IsPublic
            };
        }

        /// <summary>
        /// Builds comprehensive activity statistics for the user.
        /// Like compiling an annual fitness report with all achievements and trends.
        /// </summary>
        private async Task<ActivityStatisticsDto> BuildActivityStatistics(int userId, DateTime? startDate, DateTime? endDate)
        {
            var stats = await _activityRepository.GetUserActivityStatisticsAsync(userId, startDate, endDate);
            var personalRecords = await _personalRecordRepository.GetUserPersonalRecordsAsync(userId);

            return new ActivityStatisticsDto
            {
                TotalActivities = stats.TotalActivities,
                TotalDuration = stats.TotalDuration,
                TotalDistance = stats.TotalDistance,
                TotalCalories = stats.TotalCalories,
                TotalXpEarned = stats.TotalXpEarned,
                CurrentStreak = stats.CurrentStreak,
                LongestStreak = stats.LongestStreak,
                AverageActivitiesPerWeek = stats.AverageActivitiesPerWeek,
                MostFrequentActivity = stats.MostFrequentActivity ?? "None",
                LastActivityDate = stats.LastActivityDate,
                PersonalRecords = personalRecords.Select(pr => new PersonalRecordDto
                {
                    RecordType = pr.RecordType,
                    Value = pr.Value,
                    AchievedAt = pr.AchievedAt,
                    ActivityName = pr.ActivityName
                }).ToList()
            };
        }

        /// <summary>
        /// Builds activity breakdown by type.
        /// Like creating a pie chart of how you spend your workout time.
        /// </summary>
        private async Task<List<ActivityTypeStatsDto>> BuildActivityBreakdown(int userId, DateTime? startDate, DateTime? endDate)
        {
            var breakdown = await _activityRepository.GetActivityTypeBreakdownAsync(userId, startDate, endDate);

            return breakdown.Select(b => new ActivityTypeStatsDto
            {
                ActivityType = b.ActivityType,
                Count = b.Count,
                TotalDuration = b.TotalDuration,
                TotalDistance = b.TotalDistance,
                TotalCalories = b.TotalCalories,
                Percentage = b.Percentage,
                LastActivity = b.LastActivity
            }).ToList();
        }

        /// <summary>
        /// Builds monthly progress tracking data.
        /// Like creating a month-by-month fitness journey timeline.
        /// </summary>
        private async Task<MonthlyProgressDto> BuildMonthlyProgress(int userId, DateTime? startDate, DateTime? endDate)
        {
            var monthlyData = await _activityRepository.GetMonthlyProgressAsync(userId, startDate, endDate);

            var progressData = monthlyData.Select(m => new MonthlyDataDto
            {
                Year = m.Year,
                Month = m.Month,
                MonthName = new DateTime(m.Year, m.Month, 1).ToString("MMMM"),
                ActivityCount = m.ActivityCount,
                TotalDuration = m.TotalDuration,
                TotalDistance = m.TotalDistance,
                TotalCalories = m.TotalCalories,
                XpEarned = m.XpEarned
            }).ToList();

            // Calculate trend
            var trend = CalculateTrend(progressData);

            return new MonthlyProgressDto
            {
                MonthlyData = progressData,
                TrendDirection = trend.Direction,
                TrendPercentage = trend.Percentage
            };
        }

        /// <summary>
        /// Calculates the trend direction and percentage for monthly progress.
        /// Like determining if your fitness is improving, stable, or declining.
        /// </summary>
        private (string Direction, double Percentage) CalculateTrend(List<MonthlyDataDto> monthlyData)
        {
            if (monthlyData.Count < 2)
                return ("Stable", 0);

            // Compare recent months to determine trend
            var recentMonths = monthlyData.TakeLast(3).ToList();
            var olderMonths = monthlyData.SkipLast(3).TakeLast(3).ToList();

            if (!olderMonths.Any())
                return ("Stable", 0);

            var recentAverage = recentMonths.Average(m => m.ActivityCount);
            var olderAverage = olderMonths.Average(m => m.ActivityCount);

            var percentageChange = olderAverage > 0 
                ? ((recentAverage - olderAverage) / olderAverage) * 100 
                : 0;

            var direction = percentageChange switch
            {
                > 10 => "Improving",
                < -10 => "Declining",
                _ => "Stable"
            };

            return (direction, Math.Abs(percentageChange));
        }
    }

    /// <summary>
    /// Filter criteria for activity queries.
    /// </summary>
    public class ActivityFilterCriteria
    {
        public int UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ActivityType { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; } = string.Empty;
        public string SortDirection { get; set; } = string.Empty;
    }
}
