using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Queries.Activities;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Activities
{
    /// <summary>
    /// Handler for real-time activity tracking - the live workout monitor.
    /// This is like having a digital personal trainer watching your workout in real-time,
    /// providing live stats, friend cheers, and safety monitoring during your fitness session.
    /// </summary>
    public class GetLiveActivityQueryHandler : IRequestHandler<GetLiveActivityQuery, LiveActivityResponse>
    {
        private readonly IActivityRepository _activityRepository;
        private readonly ILiveActivityService _liveActivityService;
        private readonly ISocialService _socialService;
        private readonly IHeartRateService _heartRateService;
        private readonly ILocationService _locationService;
        private readonly IGoalService _goalService;
        private readonly ILogger<GetLiveActivityQueryHandler> _logger;

        public GetLiveActivityQueryHandler(
            IActivityRepository activityRepository,
            ILiveActivityService liveActivityService,
            ISocialService socialService,
            IHeartRateService heartRateService,
            ILocationService locationService,
            IGoalService goalService,
            ILogger<GetLiveActivityQueryHandler> logger)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _liveActivityService = liveActivityService ?? throw new ArgumentNullException(nameof(liveActivityService));
            _socialService = socialService ?? throw new ArgumentNullException(nameof(socialService));
            _heartRateService = heartRateService ?? throw new ArgumentNullException(nameof(heartRateService));
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
            _goalService = goalService ?? throw new ArgumentNullException(nameof(goalService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LiveActivityResponse> Handle(GetLiveActivityQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Getting live activity data for user {UserId}, session {SessionId}", 
                request.UserId, request.SessionId);

            // Get the active session
            var activity = request.SessionId.HasValue
                ? await _activityRepository.GetByIdAsync(request.SessionId.Value)
                : await _activityRepository.GetActiveSessionAsync(request.UserId);

            if (activity == null)
            {
                throw new InvalidOperationException("No active workout session found");
            }

            // Verify ownership
            if (activity.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Cannot access another user's live activity");
            }

            // Verify session is active
            if (activity.Status != Domain.Enums.ActivityStatus.Active)
            {
                throw new InvalidOperationException($"Activity session is {activity.Status}, not active");
            }

            // Build session information
            var session = BuildActiveSessionInfo(activity);

            // Get real-time metrics
            var currentMetrics = request.IncludeRealTimeMetrics
                ? await BuildLiveMetrics(activity)
                : new LiveMetricsDto();

            // Get recent cheers from friends
            var recentCheers = request.IncludeCheers
                ? await GetRecentCheers(activity.Id)
                : new List<LiveCheerDto>();

            // Get friends currently watching
            var friendsWatching = request.IncludeFriendsWatching
                ? await GetFriendsWatching(activity.Id)
                : new List<FriendWatchingDto>();

            // Calculate workout progress
            var progress = await BuildWorkoutProgress(activity);

            // Get upcoming milestones
            var upcomingMilestones = await GetUpcomingMilestones(activity);

            // Monitor safety status
            var safetyStatus = await BuildSafetyStatus(activity);

            // Generate live session URL
            var liveSessionUrl = $"/live-session/{activity.Id}";

            _logger.LogDebug("Successfully retrieved live activity data for session {SessionId}", activity.Id);

            return new LiveActivityResponse
            {
                Session = session,
                CurrentMetrics = currentMetrics,
                RecentCheers = recentCheers,
                FriendsWatching = friendsWatching,
                Progress = progress,
                UpcomingMilestones = upcomingMilestones,
                SafetyStatus = safetyStatus,
                LiveSessionUrl = liveSessionUrl
            };
        }

        /// <summary>
        /// Builds current active session information.
        /// Like displaying the main workout dashboard showing session details.
        /// </summary>
        private ActiveSessionDto BuildActiveSessionInfo(Domain.Entities.Activities.Activity activity)
        {
            var elapsedTime = DateTime.UtcNow - activity.StartTime;
            
            return new ActiveSessionDto
            {
                SessionId = activity.Id,
                ActivityName = activity.Name,
                ActivityType = activity.ActivityType,
                StartTime = activity.StartTime,
                ElapsedTime = elapsedTime,
                Status = activity.Status.ToString(),
                IsGpsEnabled = activity.IsGpsEnabled,
                IsPublic = activity.IsPublic,
                CanReceiveCheers = activity.IsPublic && activity.AllowCheers,
                PlannedDuration = activity.PlannedDuration,
                TargetDistance = activity.TargetDistance,
                TargetCalories = activity.TargetCalories,
                Tags = activity.Tags.ToList()
            };
        }

        /// <summary>
        /// Builds real-time workout metrics.
        /// Like reading all the sensors on your fitness tracker right now.
        /// </summary>
        private async Task<LiveMetricsDto> BuildLiveMetrics(Domain.Entities.Activities.Activity activity)
        {
            try
            {
                // Get latest real-time data
                var liveData = await _liveActivityService.GetCurrentMetricsAsync(activity.Id);
                var currentLocation = await _locationService.GetCurrentLocationAsync(activity.Id);
                var currentHeartRate = await _heartRateService.GetCurrentHeartRateAsync(activity.UserId);

                var currentDuration = DateTime.UtcNow - activity.StartTime;

                return new LiveMetricsDto
                {
                    CurrentDuration = currentDuration,
                    CurrentDistance = liveData.CurrentDistance,
                    DistanceUnit = "km",
                    CurrentCalories = liveData.CurrentCalories,
                    CurrentSpeed = liveData.CurrentSpeed,
                    CurrentPace = liveData.CurrentPace,
                    PaceUnit = "min/km",
                    CurrentHeartRate = currentHeartRate,
                    CurrentAltitude = currentLocation?.Altitude,
                    CurrentLatitude = currentLocation?.Latitude,
                    CurrentLongitude = currentLocation?.Longitude,
                    PerceivedExertion = liveData.PerceivedExertion,
                    CurrentMood = liveData.CurrentMood,
                    LastUpdate = liveData.LastUpdate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get live metrics for activity {ActivityId}", activity.Id);
                
                // Return basic metrics if live data unavailable
                return new LiveMetricsDto
                {
                    CurrentDuration = DateTime.UtcNow - activity.StartTime,
                    CurrentDistance = activity.TotalDistance,
                    DistanceUnit = "km",
                    CurrentCalories = activity.CaloriesBurned,
                    LastUpdate = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Gets recent cheers and encouragement from friends.
        /// Like collecting all the motivational messages from your fitness squad!
        /// </summary>
        private async Task<List<LiveCheerDto>> GetRecentCheers(int activityId)
        {
            try
            {
                var cheers = await _socialService.GetRecentCheersAsync(activityId, limit: 10);
                
                return cheers.Select(c => new LiveCheerDto
                {
                    Id = c.Id,
                    FromUserName = c.FromUserName,
                    FromUserAvatarUrl = c.FromUserAvatarUrl,
                    CheerType = c.CheerType.ToString(),
                    Message = c.Message,
                    AudioUrl = c.AudioUrl,
                    EmojiCode = c.EmojiCode,
                    SentAt = c.SentAt,
                    IsRead = c.IsRead,
                    PowerUpValue = c.PowerUpValue
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recent cheers for activity {ActivityId}", activityId);
                return new List<LiveCheerDto>();
            }
        }

        /// <summary>
        /// Gets friends currently watching the live workout.
        /// Like seeing who's cheering you on from the sidelines!
        /// </summary>
        private async Task<List<FriendWatchingDto>> GetFriendsWatching(int activityId)
        {
            try
            {
                var watchers = await _socialService.GetFriendsWatchingAsync(activityId);
                
                return watchers.Select(w => new FriendWatchingDto
                {
                    UserId = w.UserId,
                    UserName = w.UserName,
                    AvatarUrl = w.AvatarUrl,
                    StartedWatchingAt = w.StartedWatchingAt,
                    CanSendCheers = w.CanSendCheers,
                    CurrentActivity = w.CurrentActivity
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get friends watching for activity {ActivityId}", activityId);
                return new List<FriendWatchingDto>();
            }
        }

        /// <summary>
        /// Builds workout progress towards goals.
        /// Like showing your progress bars toward all your workout targets.
        /// </summary>
        private async Task<WorkoutProgressDto> BuildWorkoutProgress(Domain.Entities.Activities.Activity activity)
        {
            try
            {
                var goals = await _goalService.GetWorkoutGoalsAsync(activity.Id);
                var currentMetrics = await _liveActivityService.GetCurrentMetricsAsync(activity.Id);
                
                var goalProgress = goals.Select(g => new GoalProgressDto
                {
                    GoalType = g.GoalType,
                    GoalName = g.GoalName,
                    CurrentValue = g.CurrentValue,
                    TargetValue = g.TargetValue,
                    ProgressPercentage = g.ProgressPercentage,
                    Unit = g.Unit,
                    IsAchieved = g.IsAchieved,
                    EstimatedTimeToComplete = g.EstimatedTimeToComplete
                }).ToList();

                var estimatedCompletion = await CalculateEstimatedCompletion(activity, currentMetrics);

                return new WorkoutProgressDto
                {
                    DurationProgressPercentage = CalculateDurationProgress(activity),
                    DistanceProgressPercentage = CalculateDistanceProgress(activity),
                    CaloriesProgressPercentage = CalculateCaloriesProgress(activity),
                    GoalProgress = goalProgress,
                    EstimatedCompletion = estimatedCompletion,
                    OverallPace = DetermineOverallPace(activity, currentMetrics)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build workout progress for activity {ActivityId}", activity.Id);
                return new WorkoutProgressDto();
            }
        }

        /// <summary>
        /// Gets upcoming milestones in the workout.
        /// Like showing the next checkpoint rewards you can earn!
        /// </summary>
        private async Task<List<MilestoneDto>> GetUpcomingMilestones(Domain.Entities.Activities.Activity activity)
        {
            try
            {
                var milestones = await _goalService.GetUpcomingMilestonesAsync(activity.Id);
                
                return milestones.Select(m => new MilestoneDto
                {
                    MilestoneType = m.MilestoneType,
                    Name = m.Name,
                    Description = m.Description,
                    TargetValue = m.TargetValue,
                    CurrentValue = m.CurrentValue,
                    Unit = m.Unit,
                    ProgressPercentage = m.ProgressPercentage,
                    EstimatedTimeToReach = m.EstimatedTimeToReach,
                    XpReward = m.XpReward,
                    IsPersonalRecord = m.IsPersonalRecord
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get upcoming milestones for activity {ActivityId}", activity.Id);
                return new List<MilestoneDto>();
            }
        }

        /// <summary>
        /// Builds safety and wellness monitoring status.
        /// Like having a fitness supervisor ensuring you're working out safely.
        /// </summary>
        private async Task<SafetyStatusDto> BuildSafetyStatus(Domain.Entities.Activities.Activity activity)
        {
            try
            {
                var safetyData = await _liveActivityService.GetSafetyStatusAsync(activity.Id);
                var heartRateZone = await _heartRateService.GetCurrentHeartRateZoneAsync(activity.UserId);
                
                return new SafetyStatusDto
                {
                    OverallStatus = safetyData.OverallStatus,
                    ActiveAlerts = safetyData.ActiveAlerts.Select(a => new SafetyAlertDto
                    {
                        AlertType = a.AlertType,
                        Severity = a.Severity,
                        Message = a.Message,
                        Recommendation = a.Recommendation,
                        TriggeredAt = a.TriggeredAt
                    }).ToList(),
                    HeartRateZone = heartRateZone != null ? new HeartRateZoneDto
                    {
                        CurrentZone = heartRateZone.CurrentZone,
                        CurrentHeartRate = heartRateZone.CurrentHeartRate,
                        ZoneMinHeartRate = heartRateZone.ZoneMinHeartRate,
                        ZoneMaxHeartRate = heartRateZone.ZoneMaxHeartRate,
                        TimeInZonePercentage = heartRateZone.TimeInZonePercentage,
                        ZoneColor = heartRateZone.ZoneColor,
                        IsTargetZone = heartRateZone.IsTargetZone
                    } : null,
                    LastCheckIn = safetyData.LastCheckIn,
                    AutoPauseEnabled = safetyData.AutoPauseEnabled,
                    EmergencyContactsNotified = safetyData.EmergencyContactsNotified
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get safety status for activity {ActivityId}", activity.Id);
                
                // Return safe default status
                return new SafetyStatusDto
                {
                    OverallStatus = "Good",
                    ActiveAlerts = new List<SafetyAlertDto>(),
                    AutoPauseEnabled = false,
                    EmergencyContactsNotified = false
                };
            }
        }

        /// <summary>
        /// Calculates duration progress percentage.
        /// </summary>
        private double CalculateDurationProgress(Domain.Entities.Activities.Activity activity)
        {
            if (!activity.PlannedDuration.HasValue)
                return 0;

            var elapsed = DateTime.UtcNow - activity.StartTime;
            return Math.Min(100, (elapsed.TotalMinutes / activity.PlannedDuration.Value.TotalMinutes) * 100);
        }

        /// <summary>
        /// Calculates distance progress percentage.
        /// </summary>
        private double CalculateDistanceProgress(Domain.Entities.Activities.Activity activity)
        {
            if (!activity.TargetDistance.HasValue || activity.TargetDistance.Value == 0)
                return 0;

            return Math.Min(100, (activity.TotalDistance / activity.TargetDistance.Value) * 100);
        }

        /// <summary>
        /// Calculates calories progress percentage.
        /// </summary>
        private double CalculateCaloriesProgress(Domain.Entities.Activities.Activity activity)
        {
            if (!activity.TargetCalories.HasValue || activity.TargetCalories.Value == 0)
                return 0;

            return Math.Min(100, ((double)activity.CaloriesBurned / activity.TargetCalories.Value) * 100);
        }

        /// <summary>
        /// Calculates estimated completion time and remaining work.
        /// </summary>
        private async Task<EstimatedCompletionDto> CalculateEstimatedCompletion(
            Domain.Entities.Activities.Activity activity, 
            dynamic currentMetrics)
        {
            try
            {
                var estimation = await _liveActivityService.CalculateEstimatedCompletionAsync(activity.Id);
                
                return new EstimatedCompletionDto
                {
                    EstimatedFinishTime = estimation.EstimatedFinishTime,
                    TimeRemaining = estimation.TimeRemaining,
                    DistanceRemaining = estimation.DistanceRemaining,
                    CaloriesRemaining = estimation.CaloriesRemaining,
                    Confidence = estimation.Confidence
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate estimated completion for activity {ActivityId}", activity.Id);
                return new EstimatedCompletionDto { Confidence = 0 };
            }
        }

        /// <summary>
        /// Determines overall pace compared to goals.
        /// </summary>
        private string DetermineOverallPace(Domain.Entities.Activities.Activity activity, dynamic currentMetrics)
        {
            try
            {
                // Simple pace calculation based on progress vs. time elapsed
                var timeProgress = CalculateDurationProgress(activity);
                var distanceProgress = CalculateDistanceProgress(activity);
                
                if (distanceProgress == 0) return "On Track";
                
                var paceRatio = distanceProgress / Math.Max(timeProgress, 1);
                
                return paceRatio switch
                {
                    > 1.1 => "Ahead",
                    < 0.9 => "Behind", 
                    _ => "On Track"
                };
            }
            catch
            {
                return "On Track";
            }
        }
    }
}
