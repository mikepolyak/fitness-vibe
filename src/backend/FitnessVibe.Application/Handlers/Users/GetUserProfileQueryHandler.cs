using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Queries.Users;
using FitnessVibe.Domain.Repositories;

namespace FitnessVibe.Application.Handlers.Users
{
    /// <summary>
    /// Handler for retrieving user profile information - the digital receptionist that compiles your member file.
    /// This is like having a gym staff member gather all your membership details, progress reports,
    /// achievements, and preferences into one comprehensive summary.
    /// </summary>
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IClubRepository _clubRepository;
        private readonly ILogger<GetUserProfileQueryHandler> _logger;

        public GetUserProfileQueryHandler(
            IUserRepository userRepository,
            IBadgeRepository badgeRepository,
            IActivityRepository activityRepository,
            IClubRepository clubRepository,
            ILogger<GetUserProfileQueryHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _badgeRepository = badgeRepository ?? throw new ArgumentNullException(nameof(badgeRepository));
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _clubRepository = clubRepository ?? throw new ArgumentNullException(nameof(clubRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserProfileResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving user profile for user: {UserId}", request.UserId);

            // Get the user - like pulling up their membership file
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User profile not found: {UserId}", request.UserId);
                throw new ArgumentException($"User with ID {request.UserId} not found");
            }

            // Build the basic profile response
            var response = new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender?.ToString(),
                AvatarUrl = user.AvatarUrl,
                FitnessLevel = user.FitnessLevel.ToString(),
                PrimaryGoal = user.PrimaryGoal.ToString(),
                Level = user.Level,
                ExperiencePoints = user.ExperiencePoints,
                ExperiencePointsToNextLevel = CalculateExperiencePointsToNextLevel(user.Level, user.ExperiencePoints),
                IsEmailVerified = user.IsEmailVerified,
                LastActiveDate = user.LastActiveDate,
                CreatedAt = user.CreatedAt
            };

            // Gather additional information based on request parameters
            if (request.IncludeStatistics)
            {
                response.Statistics = await BuildUserStatistics(user.Id);
            }

            if (request.IncludeBadges)
            {
                response.RecentBadges = await GetRecentBadges(user.Id);
            }

            if (request.IncludePreferences)
            {
                response.Preferences = await BuildUserPreferences(user.Id);
            }

            _logger.LogDebug("Successfully retrieved user profile for user: {UserId}", request.UserId);
            
            return response;
        }

        /// <summary>
        /// Builds comprehensive fitness statistics for the user.
        /// Like compiling a detailed fitness report card showing all achievements and progress.
        /// </summary>
        private async Task<ProfileStatistics> BuildUserStatistics(int userId)
        {
            try
            {
                // Get activity summary statistics
                var activityStats = await _activityRepository.GetUserStatisticsAsync(userId);
                
                // Get social statistics
                var friendsCount = await _userRepository.GetFriendsCountAsync(userId);
                var clubMemberships = await _clubRepository.GetUserClubCountAsync(userId);
                
                // Get badge statistics
                var totalBadges = await _badgeRepository.GetUserBadgeCountAsync(userId);
                
                // Get streak information
                var currentStreak = await _userRepository.GetCurrentStreakAsync(userId);
                var longestStreak = await _userRepository.GetLongestStreakAsync(userId);
                
                // Get favorite activity
                var favoriteActivity = await _activityRepository.GetMostFrequentActivityTypeAsync(userId);

                return new ProfileStatistics
                {
                    TotalWorkouts = activityStats.TotalWorkouts,
                    TotalDistanceKm = activityStats.TotalDistanceKm,
                    TotalDuration = activityStats.TotalDuration,
                    TotalCaloriesBurned = activityStats.TotalCaloriesBurned,
                    CurrentStreak = currentStreak,
                    LongestStreak = longestStreak,
                    TotalBadges = totalBadges,
                    FriendsCount = friendsCount,
                    ClubMemberships = clubMemberships,
                    LastWorkoutDate = activityStats.LastWorkoutDate,
                    FavoriteActivity = favoriteActivity ?? "Not yet determined"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build user statistics for user: {UserId}", userId);
                
                // Return minimal statistics to prevent complete failure
                return new ProfileStatistics
                {
                    TotalWorkouts = 0,
                    TotalDistanceKm = 0,
                    TotalDuration = TimeSpan.Zero,
                    TotalCaloriesBurned = 0,
                    CurrentStreak = 0,
                    LongestStreak = 0,
                    TotalBadges = 0,
                    FriendsCount = 0,
                    ClubMemberships = 0,
                    FavoriteActivity = "Not available"
                };
            }
        }

        /// <summary>
        /// Gets the user's most recently earned badges.
        /// Like displaying your latest achievement medals on your gym locker.
        /// </summary>
        private async Task<List<BadgeInfo>> GetRecentBadges(int userId)
        {
            try
            {
                var recentBadges = await _badgeRepository.GetRecentUserBadgesAsync(userId, limit: 10);
                
                return recentBadges.Select(ub => new BadgeInfo
                {
                    Id = ub.Badge.Id,
                    Name = ub.Badge.Name,
                    Description = ub.Badge.Description,
                    IconUrl = ub.Badge.IconUrl,
                    EarnedAt = ub.EarnedAt,
                    Category = ub.Badge.Category.ToString(),
                    IsRare = ub.Badge.IsRare
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recent badges for user: {UserId}", userId);
                return new List<BadgeInfo>();
            }
        }

        /// <summary>
        /// Builds the user's app preferences and privacy settings.
        /// Like gathering all their personalized gym experience settings.
        /// </summary>
        private async Task<UserPreferences> BuildUserPreferences(int userId)
        {
            try
            {
                var userPrefs = await _userRepository.GetUserPreferencesAsync(userId);
                
                return new UserPreferences
                {
                    IsProfilePublic = userPrefs.IsProfilePublic,
                    AllowFriendRequests = userPrefs.AllowFriendRequests,
                    ShowInLeaderboards = userPrefs.ShowInLeaderboards,
                    EnablePushNotifications = userPrefs.EnablePushNotifications,
                    EnableEmailNotifications = userPrefs.EnableEmailNotifications,
                    PreferredUnits = userPrefs.PreferredUnits,
                    FavoriteActivities = userPrefs.FavoriteActivities.ToList(),
                    ShareWorkoutsWithFriends = userPrefs.ShareWorkoutsWithFriends,
                    AllowCheering = userPrefs.AllowCheering
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user preferences for user: {UserId}", userId);
                
                // Return default preferences to prevent failure
                return new UserPreferences
                {
                    IsProfilePublic = true,
                    AllowFriendRequests = true,
                    ShowInLeaderboards = true,
                    EnablePushNotifications = true,
                    EnableEmailNotifications = true,
                    PreferredUnits = "metric",
                    FavoriteActivities = new List<string>(),
                    ShareWorkoutsWithFriends = true,
                    AllowCheering = true
                };
            }
        }

        /// <summary>
        /// Calculates experience points needed to reach the next level.
        /// Like showing how many more gym visits you need for your next membership tier.
        /// </summary>
        private int CalculateExperiencePointsToNextLevel(int currentLevel, int currentXp)
        {
            // Experience requirements typically follow a progression formula
            // For simplicity, using a linear progression: Level * 1000 XP
            var nextLevelRequirement = (currentLevel + 1) * 1000;
            var currentLevelRequirement = currentLevel * 1000;
            
            return nextLevelRequirement - currentXp;
        }
    }
}
