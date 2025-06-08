using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Queries.Gamification;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Gamification
{
    /// <summary>
    /// Handler for retrieving user's gamification dashboard - the achievement showcase coordinator.
    /// This is like having a personal trophy room curator who organizes all your fitness accomplishments,
    /// shows your progress, and motivates you toward your next achievements.
    /// </summary>
    public class GetUserGamificationQueryHandler : IRequestHandler<GetUserGamificationQuery, UserGamificationResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IGamificationRepository _gamificationRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly ILeaderboardService _leaderboardService;
        private readonly ILevelService _levelService;
        private readonly ILogger<GetUserGamificationQueryHandler> _logger;

        public GetUserGamificationQueryHandler(
            IUserRepository userRepository,
            IGamificationRepository gamificationRepository,
            IBadgeRepository badgeRepository,
            IChallengeRepository challengeRepository,
            ILeaderboardService leaderboardService,
            ILevelService levelService,
            ILogger<GetUserGamificationQueryHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _gamificationRepository = gamificationRepository ?? throw new ArgumentNullException(nameof(gamificationRepository));
            _badgeRepository = badgeRepository ?? throw new ArgumentNullException(nameof(badgeRepository));
            _challengeRepository = challengeRepository ?? throw new ArgumentNullException(nameof(challengeRepository));
            _leaderboardService = leaderboardService ?? throw new ArgumentNullException(nameof(leaderboardService));
            _levelService = levelService ?? throw new ArgumentNullException(nameof(levelService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserGamificationResponse> Handle(GetUserGamificationQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving gamification data for user {UserId}", request.UserId);

            // Get the user
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {request.UserId} not found");
            }

            // Build level information
            var levelInfo = await BuildUserLevelInfo(user);

            // Build badge summary
            var badgeSummary = await BuildBadgeSummary(request.UserId);

            // Get recent achievements
            var recentAchievements = request.IncludeRecentAchievements
                ? await GetRecentAchievements(request.UserId)
                : new List<RecentAchievementDto>();

            // Get badge progress
            var badgeProgress = request.IncludeBadgeProgress
                ? await GetBadgeProgress(request.UserId)
                : new List<BadgeProgressDto>();

            // Get upcoming milestones
            var upcomingMilestones = request.IncludeNextMilestones
                ? await GetUpcomingMilestones(request.UserId)
                : new List<MilestoneDto>();

            // Get leaderboard position
            var leaderboardPosition = request.IncludeLeaderboardPosition
                ? await GetLeaderboardPosition(request.UserId)
                : new LeaderboardPositionDto();

            // Get streak information
            var streaks = await GetStreakInfo(request.UserId);

            // Get active challenges
            var activeChallenges = await GetActiveChallenges(request.UserId);

            // Generate motivational message
            var motivationalMessage = CreateMotivationalMessage(user, levelInfo, badgeSummary, streaks);

            _logger.LogDebug("Successfully retrieved gamification data for user {UserId}", request.UserId);

            return new UserGamificationResponse
            {
                Level = levelInfo,
                Badges = badgeSummary,
                RecentAchievements = recentAchievements,
                BadgeProgress = badgeProgress,
                UpcomingMilestones = upcomingMilestones,
                LeaderboardPosition = leaderboardPosition,
                Streaks = streaks,
                ActiveChallenges = activeChallenges,
                MotivationalMessage = motivationalMessage
            };
        }

        /// <summary>
        /// Builds comprehensive level and XP information.
        /// Like creating a detailed progress report for your fitness journey!
        /// </summary>
        private async Task<UserLevelInfo> BuildUserLevelInfo(Domain.Entities.Users.User user)
        {
            var levelDetails = await _levelService.GetLevelDetailsAsync(user.Level);
            var nextLevelDetails = await _levelService.GetLevelDetailsAsync(user.Level + 1);
            var xpToNextLevel = await _levelService.CalculateXpToNextLevelAsync(user.Level, user.ExperiencePoints);
            
            var xpForCurrentLevel = levelDetails?.XpRequired ?? 0;
            var xpForNextLevel = nextLevelDetails?.XpRequired ?? xpForCurrentLevel;
            var progressToNextLevel = xpForNextLevel > xpForCurrentLevel 
                ? (double)(user.ExperiencePoints - xpForCurrentLevel) / (xpForNextLevel - xpForCurrentLevel) * 100
                : 100;

            return new UserLevelInfo
            {
                CurrentLevel = user.Level,
                LevelTitle = levelDetails?.Title ?? $"Level {user.Level}",
                CurrentXp = user.ExperiencePoints,
                XpForCurrentLevel = xpForCurrentLevel,
                XpForNextLevel = xpForNextLevel,
                XpToNextLevel = xpToNextLevel,
                ProgressToNextLevel = Math.Min(progressToNextLevel, 100),
                NextLevelTitle = nextLevelDetails?.Title ?? $"Level {user.Level + 1}",
                NextLevelUnlocks = nextLevelDetails?.UnlockedFeatures?.ToList() ?? new List<string>(),
                LastLevelUpDate = user.LastLevelUpDate,
                TotalXpEarned = user.ExperiencePoints
            };
        }

        /// <summary>
        /// Builds a comprehensive badge collection summary.
        /// Like organizing your trophy case and highlighting your best achievements!
        /// </summary>
        private async Task<BadgeSummaryDto> BuildBadgeSummary(int userId)
        {
            var allUserBadges = await _badgeRepository.GetUserBadgesAsync(userId);
            var recentBadges = allUserBadges.Where(ub => ub.EarnedAt > DateTime.UtcNow.AddDays(-30)).ToList();
            var featuredBadges = allUserBadges.Where(ub => ub.Badge.IsRare || ub.Badge.Category == Domain.Enums.BadgeCategory.Achievement)
                                             .OrderByDescending(ub => ub.Badge.XpValue)
                                             .Take(5)
                                             .ToList();

            var categoryStats = new BadgeCategoryStatsDto
            {
                ActivityBadges = allUserBadges.Count(ub => ub.Badge.Category == Domain.Enums.BadgeCategory.Activity),
                SocialBadges = allUserBadges.Count(ub => ub.Badge.Category == Domain.Enums.BadgeCategory.Social),
                StreakBadges = allUserBadges.Count(ub => ub.Badge.Category == Domain.Enums.BadgeCategory.Streak),
                ChallengeBadges = allUserBadges.Count(ub => ub.Badge.Category == Domain.Enums.BadgeCategory.Challenge),
                MilestoneBadges = allUserBadges.Count(ub => ub.Badge.Category == Domain.Enums.BadgeCategory.Milestone),
                SpecialBadges = allUserBadges.Count(ub => ub.Badge.Category == Domain.Enums.BadgeCategory.Special)
            };

            // Determine strongest category
            var categoryValues = new Dictionary<string, int>
            {
                ["Activity"] = categoryStats.ActivityBadges,
                ["Social"] = categoryStats.SocialBadges,
                ["Streak"] = categoryStats.StreakBadges,
                ["Challenge"] = categoryStats.ChallengeBadges,
                ["Milestone"] = categoryStats.MilestoneBadges,
                ["Special"] = categoryStats.SpecialBadges
            };
            categoryStats.StrongestCategory = categoryValues.OrderByDescending(kv => kv.Value).First().Key;

            var badgeCollectorTitle = DetermineBadgeCollectorTitle(allUserBadges.Count);

            return new BadgeSummaryDto
            {
                TotalBadges = allUserBadges.Count,
                RecentBadges = recentBadges.Count,
                FeaturedBadges = featuredBadges.Select(ub => new BadgeDetailsDto
                {
                    Id = ub.Badge.Id,
                    Code = ub.Badge.Code,
                    Name = ub.Badge.Name,
                    Description = ub.Badge.Description,
                    IconUrl = ub.Badge.IconUrl,
                    Category = ub.Badge.Category.ToString(),
                    Rarity = ub.Badge.Rarity.ToString(),
                    XpValue = ub.Badge.XpValue,
                    EarnedAt = ub.EarnedAt
                }).ToList(),
                RecentlyEarned = recentBadges.Take(5).Select(ub => new BadgeDetailsDto
                {
                    Id = ub.Badge.Id,
                    Code = ub.Badge.Code,
                    Name = ub.Badge.Name,
                    Description = ub.Badge.Description,
                    IconUrl = ub.Badge.IconUrl,
                    Category = ub.Badge.Category.ToString(),
                    Rarity = ub.Badge.Rarity.ToString(),
                    XpValue = ub.Badge.XpValue,
                    EarnedAt = ub.EarnedAt
                }).ToList(),
                CategoryStats = categoryStats,
                RankAmongFriends = await GetBadgeRankAmongFriends(userId),
                BadgeCollectorTitle = badgeCollectorTitle
            };
        }

        /// <summary>
        /// Gets recent achievements and milestones.
        /// Like reviewing your recent victory highlights!
        /// </summary>
        private async Task<List<RecentAchievementDto>> GetRecentAchievements(int userId)
        {
            var achievements = new List<RecentAchievementDto>();
            
            // Get recent badges
            var recentBadges = await _badgeRepository.GetRecentUserBadgesAsync(userId, 30);
            achievements.AddRange(recentBadges.Select(ub => new RecentAchievementDto
            {
                Type = "Badge",
                Title = ub.Badge.Name,
                Description = ub.Badge.Description,
                IconUrl = ub.Badge.IconUrl,
                AchievedAt = ub.EarnedAt,
                XpEarned = ub.Badge.XpValue,
                IsRare = ub.Badge.IsRare,
                ShareableText = $"Just earned the '{ub.Badge.Name}' badge! 🏆"
            }));

            // Get recent level ups
            var recentLevelUps = await _gamificationRepository.GetRecentLevelUpsAsync(userId, 30);
            achievements.AddRange(recentLevelUps.Select(lu => new RecentAchievementDto
            {
                Type = "LevelUp",
                Title = $"Level {lu.NewLevel} Achieved!",
                Description = $"Reached level {lu.NewLevel}",
                IconUrl = "/images/level-up.png",
                AchievedAt = lu.AchievedAt,
                XpEarned = 0,
                IsRare = lu.NewLevel % 10 == 0, // Every 10th level is rare
                ShareableText = $"Just reached level {lu.NewLevel}! 🚀"
            }));

            return achievements.OrderByDescending(a => a.AchievedAt).Take(10).ToList();
        }

        /// <summary>
        /// Gets progress toward earning badges.
        /// Like checking how close you are to your next achievement!
        /// </summary>
        private async Task<List<BadgeProgressDto>> GetBadgeProgress(int userId)
        {
            return await _badgeRepository.GetBadgeProgressAsync(userId);
        }

        /// <summary>
        /// Gets upcoming milestones and goals.
        /// Like seeing your next fitness checkpoints!
        /// </summary>
        private async Task<List<MilestoneDto>> GetUpcomingMilestones(int userId)
        {
            return await _gamificationRepository.GetUpcomingMilestonesAsync(userId);
        }

        /// <summary>
        /// Gets user's position on leaderboards.
        /// Like checking where you rank in the fitness competition!
        /// </summary>
        private async Task<LeaderboardPositionDto> GetLeaderboardPosition(int userId)
        {
            return await _leaderboardService.GetUserLeaderboardPositionAsync(userId);
        }

        /// <summary>
        /// Gets comprehensive streak information.
        /// Like tracking your consistency across all fitness activities!
        /// </summary>
        private async Task<StreakInfoDto> GetStreakInfo(int userId)
        {
            var currentStreak = await _userRepository.GetCurrentStreakAsync(userId);
            var longestStreak = await _userRepository.GetLongestStreakAsync(userId);
            var streakData = await _gamificationRepository.GetStreakDataAsync(userId);

            return new StreakInfoDto
            {
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                StreakStartDate = streakData.CurrentStreakStartDate,
                LastActivityDate = streakData.LastActivityDate,
                StreaksByType = streakData.StreaksByType.Select(s => new StreakTypeDto
                {
                    ActivityType = s.ActivityType,
                    CurrentStreak = s.CurrentStreak,
                    LongestStreak = s.LongestStreak,
                    LastActivity = s.LastActivity
                }).ToList(),
                DaysUntilStreakMilestone = CalculateDaysUntilStreakMilestone(currentStreak),
                NextStreakReward = GetNextStreakReward(currentStreak)
            };
        }

        /// <summary>
        /// Gets active challenge information.
        /// Like checking your current fitness competitions!
        /// </summary>
        private async Task<ChallengeProgressDto> GetActiveChallenges(int userId)
        {
            var activeChallenges = await _challengeRepository.GetUserActiveChallengesAsync(userId);
            var completedChallenges = await _challengeRepository.GetUserCompletedChallengesCountAsync(userId);
            var challengeWins = await _challengeRepository.GetUserChallengeWinsAsync(userId);

            return new ChallengeProgressDto
            {
                ActiveChallenges = activeChallenges.Count,
                Challenges = activeChallenges.Select(c => new ActiveChallengeDto
                {
                    ChallengeId = c.Id,
                    Name = c.Name,
                    Type = c.Type.ToString(),
                    EndDate = c.EndDate,
                    CurrentProgress = c.UserProgress?.CurrentValue ?? 0,
                    TargetProgress = c.TargetValue,
                    Unit = c.Unit,
                    ProgressPercentage = c.UserProgress?.ProgressPercentage ?? 0,
                    CurrentPosition = c.UserProgress?.Position ?? 0,
                    TotalParticipants = c.ParticipantCount,
                    RewardDescription = c.RewardDescription
                }).ToList(),
                CompletedChallenges = completedChallenges,
                ChallengeWins = challengeWins.Count,
                FavoriteChallenge = challengeWins.GroupBy(w => w.ChallengeType)
                                                .OrderByDescending(g => g.Count())
                                                .FirstOrDefault()?.Key ?? "None"
            };
        }

        /// <summary>
        /// Creates a personalized motivational message.
        /// Like having a personal trainer give you encouraging feedback!
        /// </summary>
        private string CreateMotivationalMessage(Domain.Entities.Users.User user, UserLevelInfo levelInfo, BadgeSummaryDto badges, StreakInfoDto streaks)
        {
            var messages = new List<string>();

            // Level progress motivation
            if (levelInfo.ProgressToNextLevel > 80)
            {
                messages.Add($"You're so close to level {levelInfo.CurrentLevel + 1}! Just {levelInfo.XpToNextLevel} XP to go!");
            }
            else if (levelInfo.CurrentLevel > 1)
            {
                messages.Add($"Level {levelInfo.CurrentLevel} looking strong, {user.FirstName}!");
            }

            // Streak motivation
            if (streaks.CurrentStreak >= 7)
            {
                messages.Add($"Amazing {streaks.CurrentStreak}-day streak! Keep the momentum going!");
            }
            else if (streaks.CurrentStreak > 0)
            {
                messages.Add($"Day {streaks.CurrentStreak} of your current streak - consistency is key!");
            }

            // Badge motivation
            if (badges.RecentBadges > 0)
            {
                messages.Add($"You've earned {badges.RecentBadges} new badge{(badges.RecentBadges > 1 ? "s" : "")} recently!");
            }

            // Default motivation if no specific achievements
            if (!messages.Any())
            {
                var defaultMessages = new[]
                {
                    "Every workout counts - you're building something amazing!",
                    "Your fitness journey is unique and valuable!",
                    "Progress over perfection - keep moving forward!",
                    "Today is a great day to challenge yourself!",
                    "Your dedication is inspiring!"
                };
                messages.Add(defaultMessages[new Random().Next(defaultMessages.Length)]);
            }

            return string.Join(" ", messages);
        }

        /// <summary>
        /// Helper methods for various calculations
        /// </summary>
        private string DetermineBadgeCollectorTitle(int badgeCount) => badgeCount switch
        {
            0 => "Getting Started",
            < 5 => "Badge Novice",
            < 10 => "Rising Achiever",
            < 25 => "Badge Collector",
            < 50 => "Achievement Hunter",
            < 100 => "Trophy Master",
            _ => "Legend"
        };

        private async Task<int> GetBadgeRankAmongFriends(int userId)
        {
            try
            {
                return await _gamificationRepository.GetBadgeRankAmongFriendsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get badge rank among friends for user {UserId}", userId);
                return 0;
            }
        }

        private int CalculateDaysUntilStreakMilestone(int currentStreak)
        {
            var milestones = new[] { 7, 14, 30, 60, 100, 365 };
            var nextMilestone = milestones.FirstOrDefault(m => m > currentStreak);
            return nextMilestone > 0 ? nextMilestone - currentStreak : 0;
        }

        private string GetNextStreakReward(int currentStreak)
        {
            return currentStreak switch
            {
                < 7 => "Week Warrior Badge",
                < 14 => "Fortnight Fighter Badge",
                < 30 => "Monthly Master Badge",
                < 60 => "Consistency Champion Badge",
                < 100 => "Century Achiever Badge",
                < 365 => "Year-Long Legend Badge",
                _ => "Lifetime Dedication Badge"
            };
        }
    }
}
