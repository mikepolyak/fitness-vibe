using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FitnessVibe.Application.Commands.Users;
using FitnessVibe.Application.Queries.Users;
using System.Security.Claims;

namespace FitnessVibe.API.Controllers
{
    /// <summary>
    /// User Profile Controller - the personal command center of each fitness journey.
    /// Think of this as your personal trainer's office where you set goals, track progress,
    /// customize your experience, and manage your fitness identity. This is where the app
    /// learns who you are and what motivates you to keep moving forward.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    [Authorize]
    [Produces("application/json")]
    public class UserProfileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IMediator mediator, ILogger<UserProfileController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get the current user's complete profile.
        /// Like looking in the mirror to see your current fitness identity and progress.
        /// </summary>
        /// <returns>Complete user profile</returns>
        /// <response code="200">Profile retrieved successfully</response>
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserProfileDetailResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserProfileDetailResponse>> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetUserProfileDetailQuery { UserId = userId };

                _logger.LogDebug("Getting detailed profile for user: {UserId}", userId);

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get profile for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Profile",
                    Detail = "Unable to retrieve your profile information.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Update basic profile information.
        /// Like updating your gym membership card with new personal details.
        /// </summary>
        /// <param name="command">Profile update data</param>
        /// <returns>Updated profile information</returns>
        /// <response code="200">Profile updated successfully</response>
        /// <response code="400">Invalid profile data</response>
        [HttpPut("me")]
        [ProducesResponseType(typeof(UpdateProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UpdateProfileResponse>> UpdateProfile([FromBody] UpdateUserProfileCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogInformation("Updating profile for user: {UserId}", userId);

                var result = await _mediator.Send(command);

                _logger.LogInformation("Profile updated successfully for user: {UserId}", userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update profile for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Profile Update Failed",
                    Detail = "Unable to update your profile. Please check your data and try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Update fitness profile and goals.
        /// Like working with a personal trainer to adjust your fitness plan and targets.
        /// </summary>
        /// <param name="command">Fitness profile data</param>
        /// <returns>Updated fitness profile</returns>
        /// <response code="200">Fitness profile updated successfully</response>
        /// <response code="400">Invalid fitness data</response>
        [HttpPut("me/fitness")]
        [ProducesResponseType(typeof(UpdateFitnessProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UpdateFitnessProfileResponse>> UpdateFitnessProfile([FromBody] UpdateFitnessProfileCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogInformation("Updating fitness profile for user: {UserId}", userId);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update fitness profile for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Fitness Profile Update Failed",
                    Detail = "Unable to update your fitness profile. Please check your data and try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Upload and set profile avatar image.
        /// Like updating your photo on your gym membership card.
        /// </summary>
        /// <param name="command">Avatar upload data</param>
        /// <returns>New avatar URL</returns>
        /// <response code="200">Avatar updated successfully</response>
        /// <response code="400">Invalid image data</response>
        [HttpPost("me/avatar")]
        [ProducesResponseType(typeof(UploadAvatarResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UploadAvatarResponse>> UploadAvatar([FromBody] UploadAvatarCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogInformation("Uploading avatar for user: {UserId}", userId);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload avatar for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Avatar Upload Failed",
                    Detail = "Unable to upload your avatar image. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get user preferences and app settings.
        /// Like checking your personalized app configuration and notification settings.
        /// </summary>
        /// <returns>User preferences</returns>
        /// <response code="200">Preferences retrieved successfully</response>
        [HttpGet("me/preferences")]
        [ProducesResponseType(typeof(UserPreferencesResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserPreferencesResponse>> GetPreferences()
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetUserPreferencesQuery { UserId = userId };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get preferences for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Preferences",
                    Detail = "Unable to retrieve your preferences.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Update user preferences and app settings.
        /// Like customizing your gym experience - music volume, equipment preferences, etc.
        /// </summary>
        /// <param name="command">Preference updates</param>
        /// <returns>Updated preferences</returns>
        /// <response code="200">Preferences updated successfully</response>
        [HttpPut("me/preferences")]
        [ProducesResponseType(typeof(UpdatePreferencesResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UpdatePreferencesResponse>> UpdatePreferences([FromBody] UpdateUserPreferencesCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update preferences for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Update Preferences",
                    Detail = "Unable to update your preferences.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get user's fitness analytics and insights.
        /// Like getting a comprehensive fitness report card from your personal trainer.
        /// </summary>
        /// <param name="timeframe">Analysis timeframe (week, month, quarter, year)</param>
        /// <returns>Fitness analytics and insights</returns>
        /// <response code="200">Analytics retrieved successfully</response>
        [HttpGet("me/analytics")]
        [ProducesResponseType(typeof(UserAnalyticsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserAnalyticsResponse>> GetAnalytics([FromQuery] string timeframe = "month")
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetUserAnalyticsQuery 
                { 
                    UserId = userId,
                    Timeframe = timeframe
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get analytics for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Analytics",
                    Detail = "Unable to retrieve your fitness analytics.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get user's personal records and milestones.
        /// Like looking at your trophy case to see all your personal best achievements.
        /// </summary>
        /// <returns>Personal records and milestones</returns>
        /// <response code="200">Personal records retrieved successfully</response>
        [HttpGet("me/records")]
        [ProducesResponseType(typeof(PersonalRecordsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<PersonalRecordsResponse>> GetPersonalRecords()
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetPersonalRecordsQuery { UserId = userId };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get personal records for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Personal Records",
                    Detail = "Unable to retrieve your personal records.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Export user data for external analysis or backup.
        /// Like getting a complete copy of your fitness journal for your own records.
        /// </summary>
        /// <param name="format">Export format (json, csv, pdf)</param>
        /// <param name="includePersonalData">Include personal information</param>
        /// <returns>Export file or download link</returns>
        /// <response code="200">Export generated successfully</response>
        [HttpPost("me/export")]
        [ProducesResponseType(typeof(ExportDataResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ExportDataResponse>> ExportUserData(
            [FromQuery] string format = "json",
            [FromQuery] bool includePersonalData = true)
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new ExportUserDataCommand 
                { 
                    UserId = userId,
                    Format = format,
                    IncludePersonalData = includePersonalData
                };

                _logger.LogInformation("Exporting data for user: {UserId}, format: {Format}", userId, format);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export data for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Data Export Failed",
                    Detail = "Unable to export your data. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Change user password.
        /// Like updating your gym locker combination for better security.
        /// </summary>
        /// <param name="command">Password change request</param>
        /// <returns>Password change confirmation</returns>
        /// <response code="200">Password changed successfully</response>
        /// <response code="400">Invalid password data</response>
        /// <response code="401">Current password incorrect</response>
        [HttpPost("me/change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogInformation("Password change attempt for user: {UserId}", userId);

                await _mediator.Send(command);

                _logger.LogInformation("Password changed successfully for user: {UserId}", userId);

                return Ok(new { message = "Password changed successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Password change failed - incorrect current password for user: {UserId}", GetCurrentUserId());
                return Unauthorized(new ProblemDetails
                {
                    Title = "Incorrect Current Password",
                    Detail = "The current password you provided is incorrect.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password change failed for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Password Change Failed",
                    Detail = "Unable to change your password. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get privacy settings and data sharing preferences.
        /// Like checking what information you're comfortable sharing with the gym community.
        /// </summary>
        /// <returns>Privacy settings</returns>
        /// <response code="200">Privacy settings retrieved successfully</response>
        [HttpGet("me/privacy")]
        [ProducesResponseType(typeof(PrivacySettingsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<PrivacySettingsResponse>> GetPrivacySettings()
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetPrivacySettingsQuery { UserId = userId };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get privacy settings for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Privacy Settings",
                    Detail = "Unable to retrieve your privacy settings.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Update privacy settings and data sharing preferences.
        /// Like setting boundaries for what fitness information you want to keep private.
        /// </summary>
        /// <param name="command">Privacy settings update</param>
        /// <returns>Updated privacy settings</returns>
        /// <response code="200">Privacy settings updated successfully</response>
        [HttpPut("me/privacy")]
        [ProducesResponseType(typeof(UpdatePrivacySettingsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UpdatePrivacySettingsResponse>> UpdatePrivacySettings([FromBody] UpdatePrivacySettingsCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update privacy settings for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Update Privacy Settings",
                    Detail = "Unable to update your privacy settings.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Delete user account and all associated data.
        /// Like canceling your gym membership and clearing out your locker.
        /// </summary>
        /// <param name="command">Account deletion confirmation</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Account deletion initiated</response>
        /// <response code="400">Invalid deletion request</response>
        [HttpDelete("me")]
        [ProducesResponseType(typeof(DeleteAccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DeleteAccountResponse>> DeleteAccount([FromBody] DeleteAccountCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogWarning("Account deletion initiated for user: {UserId}", userId);

                var result = await _mediator.Send(command);

                _logger.LogWarning("Account deletion processed for user: {UserId}", userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Account deletion failed for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Account Deletion Failed",
                    Detail = "Unable to process account deletion. Please contact support.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get a public user profile (for social features).
        /// Like viewing someone's public gym profile card.
        /// </summary>
        /// <param name="userId">User ID to view</param>
        /// <returns>Public profile information</returns>
        /// <response code="200">Public profile retrieved successfully</response>
        /// <response code="404">User not found</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(PublicUserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PublicUserProfileResponse>> GetPublicProfile(int userId)
        {
            try
            {
                var viewerUserId = GetCurrentUserId();
                var query = new GetPublicUserProfileQuery 
                { 
                    UserId = userId,
                    ViewerUserId = viewerUserId
                };

                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "User Not Found",
                        Detail = "The specified user could not be found.",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get public profile for user: {UserId}", userId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Public Profile",
                    Detail = "Unable to retrieve user profile.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get current user ID from JWT claims.
        /// Like reading the member ID from the gym access card.
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

    // Request/Response DTOs for User Profile

    public class UserProfileDetailResponse
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
        public int XpForNextLevel { get; set; }
        public double LevelProgress { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime LastActiveDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserStatsResponse Stats { get; set; } = new();
        public List<UserGoalResponse> ActiveGoals { get; set; } = new();
        public List<RecentAchievementResponse> RecentAchievements { get; set; } = new();
    }

    public class UserStatsResponse
    {
        public int TotalActivities { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public double TotalDistance { get; set; }
        public int TotalCalories { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int BadgesEarned { get; set; }
        public int ChallengesCompleted { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
    }

    public class UserGoalResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public double CurrentValue { get; set; }
        public double ProgressPercentage { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateProfileResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateFitnessProfileResponse
    {
        public int UserId { get; set; }
        public string FitnessLevel { get; set; } = string.Empty;
        public string PrimaryGoal { get; set; } = string.Empty;
        public List<string> FavoriteActivities { get; set; } = new();
        public UserMetricsResponse? Metrics { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UserMetricsResponse
    {
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public string? HeightUnit { get; set; }
        public string? WeightUnit { get; set; }
        public int? RestingHeartRate { get; set; }
        public int? MaxHeartRate { get; set; }
    }

    public class UploadAvatarResponse
    {
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string FileSize { get; set; } = string.Empty;
    }

    public class UserPreferencesResponse
    {
        public NotificationPreferencesResponse Notifications { get; set; } = new();
        public PrivacyPreferencesResponse Privacy { get; set; } = new();
        public AppPreferencesResponse App { get; set; } = new();
        public WorkoutPreferencesResponse Workout { get; set; } = new();
    }

    public class NotificationPreferencesResponse
    {
        public bool PushNotifications { get; set; }
        public bool EmailNotifications { get; set; }
        public bool WorkoutReminders { get; set; }
        public bool SocialNotifications { get; set; }
        public bool ChallengeNotifications { get; set; }
        public bool AchievementNotifications { get; set; }
        public TimeSpan? QuietHoursStart { get; set; }
        public TimeSpan? QuietHoursEnd { get; set; }
    }

    public class PrivacyPreferencesResponse
    {
        public string ProfileVisibility { get; set; } = string.Empty; // Public, Friends, Private
        public bool ShowActivityFeed { get; set; }
        public bool ShowPersonalRecords { get; set; }
        public bool ShowLocation { get; set; }
        public bool AllowChallengeInvites { get; set; }
        public bool ShowInLeaderboards { get; set; }
    }

    public class AppPreferencesResponse
    {
        public string Theme { get; set; } = string.Empty; // Light, Dark, Auto
        public string Language { get; set; } = string.Empty;
        public string DistanceUnit { get; set; } = string.Empty; // Metric, Imperial
        public string WeightUnit { get; set; } = string.Empty; // Kg, Lbs
        public bool AutoPauseWorkouts { get; set; }
        public bool ShowMapRoutes { get; set; }
    }

    public class WorkoutPreferencesResponse
    {
        public List<string> FavoriteActivities { get; set; } = new();
        public int DefaultWorkoutDuration { get; set; }
        public bool AutoTrackWorkouts { get; set; }
        public bool CountdownBeforeStart { get; set; }
        public bool VoiceCoaching { get; set; }
        public string PreferredWorkoutTime { get; set; } = string.Empty;
    }

    public class UpdatePreferencesResponse
    {
        public int UserId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class UserAnalyticsResponse
    {
        public string Timeframe { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AnalyticsSummaryResponse Summary { get; set; } = new();
        public List<ActivityTrendResponse> ActivityTrends { get; set; } = new();
        public List<GoalProgressResponse> GoalProgress { get; set; } = new();
        public InsightsResponse Insights { get; set; } = new();
        public List<ComparisonResponse> Comparisons { get; set; } = new();
    }

    public class AnalyticsSummaryResponse
    {
        public int TotalWorkouts { get; set; }
        public TimeSpan TotalTime { get; set; }
        public double TotalDistance { get; set; }
        public int TotalCalories { get; set; }
        public double AverageWorkoutDuration { get; set; }
        public int ActiveDays { get; set; }
        public double ConsistencyPercentage { get; set; }
    }

    public class ActivityTrendResponse
    {
        public DateTime Date { get; set; }
        public int WorkoutCount { get; set; }
        public TimeSpan Duration { get; set; }
        public double Distance { get; set; }
        public int Calories { get; set; }
    }

    public class GoalProgressResponse
    {
        public int GoalId { get; set; }
        public string GoalName { get; set; } = string.Empty;
        public double Progress { get; set; }
        public double Target { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ExpectedCompletion { get; set; }
    }

    public class InsightsResponse
    {
        public string MostActiveDay { get; set; } = string.Empty;
        public string FavoriteActivity { get; set; } = string.Empty;
        public string BestPerformanceTime { get; set; } = string.Empty;
        public double ImprovementPercentage { get; set; }
        public List<string> Recommendations { get; set; } = new();
        public List<string> Achievements { get; set; } = new();
    }

    public class ComparisonResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double ThisPeriod { get; set; }
        public double PreviousPeriod { get; set; }
        public double ChangePercentage { get; set; }
        public string Trend { get; set; } = string.Empty; // Improving, Declining, Stable
    }

    public class ExportDataResponse
    {
        public string ExportId { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string FileSize { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }

    public class PrivacySettingsResponse
    {
        public string ProfileVisibility { get; set; } = string.Empty;
        public bool ShowActivityHistory { get; set; }
        public bool ShowPersonalRecords { get; set; }
        public bool ShowLocation { get; set; }
        public bool ShowInLeaderboards { get; set; }
        public bool AllowDirectMessages { get; set; }
        public bool AllowChallengeInvites { get; set; }
        public bool ShareDataWithPartners { get; set; }
        public List<string> BlockedUsers { get; set; } = new();
    }

    public class UpdatePrivacySettingsResponse
    {
        public int UserId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> UpdatedSettings { get; set; } = new();
    }

    public class DeleteAccountResponse
    {
        public int UserId { get; set; }
        public DateTime DeletionScheduledAt { get; set; }
        public DateTime PermanentDeletionDate { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> DataRetentionNotices { get; set; } = new();
    }

    public class PublicUserProfileResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int Level { get; set; }
        public string FitnessLevel { get; set; } = string.Empty;
        public string PrimaryGoal { get; set; } = string.Empty;
        public DateTime JoinedDate { get; set; }
        public PublicStatsResponse? Stats { get; set; }
        public List<UserBadgeResponse>? RecentBadges { get; set; }
        public List<ActivitySummaryResponse>? RecentActivities { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsFollowedBy { get; set; }
        public bool CanSendMessage { get; set; }
        public bool CanChallenge { get; set; }
    }

    public class PublicStatsResponse
    {
        public int TotalActivities { get; set; }
        public int BadgesEarned { get; set; }
        public int ChallengesCompleted { get; set; }
        public int CurrentStreak { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        // Only show aggregated data, no specific metrics for privacy
    }

    // Placeholder command and query classes (to be implemented in Application layer)
    public class GetUserProfileDetailQuery : IRequest<UserProfileDetailResponse>
    {
        public int UserId { get; set; }
    }

    public class UpdateUserProfileCommand : IRequest<UpdateProfileResponse>
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
    }

    public class UpdateFitnessProfileCommand : IRequest<UpdateFitnessProfileResponse>
    {
        public int UserId { get; set; }
        public string FitnessLevel { get; set; } = string.Empty;
        public string PrimaryGoal { get; set; } = string.Empty;
        public List<string> FavoriteActivities { get; set; } = new();
        public UserMetricsRequest? Metrics { get; set; }
    }

    public class UserMetricsRequest
    {
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public string? HeightUnit { get; set; }
        public string? WeightUnit { get; set; }
        public int? RestingHeartRate { get; set; }
        public int? MaxHeartRate { get; set; }
    }

    public class UploadAvatarCommand : IRequest<UploadAvatarResponse>
    {
        public int UserId { get; set; }
        public string ImageBase64 { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }

    public class GetUserPreferencesQuery : IRequest<UserPreferencesResponse>
    {
        public int UserId { get; set; }
    }

    public class UpdateUserPreferencesCommand : IRequest<UpdatePreferencesResponse>
    {
        public int UserId { get; set; }
        public NotificationPreferencesRequest? Notifications { get; set; }
        public PrivacyPreferencesRequest? Privacy { get; set; }
        public AppPreferencesRequest? App { get; set; }
        public WorkoutPreferencesRequest? Workout { get; set; }
    }

    public class NotificationPreferencesRequest
    {
        public bool? PushNotifications { get; set; }
        public bool? EmailNotifications { get; set; }
        public bool? WorkoutReminders { get; set; }
        public bool? SocialNotifications { get; set; }
        public bool? ChallengeNotifications { get; set; }
        public bool? AchievementNotifications { get; set; }
        public TimeSpan? QuietHoursStart { get; set; }
        public TimeSpan? QuietHoursEnd { get; set; }
    }

    public class PrivacyPreferencesRequest
    {
        public string? ProfileVisibility { get; set; }
        public bool? ShowActivityFeed { get; set; }
        public bool? ShowPersonalRecords { get; set; }
        public bool? ShowLocation { get; set; }
        public bool? AllowChallengeInvites { get; set; }
        public bool? ShowInLeaderboards { get; set; }
    }

    public class AppPreferencesRequest
    {
        public string? Theme { get; set; }
        public string? Language { get; set; }
        public string? DistanceUnit { get; set; }
        public string? WeightUnit { get; set; }
        public bool? AutoPauseWorkouts { get; set; }
        public bool? ShowMapRoutes { get; set; }
    }

    public class WorkoutPreferencesRequest
    {
        public List<string>? FavoriteActivities { get; set; }
        public int? DefaultWorkoutDuration { get; set; }
        public bool? AutoTrackWorkouts { get; set; }
        public bool? CountdownBeforeStart { get; set; }
        public bool? VoiceCoaching { get; set; }
        public string? PreferredWorkoutTime { get; set; }
    }

    public class GetUserAnalyticsQuery : IRequest<UserAnalyticsResponse>
    {
        public int UserId { get; set; }
        public string Timeframe { get; set; } = "month";
    }

    public class GetPersonalRecordsQuery : IRequest<PersonalRecordsResponse>
    {
        public int UserId { get; set; }
    }

    public class ExportUserDataCommand : IRequest<ExportDataResponse>
    {
        public int UserId { get; set; }
        public string Format { get; set; } = "json";
        public bool IncludePersonalData { get; set; } = true;
    }

    public class ChangePasswordCommand : IRequest
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class GetPrivacySettingsQuery : IRequest<PrivacySettingsResponse>
    {
        public int UserId { get; set; }
    }

    public class UpdatePrivacySettingsCommand : IRequest<UpdatePrivacySettingsResponse>
    {
        public int UserId { get; set; }
        public string? ProfileVisibility { get; set; }
        public bool? ShowActivityHistory { get; set; }
        public bool? ShowPersonalRecords { get; set; }
        public bool? ShowLocation { get; set; }
        public bool? ShowInLeaderboards { get; set; }
        public bool? AllowDirectMessages { get; set; }
        public bool? AllowChallengeInvites { get; set; }
        public bool? ShareDataWithPartners { get; set; }
        public List<int>? BlockedUserIds { get; set; }
    }

    public class DeleteAccountCommand : IRequest<DeleteAccountResponse>
    {
        public int UserId { get; set; }
        public string ConfirmationPassword { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

    public class GetPublicUserProfileQuery : IRequest<PublicUserProfileResponse?>
    {
        public int UserId { get; set; }
        public int ViewerUserId { get; set; }
    }
}
