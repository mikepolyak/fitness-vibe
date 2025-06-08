using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FitnessVibe.Application.Commands.Gamification;
using FitnessVibe.Application.Queries.Gamification;
using System.Security.Claims;

namespace FitnessVibe.API.Controllers
{
    /// <summary>
    /// Gamification Controller - the achievement and rewards center of our fitness app.
    /// Think of this as your personal trophy room and progress tracking system where you can
    /// see all your achievements, track your XP, climb leaderboards, and get motivated by your progress!
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class GamificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GamificationController> _logger;

        public GamificationController(IMediator mediator, ILogger<GamificationController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get user's complete gamification dashboard.
        /// Like opening your personal achievement center to see all your fitness accomplishments!
        /// </summary>
        /// <param name="includeBadgeProgress">Include progress toward earning badges</param>
        /// <param name="includeRecentAchievements">Include recent achievements and milestones</param>
        /// <param name="includeLeaderboardPosition">Include current leaderboard rankings</param>
        /// <param name="includeNextMilestones">Include upcoming milestones and goals</param>
        /// <returns>Complete gamification data with XP, badges, streaks, and achievements</returns>
        /// <response code="200">Gamification dashboard retrieved successfully</response>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(UserGamificationResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserGamificationResponse>> GetGamificationDashboard(
            [FromQuery] bool includeBadgeProgress = true,
            [FromQuery] bool includeRecentAchievements = true,
            [FromQuery] bool includeLeaderboardPosition = true,
            [FromQuery] bool includeNextMilestones = true)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Getting gamification dashboard for user {UserId}", userId);

