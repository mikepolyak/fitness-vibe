using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Commands.Activities;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Activities
{
    /// <summary>
    /// Handler for starting a new activity session - the personal trainer that gets you warmed up.
    /// This is like having a motivational coach who sets up your workout equipment,
    /// starts your timer, and gives you that encouraging push to begin your fitness journey.
    /// </summary>
    public class StartActivityCommandHandler : IRequestHandler<StartActivityCommand, StartActivityResponse>
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGamificationService _gamificationService;
        private readonly ILocationService _locationService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<StartActivityCommandHandler> _logger;

        public StartActivityCommandHandler(
            IActivityRepository activityRepository,
            IUserRepository userRepository,
            IGamificationService gamificationService,
            ILocationService locationService,
            INotificationService notificationService,
            ILogger<StartActivityCommandHandler> logger)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _gamificationService = gamificationService ?? throw new ArgumentNullException(nameof(gamificationService));
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<StartActivityResponse> Handle(StartActivityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting activity session for user {UserId}, activity type: {ActivityType}", 
                request.UserId, request.ActivityType);

            // Verify user exists and is active
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {request.UserId} not found");
            }

            // Check if user has any active sessions - like ensuring they're not already in the gym
            var activeSession = await _activityRepository.GetActiveSessionAsync(request.UserId);
            if (activeSession != null)
            {
                _logger.LogWarning("User {UserId} already has an active session: {SessionId}", 
                    request.UserId, activeSession.Id);
                throw new InvalidOperationException("You already have an active workout session. Please complete or cancel it first.");
            }

            // Create activity name if not provided
            var activityName = string.IsNullOrEmpty(request.ActivityName) 
                ? GenerateActivityName(request.ActivityType, user.FirstName)
                : request.ActivityName;

            // Create the new activity session
            var activity = new Activity(
                userId: request.UserId,
                activityType: request.ActivityType,
                name: activityName,
                startTime: request.PlannedStartTime ?? DateTime.UtcNow,
                plannedDuration: request.PlannedDuration,
                isPublic: request.IsPublic
            );

            // Set location if provided
            if (request.StartLatitude.HasValue && request.StartLongitude.HasValue)
            {
                activity.SetStartLocation(
                    request.StartLatitude.Value,
                    request.StartLongitude.Value,
                    request.StartAltitude
                );
            }

            // Add notes and metadata
            if (!string.IsNullOrEmpty(request.Notes))
            {
                activity.AddNotes(request.Notes);
            }

            // Add tags
            foreach (var tag in request.Tags)
            {
                activity.AddTag(tag);
            }

            // Add metadata
            foreach (var metadata in request.Metadata)
            {
                activity.AddMetadata(metadata.Key, metadata.Value);
            }

            // Save the activity session
            await _activityRepository.AddAsync(activity);
            await _activityRepository.SaveChangesAsync();

            _logger.LogInformation("Activity session created successfully: {SessionId} for user {UserId}", 
                activity.Id, request.UserId);

            // Generate estimated statistics based on activity type and user profile
            var estimatedStats = await GenerateEstimatedStats(request.ActivityType, user, request.PlannedDuration);

            // Create motivational message based on time of day and activity type
            var motivationalMessage = CreateMotivationalMessage(request.ActivityType, user.FirstName);

            // Get recommended targets for this workout
            var recommendedTargets = await GenerateRecommendedTargets(request.ActivityType, user);

            // Generate live session URL for real-time features
            var liveSessionUrl = GenerateLiveSessionUrl(activity.Id);

            // Determine if GPS is enabled based on activity type and location availability
            var isGpsEnabled = IsGpsActivity(request.ActivityType) && 
                              request.StartLatitude.HasValue && 
                              request.StartLongitude.HasValue;

            // Notify friends if workout is public and user allows cheering
            if (request.IsPublic)
            {
                await NotifyFriendsOfWorkoutStart(user, activity);
            }

            return new StartActivityResponse
            {
                SessionId = activity.Id,
                ActivityType = activity.ActivityType,
                ActivityName = activity.Name,
                StartTime = activity.StartTime,
                Status = activity.Status.ToString(),
                IsGpsEnabled = isGpsEnabled,
                CanReceiveCheers = request.IsPublic,
                LiveSessionUrl = liveSessionUrl,
                EstimatedStats = estimatedStats,
                MotivationalMessage = motivationalMessage,
                RecommendedTargets = recommendedTargets
            };
        }

        /// <summary>
        /// Generates an engaging activity name based on the activity type and user.
        /// Like a personal trainer giving your workout session a motivating title.
        /// </summary>
        private string GenerateActivityName(string activityType, string firstName)
        {
            var timeOfDay = DateTime.Now.Hour switch
            {
                >= 5 and < 12 => "Morning",
                >= 12 and < 17 => "Afternoon",
                >= 17 and < 22 => "Evening",
                _ => "Late Night"
            };

            return activityType.ToLower() switch
            {
                "running" => $"{firstName}'s {timeOfDay} Run",
                "cycling" => $"{firstName}'s {timeOfDay} Ride",
                "swimming" => $"{firstName}'s {timeOfDay} Swim",
                "gym" => $"{firstName}'s {timeOfDay} Workout",
                "yoga" => $"{firstName}'s {timeOfDay} Yoga Session",
                "hiking" => $"{firstName}'s {timeOfDay} Hike",
                "walking" => $"{firstName}'s {timeOfDay} Walk",
                _ => $"{firstName}'s {timeOfDay} {activityType}"
            };
        }

        /// <summary>
        /// Generates estimated statistics for the planned workout.
        /// Like having a fitness tracker predict what you'll achieve based on your goals.
        /// </summary>
        private async Task<EstimatedStatsResponse> GenerateEstimatedStats(string activityType, User user, TimeSpan? plannedDuration)
        {
            // Get user's average performance for this activity type
            var historicalData = await _activityRepository.GetUserActivityAveragesAsync(user.Id, activityType);
            
            var duration = plannedDuration ?? TimeSpan.FromMinutes(30); // Default to 30 minutes
            
            var estimatedCalories = CalculateEstimatedCalories(activityType, duration, user);
            var estimatedDistance = CalculateEstimatedDistance(activityType, duration, historicalData);
            var difficultyLevel = DetermineDifficultyLevel(duration, activityType);
            var estimatedXp = CalculateEstimatedXp(duration, activityType);
            var possibleBadges = await GetPossibleBadges(user.Id, activityType);

            return new EstimatedStatsResponse
            {
                EstimatedCaloriesPerHour = (int)(estimatedCalories / duration.TotalHours),
                EstimatedDistanceIfPlanned = estimatedDistance,
                EstimatedDuration = duration,
                DifficultyLevel = difficultyLevel,
                EstimatedXpReward = estimatedXp,
                PossibleBadges = possibleBadges
            };
        }

        /// <summary>
        /// Creates a motivational message to inspire the user as they start their workout.
        /// Like having a personal trainer give you an encouraging pep talk before you begin.
        /// </summary>
        private string CreateMotivationalMessage(string activityType, string firstName)
        {
            var motivationalMessages = activityType.ToLower() switch
            {
                "running" => new[]
                {
                    $"Go {firstName}! Every step is progress toward your goals!",
                    $"Time to hit the pavement, {firstName}! Your body will thank you later!",
                    $"Ready, set, run {firstName}! Today's miles are tomorrow's smiles!",
                    $"Let's make this run count, {firstName}! You've got this!"
                },
                "cycling" => new[]
                {
                    $"Pedal to the metal, {firstName}! Your cycling adventure awaits!",
                    $"Time to roll, {firstName}! Every revolution counts!",
                    $"Let's ride, {firstName}! Feel the freedom of the open road!",
                    $"Gear up {firstName}! This ride is going to be amazing!"
                },
                "gym" => new[]
                {
                    $"Iron time, {firstName}! Transform your body, transform your life!",
                    $"Let's lift, {firstName}! Today's strength is tomorrow's confidence!",
                    $"Gym time, {firstName}! Make every rep count!",
                    $"Power up, {firstName}! Your muscles are ready to grow!"
                },
                "yoga" => new[]
                {
                    $"Find your center, {firstName}! Mind, body, and soul in harmony!",
                    $"Breathe and flow, {firstName}! Inner peace awaits!",
                    $"Namaste ready, {firstName}! Your practice begins now!",
                    $"Stretch and strengthen, {firstName}! Balance is the key!"
                },
                _ => new[]
                {
                    $"Let's do this, {firstName}! Your fitness journey continues!",
                    $"Time to move, {firstName}! Every workout matters!",
                    $"You've got this, {firstName}! Make it count!",
                    $"Let's get active, {firstName}! Your body is ready!"
                }
            };

            var random = new Random();
            return motivationalMessages[random.Next(motivationalMessages.Length)];
        }

        /// <summary>
        /// Generates recommended targets for this workout session.
        /// Like having a coach suggest achievable goals for your training.
        /// </summary>
        private async Task<List<string>> GenerateRecommendedTargets(string activityType, User user)
        {
            var targets = new List<string>();
            
            // Get user's recent performance
            var recentAverage = await _activityRepository.GetRecentAveragePerformanceAsync(user.Id, activityType);
            
            switch (activityType.ToLower())
            {
                case "running":
                    targets.Add($"Aim for {recentAverage.AverageDistance + 0.5:F1}km distance");
                    targets.Add($"Try to maintain {recentAverage.AveragePace:F1} min/km pace");
                    targets.Add("Focus on consistent breathing rhythm");
                    break;
                    
                case "cycling":
                    targets.Add($"Target {recentAverage.AverageDistance + 2:F1}km distance");
                    targets.Add($"Maintain {recentAverage.AverageSpeed + 2:F1} km/h average speed");
                    targets.Add("Keep cadence between 80-100 RPM");
                    break;
                    
                case "gym":
                    targets.Add("Increase weight by 2.5kg on main lifts");
                    targets.Add("Complete all sets with good form");
                    targets.Add("Rest 60-90 seconds between sets");
                    break;
                    
                default:
                    targets.Add("Focus on form and technique");
                    targets.Add("Listen to your body");
                    targets.Add("Enjoy the process");
                    break;
            }

            return targets;
        }

        /// <summary>
        /// Determines if this activity type typically uses GPS tracking.
        /// </summary>
        private bool IsGpsActivity(string activityType) =>
            activityType.ToLower() is "running" or "cycling" or "walking" or "hiking";

        /// <summary>
        /// Generates a URL for live session features.
        /// </summary>
        private string GenerateLiveSessionUrl(int sessionId) =>
            $"/live-session/{sessionId}";

        /// <summary>
        /// Calculates estimated calories based on activity type, duration, and user profile.
        /// </summary>
        private int CalculateEstimatedCalories(string activityType, TimeSpan duration, User user)
        {
            // Simplified calorie calculation - in reality this would be more sophisticated
            var baseCaloriesPerHour = activityType.ToLower() switch
            {
                "running" => 600,
                "cycling" => 500,
                "swimming" => 650,
                "gym" => 400,
                "yoga" => 200,
                "hiking" => 450,
                "walking" => 300,
                _ => 350
            };

            // Adjust for user factors (simplified)
            var adjustment = user.FitnessLevel switch
            {
                Domain.Enums.FitnessLevel.Beginner => 0.8,
                Domain.Enums.FitnessLevel.Intermediate => 1.0,
                Domain.Enums.FitnessLevel.Advanced => 1.2,
                _ => 1.0
            };

            return (int)(baseCaloriesPerHour * duration.TotalHours * adjustment);
        }

        /// <summary>
        /// Calculates estimated distance based on activity type and historical data.
        /// </summary>
        private double CalculateEstimatedDistance(string activityType, TimeSpan duration, dynamic historicalData)
        {
            if (!IsGpsActivity(activityType))
                return 0;

            // Use historical average or default speeds
            var averageSpeed = historicalData?.AverageSpeed ?? GetDefaultSpeed(activityType);
            return averageSpeed * duration.TotalHours;
        }

        /// <summary>
        /// Gets default speed for activity types.
        /// </summary>
        private double GetDefaultSpeed(string activityType) => activityType.ToLower() switch
        {
            "running" => 10.0, // km/h
            "cycling" => 20.0, // km/h
            "walking" => 5.0, // km/h
            "hiking" => 4.0, // km/h
            _ => 0
        };

        /// <summary>
        /// Determines workout difficulty level based on duration and activity type.
        /// </summary>
        private string DetermineDifficultyLevel(TimeSpan duration, string activityType)
        {
            var minutes = duration.TotalMinutes;
            
            return activityType.ToLower() switch
            {
                "running" => minutes switch
                {
                    < 20 => "Easy",
                    < 45 => "Moderate", 
                    < 90 => "Hard",
                    _ => "Extreme"
                },
                "cycling" => minutes switch
                {
                    < 30 => "Easy",
                    < 60 => "Moderate",
                    < 120 => "Hard", 
                    _ => "Extreme"
                },
                _ => minutes switch
                {
                    < 30 => "Easy",
                    < 60 => "Moderate",
                    < 90 => "Hard",
                    _ => "Extreme"
                }
            };
        }

        /// <summary>
        /// Calculates estimated XP reward based on activity duration and type.
        /// </summary>
        private int CalculateEstimatedXp(TimeSpan duration, string activityType)
        {
            var baseXpPerMinute = activityType.ToLower() switch
            {
                "running" => 5,
                "cycling" => 4,
                "swimming" => 6,
                "gym" => 4,
                "yoga" => 3,
                "hiking" => 4,
                "walking" => 2,
                _ => 3
            };

            return (int)(baseXpPerMinute * duration.TotalMinutes);
        }

        /// <summary>
        /// Gets possible badges the user could earn from this workout.
        /// </summary>
        private async Task<List<string>> GetPossibleBadges(int userId, string activityType)
        {
            return await _gamificationService.GetPossibleBadgesForActivityAsync(userId, activityType);
        }

        /// <summary>
        /// Notifies friends that the user has started a workout.
        /// </summary>
        private async Task NotifyFriendsOfWorkoutStart(User user, Activity activity)
        {
            try
            {
                await _notificationService.NotifyFriendsOfWorkoutStartAsync(user.Id, activity.Id, activity.Name);
                _logger.LogDebug("Notified friends of workout start for user: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to notify friends of workout start for user: {UserId}", user.Id);
                // Don't fail the activity start for this
            }
        }
    }
}
