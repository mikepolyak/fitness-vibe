using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Commands.Activities;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Activities
{
    /// <summary>
    /// Handler for completing workout sessions - the victory celebration coordinator.
    /// This is like having a personal trainer who celebrates your finish, tallies your achievements,
    /// updates your records, and motivates you for your next workout. Every completion deserves recognition!
    /// </summary>
    public class CompleteActivityCommandHandler : IRequestHandler<CompleteActivityCommand, CompleteActivityResponse>
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGamificationService _gamificationService;
        private readonly IPersonalRecordService _personalRecordService;
        private readonly ISocialService _socialService;
        private readonly IRecommendationService _recommendationService;
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<CompleteActivityCommandHandler> _logger;

        public CompleteActivityCommandHandler(
            IActivityRepository activityRepository,
            IUserRepository userRepository,
            IGamificationService gamificationService,
            IPersonalRecordService personalRecordService,
            ISocialService socialService,
            IRecommendationService recommendationService,
            IAnalyticsService analyticsService,
            ILogger<CompleteActivityCommandHandler> logger)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _gamificationService = gamificationService ?? throw new ArgumentNullException(nameof(gamificationService));
            _personalRecordService = personalRecordService ?? throw new ArgumentNullException(nameof(personalRecordService));
            _socialService = socialService ?? throw new ArgumentNullException(nameof(socialService));
            _recommendationService = recommendationService ?? throw new ArgumentNullException(nameof(recommendationService));
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CompleteActivityResponse> Handle(CompleteActivityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Completing activity session {SessionId} for user {UserId}", 
                request.SessionId, request.UserId);

            // Get the activity session
            var activity = await _activityRepository.GetByIdAsync(request.SessionId);
            if (activity == null)
            {
                throw new ArgumentException($"Activity session {request.SessionId} not found");
            }

            // Verify ownership
            if (activity.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You can only complete your own activities");
            }

            // Verify session is active
            if (activity.Status != Domain.Enums.ActivityStatus.Active)
            {
                throw new InvalidOperationException($"Activity session is already {activity.Status}");
            }

            // Complete the activity with provided data
            var endTime = request.EndTime ?? DateTime.UtcNow;
            
            activity.Complete(
                endTime: endTime,
                endLatitude: request.EndLatitude,
                endLongitude: request.EndLongitude,
                endAltitude: request.EndAltitude,
                manualCalories: request.ManualCalories,
                manualDistance: request.ManualDistance,
                perceivedExertion: request.PerceivedExertion,
                moodAfter: request.MoodAfter
            );

            // Add completion notes if provided
            if (!string.IsNullOrEmpty(request.Notes))
            {
                activity.AddCompletionNotes(request.Notes);
            }

            // Add workout photos
            foreach (var photoUrl in request.PhotoUrls)
            {
                activity.AddPhoto(photoUrl);
            }

            // Add additional metrics
            foreach (var metric in request.AdditionalMetrics)
            {
                activity.AddMetadata(metric.Key, metric.Value);
            }

            // Save the completed activity
            await _activityRepository.UpdateAsync(activity);
            await _activityRepository.SaveChangesAsync();

            _logger.LogInformation("Activity session {SessionId} completed successfully", request.SessionId);

            // Calculate comprehensive workout statistics
            var workoutStats = await CalculateWorkoutStats(activity);

            // Process gamification rewards (XP, badges, level ups)
            var rewards = await ProcessGamificationRewards(activity);

            // Check and update personal records
            var personalRecords = await ProcessPersonalRecords(activity);

            // Generate celebration message
            var celebrationMessage = CreateCelebrationMessage(activity, rewards, personalRecords);

            // Handle social sharing if requested
            var socialShare = await HandleSocialSharing(activity, request.ShareToFeed, request.TagFriends);

            // Generate next workout suggestions
            var nextSuggestion = await GenerateNextWorkoutSuggestion(activity);

            // Track analytics
            await TrackCompletionAnalytics(activity, workoutStats);

            // Determine friends who will see this workout
            var friendsWhoWillSeeThis = await CalculateFriendsVisibility(activity, request.ShareToFeed);

            return new CompleteActivityResponse
            {
                ActivityId = activity.Id,
                ActivityName = activity.Name,
                Stats = workoutStats,
                Rewards = rewards,
                PersonalRecords = personalRecords,
                CelebrationMessage = celebrationMessage,
                Achievements = BuildAchievementsList(rewards, personalRecords),
                SocialShare = socialShare,
                NextSuggestion = nextSuggestion,
                CreatedNewPersonalRecord = personalRecords.NewRecords.Any(),
                FriendsWhoWillSeeThis = friendsWhoWillSeeThis
            };
        }

        /// <summary>
        /// Calculates comprehensive workout statistics.
        /// Like having a data analyst crunch all your performance numbers.
        /// </summary>
        private async Task<WorkoutStatsResponse> CalculateWorkoutStats(Domain.Entities.Activities.Activity activity)
        {
            var duration = activity.EndTime!.Value - activity.StartTime;
            var splits = await _analyticsService.CalculateWorkoutSplitsAsync(activity.Id);
            
            return new WorkoutStatsResponse
            {
                Duration = duration,
                ActiveTime = activity.ActiveTime ?? duration,
                RestTime = duration - (activity.ActiveTime ?? duration),
                Distance = activity.TotalDistance,
                DistanceUnit = "km",
                CaloriesBurned = activity.CaloriesBurned,
                AverageSpeed = activity.AverageSpeed,
                MaxSpeed = activity.MaxSpeed,
                ElevationGain = activity.ElevationGain,
                AverageHeartRate = activity.AverageHeartRate,
                MaxHeartRate = activity.MaxHeartRate,
                Splits = splits.Select(s => new WorkoutSplitResponse
                {
                    SplitNumber = s.SplitNumber,
                    Distance = s.Distance,
                    Duration = s.Duration,
                    Pace = s.Pace,
                    PaceUnit = "min/km"
                }).ToList(),
                PerformanceRating = DeterminePerformanceRating(activity)
            };
        }

        /// <summary>
        /// Processes gamification rewards for the completed workout.
        /// Like having a game master calculate all your points and unlocks!
        /// </summary>
        private async Task<RewardsEarnedResponse> ProcessGamificationRewards(Domain.Entities.Activities.Activity activity)
        {
            var rewards = await _gamificationService.ProcessActivityCompletionAsync(activity.Id);
            
            return new RewardsEarnedResponse
            {
                XpEarned = rewards.BaseXp,
                XpBonus = rewards.BonusXp,
                XpBonusReason = rewards.BonusReason,
                BadgesEarned = rewards.BadgesEarned.Select(b => new BadgeEarnedResponse
                {
                    Name = b.Name,
                    Description = b.Description,
                    IconUrl = b.IconUrl,
                    Rarity = b.Rarity.ToString(),
                    Points = b.Points,
                    IsFirstTime = b.IsFirstTime
                }).ToList(),
                LeveledUp = rewards.LeveledUp,
                NewLevel = rewards.NewLevel,
                NewLevelTitle = rewards.NewLevelTitle,
                UnlockedFeatures = rewards.UnlockedFeatures.ToList(),
                StreakDays = rewards.StreakDays,
                StreakMilestone = rewards.StreakMilestone
            };
        }

        /// <summary>
        /// Processes personal records and achievements.
        /// Like updating your trophy case with new best performances!
        /// </summary>
        private async Task<PersonalRecordsResponse> ProcessPersonalRecords(Domain.Entities.Activities.Activity activity)
        {
            var recordResults = await _personalRecordService.CheckAndUpdateRecordsAsync(activity.Id);
            
            return new PersonalRecordsResponse
            {
                NewRecords = recordResults.NewRecords.Select(r => new PersonalRecordResponse
                {
                    RecordType = r.RecordType,
                    NewValue = r.NewValue,
                    PreviousValue = r.PreviousValue,
                    Improvement = r.Improvement,
                    AchievedAt = r.AchievedAt
                }).ToList(),
                ImprovedRecords = recordResults.ImprovedRecords.Select(r => new PersonalRecordResponse
                {
                    RecordType = r.RecordType,
                    NewValue = r.NewValue,
                    PreviousValue = r.PreviousValue,
                    Improvement = r.Improvement,
                    AchievedAt = r.AchievedAt
                }).ToList()
            };
        }

        /// <summary>
        /// Creates an enthusiastic celebration message for the completed workout.
        /// Like having a cheerleader celebrate your accomplishments!
        /// </summary>
        private string CreateCelebrationMessage(
            Domain.Entities.Activities.Activity activity, 
            RewardsEarnedResponse rewards, 
            PersonalRecordsResponse personalRecords)
        {
            var messages = new List<string>();
            
            // Base celebration
            var duration = activity.EndTime!.Value - activity.StartTime;
            messages.Add($"🎉 Workout complete! You crushed {duration.TotalMinutes:F0} minutes of {activity.ActivityType.ToLower()}!");

            // XP celebration
            var totalXp = rewards.XpEarned + rewards.XpBonus;
            messages.Add($"💪 Earned {totalXp} XP!");

            // Level up celebration
            if (rewards.LeveledUp)
            {
                messages.Add($"🚀 LEVEL UP! Welcome to level {rewards.NewLevel}!");
            }

            // Badge celebration
            if (rewards.BadgesEarned.Any())
            {
                var badgeCount = rewards.BadgesEarned.Count;
                messages.Add($"🏆 Unlocked {badgeCount} new badge{(badgeCount > 1 ? "s" : "")}!");
            }

            // Personal record celebration
            if (personalRecords.NewRecords.Any())
            {
                messages.Add("🔥 NEW PERSONAL RECORD! You're stronger than ever!");
            }

            // Streak celebration
            if (rewards.StreakDays > 1)
            {
                messages.Add($"⚡ {rewards.StreakDays}-day streak! Consistency is key!");
            }

            return string.Join(" ", messages);
        }

        /// <summary>
        /// Handles social sharing of the completed workout.
        /// Like posting your achievement on the gym's bulletin board!
        /// </summary>
        private async Task<SocialShareResponse> HandleSocialSharing(
            Domain.Entities.Activities.Activity activity, 
            bool shareToFeed, 
            List<string> tagFriends)
        {
            if (!shareToFeed)
            {
                return new SocialShareResponse { WasShared = false };
            }

            try
            {
                var shareResult = await _socialService.ShareWorkoutAsync(activity.Id, tagFriends);
                
                return new SocialShareResponse
                {
                    WasShared = true,
                    PostId = shareResult.PostId,
                    ShareableMessage = shareResult.Message,
                    ShareableImageUrl = shareResult.ImageUrl,
                    SuggestedHashtags = shareResult.SuggestedHashtags.ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to share workout for activity: {ActivityId}", activity.Id);
                return new SocialShareResponse { WasShared = false };
            }
        }

        /// <summary>
        /// Generates intelligent suggestions for the next workout.
        /// Like having a personal trainer plan your next session!
        /// </summary>
        private async Task<NextWorkoutSuggestionResponse> GenerateNextWorkoutSuggestion(Domain.Entities.Activities.Activity activity)
        {
            try
            {
                var suggestion = await _recommendationService.GetNextWorkoutSuggestionAsync(activity.UserId, activity.Id);
                
                return new NextWorkoutSuggestionResponse
                {
                    SuggestedActivity = suggestion.ActivityType,
                    Reasoning = suggestion.Reasoning,
                    SuggestedTime = suggestion.SuggestedTime,
                    DifficultyLevel = suggestion.DifficultyLevel,
                    EstimatedDuration = suggestion.EstimatedDuration,
                    FocusArea = suggestion.FocusArea
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate next workout suggestion for user: {UserId}", activity.UserId);
                
                // Return a generic suggestion
                return new NextWorkoutSuggestionResponse
                {
                    SuggestedActivity = "Rest",
                    Reasoning = "Take a well-deserved recovery day!",
                    SuggestedTime = DateTime.Now.AddDays(1),
                    DifficultyLevel = "Easy",
                    EstimatedDuration = TimeSpan.FromMinutes(30),
                    FocusArea = "Recovery"
                };
            }
        }

        /// <summary>
        /// Determines performance rating based on workout metrics.
        /// </summary>
        private string DeterminePerformanceRating(Domain.Entities.Activities.Activity activity)
        {
            // Simplified performance rating logic
            var perceivedExertion = activity.PerceivedExertion ?? 5;
            
            return perceivedExertion switch
            {
                <= 3 => "Easy Recovery",
                <= 5 => "Good Effort",
                <= 7 => "Strong Performance",
                <= 9 => "Excellent Workout",
                _ => "Beast Mode!"
            };
        }

        /// <summary>
        /// Builds a list of achievements from rewards and records.
        /// </summary>
        private List<string> BuildAchievementsList(RewardsEarnedResponse rewards, PersonalRecordsResponse personalRecords)
        {
            var achievements = new List<string>();
            
            if (rewards.LeveledUp)
                achievements.Add($"Reached Level {rewards.NewLevel}!");
                
            foreach (var badge in rewards.BadgesEarned)
                achievements.Add($"Earned '{badge.Name}' badge!");
                
            foreach (var record in personalRecords.NewRecords)
                achievements.Add($"New {record.RecordType} record!");
                
            if (rewards.StreakMilestone)
                achievements.Add($"{rewards.StreakDays}-day streak milestone!");
                
            return achievements;
        }

        /// <summary>
        /// Tracks analytics for the completed workout.
        /// </summary>
        private async Task TrackCompletionAnalytics(Domain.Entities.Activities.Activity activity, WorkoutStatsResponse stats)
        {
            try
            {
                await _analyticsService.TrackWorkoutCompletionAsync(activity.Id, stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to track workout completion analytics for activity: {ActivityId}", activity.Id);
                // Don't fail the completion for analytics issues
            }
        }

        /// <summary>
        /// Calculates how many friends will see this workout.
        /// </summary>
        private async Task<int> CalculateFriendsVisibility(Domain.Entities.Activities.Activity activity, bool shareToFeed)
        {
            if (!shareToFeed)
                return 0;
                
            try
            {
                return await _socialService.GetFriendsWhoWillSeeWorkoutAsync(activity.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate friends visibility for user: {UserId}", activity.UserId);
                return 0;
            }
        }
    }
}
