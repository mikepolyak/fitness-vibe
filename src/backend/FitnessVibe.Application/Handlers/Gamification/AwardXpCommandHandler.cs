using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Commands.Gamification;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Gamification
{
    /// <summary>
    /// Handler for awarding experience points - the reward system coordinator.
    /// This is like having a game master who calculates your points, checks for level-ups,
    /// and ensures you get all the recognition and rewards you've earned for your fitness efforts.
    /// </summary>
    public class AwardXpCommandHandler : IRequestHandler<AwardXpCommand, AwardXpResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IGamificationRepository _gamificationRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly ILevelService _levelService;
        private readonly IBadgeService _badgeService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AwardXpCommandHandler> _logger;

        public AwardXpCommandHandler(
            IUserRepository userRepository,
            IGamificationRepository gamificationRepository,
            IBadgeRepository badgeRepository,
            ILevelService levelService,
            IBadgeService badgeService,
            INotificationService notificationService,
            ILogger<AwardXpCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _gamificationRepository = gamificationRepository ?? throw new ArgumentNullException(nameof(gamificationRepository));
            _badgeRepository = badgeRepository ?? throw new ArgumentNullException(nameof(badgeRepository));
            _levelService = levelService ?? throw new ArgumentNullException(nameof(levelService));
            _badgeService = badgeService ?? throw new ArgumentNullException(nameof(badgeService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AwardXpResponse> Handle(AwardXpCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Awarding {XpAmount} XP to user {UserId} for: {Reason}", 
                request.XpAmount, request.UserId, request.Reason);

            // Get the user
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {request.UserId} not found");
            }

            // Calculate total XP with any bonuses
            var baseXp = request.XpAmount;
            var bonusXp = await CalculateBonusXp(user, baseXp, request.MultiplierPercentage, request.Source);
            var totalXpAwarded = baseXp + bonusXp;

            // Store current level for comparison
            var currentLevel = user.Level;
            var currentXp = user.ExperiencePoints;

            // Award the XP to the user
            user.AwardExperiencePoints(totalXpAwarded, request.Reason);

            // Check for level up
            var levelUpResult = await _levelService.CheckAndProcessLevelUpAsync(user.Id, user.ExperiencePoints);

            // Update user in database
            await _userRepository.UpdateAsync(user);

            // Record XP transaction
            await RecordXpTransaction(request, totalXpAwarded, bonusXp);

            // Check for XP-based badge triggers
            var badgesEarned = await CheckXpBadgeTriggers(user.Id, currentXp, user.ExperiencePoints);

            // Generate celebration message
            var celebrationMessage = CreateCelebrationMessage(user, totalXpAwarded, levelUpResult.LeveledUp, request.Reason);

            // Send notifications if requested
            if (request.NotifyUser)
            {
                await SendXpNotification(user, totalXpAwarded, levelUpResult, badgesEarned);
            }

            // Calculate XP to next level
            var xpToNextLevel = await _levelService.CalculateXpToNextLevelAsync(user.Level, user.ExperiencePoints);

            _logger.LogInformation("Successfully awarded {TotalXp} XP to user {UserId} (Base: {BaseXp}, Bonus: {BonusXp})", 
                totalXpAwarded, request.UserId, baseXp, bonusXp);

            return new AwardXpResponse
            {
                XpAwarded = baseXp,
                BonusXp = bonusXp,
                TotalXpAwarded = totalXpAwarded,
                NewTotalXp = user.ExperiencePoints,
                CurrentLevel = user.Level,
                LeveledUp = levelUpResult.LeveledUp,
                NewLevel = levelUpResult.LeveledUp ? user.Level : null,
                NewLevelTitle = levelUpResult.NewLevelTitle,
                UnlockedFeatures = levelUpResult.UnlockedFeatures.ToList(),
                BadgesEarned = badgesEarned.Select(b => new BadgeEarnedInfo
                {
                    BadgeId = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    IconUrl = b.IconUrl,
                    Rarity = b.Rarity.ToString(),
                    XpValue = b.XpValue,
                    IsFirstTime = true
                }).ToList(),
                CelebrationMessage = celebrationMessage,
                XpToNextLevel = xpToNextLevel
            };
        }

        /// <summary>
        /// Calculates bonus XP based on multipliers, streaks, and special conditions.
        /// Like having a personal trainer calculate bonus points for extra effort!
        /// </summary>
        private async Task<int> CalculateBonusXp(Domain.Entities.Users.User user, int baseXp, int? multiplierPercentage, string source)
        {
            var bonusXp = 0;

            // Apply multiplier if provided
            if (multiplierPercentage.HasValue && multiplierPercentage.Value > 100)
            {
                var multiplierBonus = (multiplierPercentage.Value - 100) / 100.0;
                bonusXp += (int)(baseXp * multiplierBonus);
            }

            // Weekend bonus (50% more XP on weekends)
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                bonusXp += (int)(baseXp * 0.5);
            }

            // Streak bonus
            var currentStreak = await _userRepository.GetCurrentStreakAsync(user.Id);
            if (currentStreak >= 7) // Week+ streak
            {
                var streakMultiplier = Math.Min(currentStreak / 7 * 0.1, 1.0); // Up to 100% bonus
                bonusXp += (int)(baseXp * streakMultiplier);
            }

            // Early bird bonus (before 8 AM)
            if (DateTime.Now.Hour < 8)
            {
                bonusXp += (int)(baseXp * 0.25); // 25% bonus
            }

            // New user bonus (first 30 days)
            if (user.CreatedAt > DateTime.UtcNow.AddDays(-30))
            {
                bonusXp += (int)(baseXp * 0.2); // 20% bonus for new users
            }

            return bonusXp;
        }

        /// <summary>
        /// Records the XP transaction for audit and analytics.
        /// Like keeping a detailed log of all your achievement points!
        /// </summary>
        private async Task RecordXpTransaction(AwardXpCommand request, int totalXpAwarded, int bonusXp)
        {
            try
            {
                var transaction = new XpTransaction
                {
                    UserId = request.UserId,
                    Amount = totalXpAwarded,
                    BaseAmount = request.XpAmount,
                    BonusAmount = bonusXp,
                    Reason = request.Reason,
                    Source = request.Source,
                    SourceId = request.SourceId,
                    Metadata = request.Metadata,
                    CreatedAt = DateTime.UtcNow
                };

                await _gamificationRepository.AddXpTransactionAsync(transaction);
                await _gamificationRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record XP transaction for user {UserId}", request.UserId);
                // Don't fail the XP award if logging fails
            }
        }

        /// <summary>
        /// Checks for badges that should be awarded based on XP milestones.
        /// Like checking if your achievement points unlock any special rewards!
        /// </summary>
        private async Task<List<Domain.Entities.Gamification.Badge>> CheckXpBadgeTriggers(int userId, int previousXp, int newXp)
        {
            try
            {
                return await _badgeService.CheckXpMilestoneBadgesAsync(userId, previousXp, newXp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check XP badge triggers for user {UserId}", userId);
                return new List<Domain.Entities.Gamification.Badge>();
            }
        }

        /// <summary>
        /// Creates an enthusiastic celebration message for the XP award.
        /// Like having a cheerleader celebrate your fitness achievements!
        /// </summary>
        private string CreateCelebrationMessage(Domain.Entities.Users.User user, int xpAwarded, bool leveledUp, string reason)
        {
            var messages = new List<string>();

            if (leveledUp)
            {
                messages.Add($"🎉 LEVEL UP! Welcome to level {user.Level}, {user.FirstName}!");
            }

            messages.Add($"💪 +{xpAwarded} XP earned for {reason.ToLower()}!");

            var motivationalPhrases = new[]
            {
                "Keep crushing those goals!",
                "You're on fire!",
                "Unstoppable dedication!",
                "Making serious progress!",
                "That's the spirit!",
                "Excellence in action!",
                "Your hard work is paying off!",
                "Building momentum!"
            };

            var random = new Random();
            messages.Add(motivationalPhrases[random.Next(motivationalPhrases.Length)]);

            return string.Join(" ", messages);
        }

        /// <summary>
        /// Sends notification about XP award to the user.
        /// Like getting a congratulatory message on your phone for your achievements!
        /// </summary>
        private async Task SendXpNotification(Domain.Entities.Users.User user, int xpAwarded, dynamic levelUpResult, List<Domain.Entities.Gamification.Badge> badgesEarned)
        {
            try
            {
                var notificationData = new
                {
                    Type = "XpAwarded",
                    XpAmount = xpAwarded,
                    NewTotalXp = user.ExperiencePoints,
                    LeveledUp = levelUpResult.LeveledUp,
                    NewLevel = levelUpResult.LeveledUp ? user.Level : (int?)null,
                    BadgesEarned = badgesEarned.Select(b => b.Name).ToList()
                };

                await _notificationService.SendNotificationAsync(
                    userId: user.Id,
                    title: levelUpResult.LeveledUp ? "Level Up!" : "XP Earned!",
                    message: $"You earned {xpAwarded} XP!" + (levelUpResult.LeveledUp ? $" Welcome to level {user.Level}!" : ""),
                    type: "Gamification",
                    data: notificationData
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send XP notification to user {UserId}", user.Id);
                // Don't fail XP award if notification fails
            }
        }
    }

    /// <summary>
    /// Represents an XP transaction record.
    /// </summary>
    public class XpTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Amount { get; set; }
        public int BaseAmount { get; set; }
        public int BonusAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public int? SourceId { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