                var query = new GetUserGamificationQuery
                {
                    UserId = userId,
                    IncludeBadgeProgress = includeBadgeProgress,
                    IncludeRecentAchievements = includeRecentAchievements,
                    IncludeLeaderboardPosition = includeLeaderboardPosition,
                    IncludeNextMilestones = includeNextMilestones
                };

                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get gamification dashboard for user {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Gamification Dashboard Failed",
                    Detail = "Unable to retrieve your achievement data",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get leaderboards for competitive fitness tracking.
        /// Like checking the scoreboard to see how you rank against other fitness enthusiasts!
        /// </summary>
        /// <param name="leaderboardType">Type of leaderboard (Overall, Weekly, Monthly, Friends, Local)</param>
        /// <param name="metricType">Metric to rank by (XP, Activities, Distance, Calories, Streaks)</param>
        /// <param name="activityType">Filter by specific activity type</param>
        /// <param name="timeFrame">Time frame for rankings (AllTime, ThisWeek, ThisMonth, Today)</param>
        /// <param name="location">Location filter for local leaderboards</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of entries per page</param>
        /// <returns>Leaderboard data with rankings and competitive information</returns>
        /// <response code="200">Leaderboard retrieved successfully</response>
        [HttpGet("leaderboard")]
        [ProducesResponseType(typeof(LeaderboardResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<LeaderboardResponse>> GetLeaderboard(
            [FromQuery] string leaderboardType = "Overall",
            [FromQuery] string metricType = "XP",
            [FromQuery] string? activityType = null,
            [FromQuery] string timeFrame = "AllTime",
            [FromQuery] string? location = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Getting leaderboard for user {UserId}, type: {LeaderboardType}", userId, leaderboardType);

                var query = new GetLeaderboardQuery
                {
                    UserId = userId,
                    LeaderboardType = leaderboardType,
                    MetricType = metricType,
                    ActivityType = activityType,
                    TimeFrame = timeFrame,
                    Location = location,
                    Page = page,
                    PageSize = Math.Min(pageSize, 100), // Cap at 100 entries per page
                    IncludeUserPosition = true,
                    IncludeNearbyRanks = true
                };

                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get leaderboard for user {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Leaderboard Failed",
                    Detail = "Unable to retrieve leaderboard data",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Award experience points to the current user.
        /// Like giving yourself bonus points for completing a special fitness challenge!
        /// </summary>
        /// <param name="command">XP award details</param>
        /// <returns>XP award results including any level-ups or badges earned</returns>
        /// <response code="200">XP awarded successfully</response>
        /// <response code="400">Invalid XP award data</response>
        [HttpPost("award-xp")]
        [ProducesResponseType(typeof(AwardXpResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AwardXpResponse>> AwardXp([FromBody] AwardXpCommand command)
        {
            try
            {
                command.UserId = GetCurrentUserId();
                
                _logger.LogInformation("Awarding {XpAmount} XP to user {UserId} for: {Reason}", 
                    command.XpAmount, command.UserId, command.Reason);

                var result = await _mediator.Send(command);

                _logger.LogInformation("Successfully awarded {TotalXp} XP to user {UserId}", 
                    result.TotalXpAwarded, command.UserId);

                return Ok(result);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User Not Found",
                    Detail = "User account not found",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to award XP to user {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Award XP Failed",
                    Detail = "Unable to award experience points",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Award a badge to the current user.
        /// Like earning a trophy for a special fitness achievement!
        /// </summary>
        /// <param name="command">Badge award details</param>
        /// <returns>Badge award results with celebration details</returns>
        /// <response code="200">Badge awarded successfully</response>
        /// <response code="400">Invalid badge award data</response>
        /// <response code="409">Badge already earned</response>
        [HttpPost("award-badge")]
        [ProducesResponseType(typeof(AwardBadgeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AwardBadgeResponse>> AwardBadge([FromBody] AwardBadgeCommand command)
        {
            try
            {
                command.UserId = GetCurrentUserId();
                
                _logger.LogInformation("Awarding badge '{BadgeCode}' to user {UserId} for: {Reason}", 
                    command.BadgeCode, command.UserId, command.Reason);

                var result = await _mediator.Send(command);

                _logger.LogInformation("Successfully awarded badge '{BadgeCode}' to user {UserId}", 
                    command.BadgeCode, command.UserId);

                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already earned"))
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Badge Already Earned",
                    Detail = ex.Message,
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Badge Not Found",
                    Detail = $"Badge with code '{command.BadgeCode}' not found",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to award badge '{BadgeCode}' to user {UserId}", command.BadgeCode, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Award Badge Failed",
                    Detail = "Unable to award badge",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get user's badge collection and progress.
        /// Like browsing through your trophy case to see all your achievements!
        /// </summary>
        /// <param name="category">Filter badges by category</param>
        /// <param name="includeProgress">Include progress toward unearned badges</param>
        /// <returns>Complete badge collection and progress data</returns>
        /// <response code="200">Badge collection retrieved successfully</response>
        [HttpGet("badges")]
        [ProducesResponseType(typeof(BadgeSummaryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<BadgeSummaryDto>> GetBadgeCollection(
            [FromQuery] string? category = null,
            [FromQuery] bool includeProgress = true)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Getting badge collection for user {UserId}", userId);

                // This would typically be a separate GetUserBadgesQuery
                var query = new GetUserGamificationQuery
                {
                    UserId = userId,
                    IncludeBadgeProgress = includeProgress,
                    IncludeRecentAchievements = false,
                    IncludeLeaderboardPosition = false,
                    IncludeNextMilestones = false
                };

                var result = await _mediator.Send(query);

                return Ok(result.Badges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get badge collection for user {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Get Badge Collection Failed",
                    Detail = "Unable to retrieve badge collection",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get user's current streak information.
        /// Like checking your consistency tracking and workout momentum!
        /// </summary>
        /// <returns>Comprehensive streak data across all activities</returns>
        /// <response code="200">Streak information retrieved successfully</response>
        [HttpGet("streaks")]
        [ProducesResponseType(typeof(StreakInfoDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<StreakInfoDto>> GetStreakInfo()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Getting streak info for user {UserId}", userId);

                var query = new GetUserGamificationQuery
                {
                    UserId = userId,
                    IncludeBadgeProgress = false,
                    IncludeRecentAchievements = false,
                    IncludeLeaderboardPosition = false,
                    IncludeNextMilestones = false
                };

                var result = await _mediator.Send(query);

                return Ok(result.Streaks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get streak info for user {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Get Streak Info Failed",
                    Detail = "Unable to retrieve streak information",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get user's recent achievements and milestones.
        /// Like reviewing your recent victory highlights!
        /// </summary>
        /// <param name="days">Number of days to look back for achievements</param>
        /// <param name="limit">Maximum number of achievements to return</param>
        /// <returns>Recent achievements and milestone completions</returns>
        /// <response code="200">Recent achievements retrieved successfully</response>
        [HttpGet("achievements")]
        [ProducesResponseType(typeof(List<RecentAchievementDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RecentAchievementDto>>> GetRecentAchievements(
            [FromQuery] int days = 30,
            [FromQuery] int limit = 20)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Getting recent achievements for user {UserId}", userId);

                var query = new GetUserGamificationQuery
                {
                    UserId = userId,
                    IncludeBadgeProgress = false,
                    IncludeRecentAchievements = true,
                    IncludeLeaderboardPosition = false,
                    IncludeNextMilestones = false
                };

                var result = await _mediator.Send(query);

                // Filter and limit achievements based on parameters
                var filteredAchievements = result.RecentAchievements
                    .Where(a => a.AchievedAt > DateTime.UtcNow.AddDays(-days))
                    .Take(limit)
                    .ToList();

                return Ok(filteredAchievements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get recent achievements for user {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Get Recent Achievements Failed",
                    Detail = "Unable to retrieve recent achievements",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get upcoming milestones and goals.
        /// Like checking your next fitness checkpoints and rewards!
        /// </summary>
        /// <param name="priority">Filter by priority level (1 = highest)</param>
        /// <returns>Upcoming milestones and achievement opportunities</returns>
        /// <response code="200">Upcoming milestones retrieved successfully</response>
        [HttpGet("milestones")]
        [ProducesResponseType(typeof(List<MilestoneDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<MilestoneDto>>> GetUpcomingMilestones(
            [FromQuery] int? priority = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Getting upcoming milestones for user {UserId}", userId);

                var query = new GetUserGamificationQuery
                {
                    UserId = userId,
                    IncludeBadgeProgress = false,
                    IncludeRecentAchievements = false,
                    IncludeLeaderboardPosition = false,
                    IncludeNextMilestones = true
                };

                var result = await _mediator.Send(query);

                var milestones = result.UpcomingMilestones;
                
                if (priority.HasValue)
                {
                    milestones = milestones.Where(m => m.Priority <= priority.Value).ToList();
                }

                return Ok(milestones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get upcoming milestones for user {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Get Upcoming Milestones Failed",
                    Detail = "Unable to retrieve upcoming milestones",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get current user ID from JWT claims.
        /// Like reading your member ID from your gym access card.
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }
    }
}
