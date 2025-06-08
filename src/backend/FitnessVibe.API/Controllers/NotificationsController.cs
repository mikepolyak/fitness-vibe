using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FitnessVibe.Application.Commands.Notifications;
using FitnessVibe.Application.Queries.Notifications;
using System.Security.Claims;

namespace FitnessVibe.API.Controllers
{
    /// <summary>
    /// Notifications Controller - the smart assistant that keeps your fitness journey on track.
    /// Think of this as your personal fitness coach who taps you on the shoulder at just the right moment -
    /// reminding you to work out, celebrating your achievements, alerting you to challenges,
    /// and keeping you connected with your fitness community. It's the voice that whispers
    /// "time for your workout" and shouts "congratulations on your achievement!"
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(IMediator mediator, ILogger<NotificationsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get user's notifications with filtering and pagination.
        /// Like checking your fitness assistant's message board to see what's important.
        /// </summary>
        /// <param name="type">Filter by notification type (all, workout, achievement, social, challenge)</param>
        /// <param name="status">Filter by status (all, unread, read)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20, max: 50)</param>
        /// <returns>User's notifications</returns>
        /// <response code="200">Notifications retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(NotificationsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<NotificationsResponse>> GetNotifications(
            [FromQuery] string type = "all",
            [FromQuery] string status = "all",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetNotificationsQuery 
                { 
                    UserId = userId,
                    Type = type,
                    Status = status,
                    Page = Math.Max(1, page),
                    PageSize = Math.Min(50, Math.Max(1, pageSize))
                };

                _logger.LogDebug("Getting notifications for user: {UserId}, type: {Type}, status: {Status}", 
                    userId, type, status);

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get notifications for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Notifications",
                    Detail = "Unable to retrieve your notifications.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get count of unread notifications by type.
        /// Like seeing notification badges on your fitness app icon.
        /// </summary>
        /// <returns>Unread notification counts</returns>
        /// <response code="200">Notification counts retrieved successfully</response>
        [HttpGet("counts")]
        [ProducesResponseType(typeof(NotificationCountsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<NotificationCountsResponse>> GetNotificationCounts()
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetNotificationCountsQuery { UserId = userId };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get notification counts for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Notification Counts",
                    Detail = "Unable to retrieve notification counts.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Mark a notification as read.
        /// Like acknowledging your fitness assistant's message.
        /// </summary>
        /// <param name="notificationId">Notification ID to mark as read</param>
        /// <returns>Mark as read confirmation</returns>
        /// <response code="200">Notification marked as read successfully</response>
        /// <response code="404">Notification not found</response>
        [HttpPost("{notificationId}/read")]
        [ProducesResponseType(typeof(MarkNotificationReadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MarkNotificationReadResponse>> MarkAsRead(int notificationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new MarkNotificationReadCommand 
                { 
                    UserId = userId,
                    NotificationId = notificationId
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Notification Not Found",
                    Detail = "The specified notification could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark notification {NotificationId} as read for user: {UserId}", 
                    notificationId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Mark as Read",
                    Detail = "Unable to mark notification as read.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Mark multiple notifications as read.
        /// Like clearing several messages from your fitness assistant at once.
        /// </summary>
        /// <param name="command">Notifications to mark as read</param>
        /// <returns>Bulk mark as read confirmation</returns>
        /// <response code="200">Notifications marked as read successfully</response>
        [HttpPost("mark-read")]
        [ProducesResponseType(typeof(BulkMarkReadResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<BulkMarkReadResponse>> BulkMarkAsRead([FromBody] BulkMarkReadCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogInformation("Bulk marking {Count} notifications as read for user: {UserId}", 
                    command.NotificationIds.Count, userId);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to bulk mark notifications as read for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Mark Notifications as Read",
                    Detail = "Unable to mark notifications as read.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Mark all notifications as read.
        /// Like clearing your entire fitness assistant message board.
        /// </summary>
        /// <param name="type">Optional: only mark specific type as read</param>
        /// <returns>Mark all as read confirmation</returns>
        /// <response code="200">All notifications marked as read successfully</response>
        [HttpPost("mark-all-read")]
        [ProducesResponseType(typeof(MarkAllReadResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<MarkAllReadResponse>> MarkAllAsRead([FromQuery] string? type = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new MarkAllReadCommand 
                { 
                    UserId = userId,
                    Type = type
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark all notifications as read for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Mark All as Read",
                    Detail = "Unable to mark all notifications as read.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Delete a notification.
        /// Like dismissing a message from your fitness assistant permanently.
        /// </summary>
        /// <param name="notificationId">Notification ID to delete</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Notification deleted successfully</response>
        /// <response code="404">Notification not found</response>
        [HttpDelete("{notificationId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteNotification(int notificationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new DeleteNotificationCommand 
                { 
                    UserId = userId,
                    NotificationId = notificationId
                };

                await _mediator.Send(command);

                return Ok(new { message = "Notification deleted successfully" });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Notification Not Found",
                    Detail = "The specified notification could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete notification {NotificationId} for user: {UserId}", 
                    notificationId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Delete Notification",
                    Detail = "Unable to delete notification.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get notification preferences and settings.
        /// Like checking how your fitness assistant is configured to help you.
        /// </summary>
        /// <returns>Notification preferences</returns>
        /// <response code="200">Notification preferences retrieved successfully</response>
        [HttpGet("preferences")]
        [ProducesResponseType(typeof(NotificationPreferencesResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<NotificationPreferencesResponse>> GetNotificationPreferences()
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetNotificationPreferencesQuery { UserId = userId };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get notification preferences for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Notification Preferences",
                    Detail = "Unable to retrieve notification preferences.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Update notification preferences and settings.
        /// Like training your fitness assistant to communicate with you exactly how you prefer.
        /// </summary>
        /// <param name="command">Updated notification preferences</param>
        /// <returns>Updated preferences confirmation</returns>
        /// <response code="200">Notification preferences updated successfully</response>
        [HttpPut("preferences")]
        [ProducesResponseType(typeof(UpdateNotificationPreferencesResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UpdateNotificationPreferencesResponse>> UpdateNotificationPreferences(
            [FromBody] UpdateNotificationPreferencesCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogInformation("Updating notification preferences for user: {UserId}", userId);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update notification preferences for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Update Notification Preferences",
                    Detail = "Unable to update notification preferences.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Schedule a custom workout reminder.
        /// Like setting an alarm with your personal fitness assistant to remind you to work out.
        /// </summary>
        /// <param name="command">Reminder scheduling details</param>
        /// <returns>Scheduled reminder details</returns>
        /// <response code="201">Reminder scheduled successfully</response>
        /// <response code="400">Invalid reminder data</response>
        [HttpPost("reminders/workout")]
        [ProducesResponseType(typeof(ScheduleReminderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ScheduleReminderResponse>> ScheduleWorkoutReminder([FromBody] ScheduleWorkoutReminderCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogInformation("Scheduling workout reminder for user {UserId} at {Time}", 
                    userId, command.ReminderTime);

                var result = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetReminder),
                    new { reminderId = result.ReminderId },
                    result
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to schedule workout reminder for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Schedule Reminder",
                    Detail = "Unable to schedule workout reminder. Please check your data and try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get user's scheduled reminders.
        /// Like checking what alarms your fitness assistant has set for you.
        /// </summary>
        /// <param name="type">Filter by reminder type (workout, goal, challenge)</param>
        /// <param name="status">Filter by status (active, inactive, all)</param>
        /// <returns>User's scheduled reminders</returns>
        /// <response code="200">Reminders retrieved successfully</response>
        [HttpGet("reminders")]
        [ProducesResponseType(typeof(RemindersResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<RemindersResponse>> GetReminders(
            [FromQuery] string type = "all",
            [FromQuery] string status = "active")
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetRemindersQuery 
                { 
                    UserId = userId,
                    Type = type,
                    Status = status
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get reminders for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Reminders",
                    Detail = "Unable to retrieve your reminders.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get details of a specific reminder.
        /// Like examining the details of one of your fitness assistant's scheduled alarms.
        /// </summary>
        /// <param name="reminderId">Reminder ID</param>
        /// <returns>Reminder details</returns>
        /// <response code="200">Reminder retrieved successfully</response>
        /// <response code="404">Reminder not found</response>
        [HttpGet("reminders/{reminderId}")]
        [ProducesResponseType(typeof(ReminderDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReminderDetailResponse>> GetReminder(int reminderId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetReminderDetailQuery 
                { 
                    ReminderId = reminderId,
                    UserId = userId
                };

                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Reminder Not Found",
                        Detail = "The specified reminder could not be found.",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get reminder {ReminderId} for user: {UserId}", reminderId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Reminder",
                    Detail = "Unable to retrieve reminder details.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Update or modify a scheduled reminder.
        /// Like adjusting one of your fitness assistant's alarms.
        /// </summary>
        /// <param name="reminderId">Reminder ID to update</param>
        /// <param name="command">Updated reminder details</param>
        /// <returns>Updated reminder details</returns>
        /// <response code="200">Reminder updated successfully</response>
        /// <response code="404">Reminder not found</response>
        [HttpPut("reminders/{reminderId}")]
        [ProducesResponseType(typeof(UpdateReminderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UpdateReminderResponse>> UpdateReminder(int reminderId, [FromBody] UpdateReminderCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;
                command.ReminderId = reminderId;

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Reminder Not Found",
                    Detail = "The specified reminder could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update reminder {ReminderId} for user: {UserId}", reminderId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Update Reminder",
                    Detail = "Unable to update reminder.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Delete a scheduled reminder.
        /// Like turning off one of your fitness assistant's alarms.
        /// </summary>
        /// <param name="reminderId">Reminder ID to delete</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Reminder deleted successfully</response>
        /// <response code="404">Reminder not found</response>
        [HttpDelete("reminders/{reminderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteReminder(int reminderId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new DeleteReminderCommand 
                { 
                    ReminderId = reminderId,
                    UserId = userId
                };

                await _mediator.Send(command);

                return Ok(new { message = "Reminder deleted successfully" });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Reminder Not Found",
                    Detail = "The specified reminder could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete reminder {ReminderId} for user: {UserId}", reminderId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Delete Reminder",
                    Detail = "Unable to delete reminder.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Register a device for push notifications.
        /// Like giving your fitness assistant permission to send you messages on your phone.
        /// </summary>
        /// <param name="command">Device registration details</param>
        /// <returns>Device registration confirmation</returns>
        /// <response code="200">Device registered successfully</response>
        /// <response code="400">Invalid device data</response>
        [HttpPost("devices/register")]
        [ProducesResponseType(typeof(RegisterDeviceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegisterDeviceResponse>> RegisterDevice([FromBody] RegisterDeviceCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogInformation("Registering device for push notifications, user: {UserId}", userId);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register device for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Device Registration Failed",
                    Detail = "Unable to register device for push notifications.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Unregister a device from push notifications.
        /// Like revoking your fitness assistant's permission to send messages to a specific device.
        /// </summary>
        /// <param name="deviceToken">Device token to unregister</param>
        /// <returns>Unregistration confirmation</returns>
        /// <response code="200">Device unregistered successfully</response>
        [HttpPost("devices/unregister")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UnregisterDevice([FromBody] UnregisterDeviceCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                await _mediator.Send(command);

                return Ok(new { message = "Device unregistered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unregister device for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Device Unregistration Failed",
                    Detail = "Unable to unregister device.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Send a test notification to verify push notification setup.
        /// Like asking your fitness assistant to send you a test message to make sure everything works.
        /// </summary>
        /// <returns>Test notification confirmation</returns>
        /// <response code="200">Test notification sent successfully</response>
        [HttpPost("test")]
        [ProducesResponseType(typeof(TestNotificationResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<TestNotificationResponse>> SendTestNotification()
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new SendTestNotificationCommand { UserId = userId };

                _logger.LogInformation("Sending test notification for user: {UserId}", userId);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send test notification for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Test Notification Failed",
                    Detail = "Unable to send test notification.",
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

    // Request/Response DTOs for Notifications

    public class NotificationsResponse
    {
        public List<NotificationResponse> Notifications { get; set; } = new();
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasMore { get; set; }
    }

    public class NotificationResponse
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // Workout, Achievement, Social, Challenge, System
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead { get; set; }
        public string Priority { get; set; } = string.Empty; // Low, Medium, High, Urgent
        public string? ActionUrl { get; set; }
        public string? ActionText { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
        public string? IconUrl { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class NotificationCountsResponse
    {
        public int Total { get; set; }
        public int Workout { get; set; }
        public int Achievement { get; set; }
        public int Social { get; set; }
        public int Challenge { get; set; }
        public int System { get; set; }
        public int HighPriority { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class MarkNotificationReadResponse
    {
        public int NotificationId { get; set; }
        public DateTime ReadAt { get; set; }
        public int RemainingUnreadCount { get; set; }
    }

    public class BulkMarkReadResponse
    {
        public int MarkedCount { get; set; }
        public int RemainingUnreadCount { get; set; }
        public DateTime ProcessedAt { get; set; }
        public List<int> FailedNotificationIds { get; set; } = new();
    }

    public class MarkAllReadResponse
    {
        public int MarkedCount { get; set; }
        public string? Type { get; set; }
        public DateTime ProcessedAt { get; set; }
    }

    public class NotificationPreferencesDetailResponse
    {
        public bool PushNotificationsEnabled { get; set; }
        public bool EmailNotificationsEnabled { get; set; }
        public WorkoutNotificationSettingsResponse WorkoutReminders { get; set; } = new();
        public SocialNotificationSettingsResponse Social { get; set; } = new();
        public AchievementNotificationSettingsResponse Achievements { get; set; } = new();
        public ChallengeNotificationSettingsResponse Challenges { get; set; } = new();
        public QuietHoursResponse QuietHours { get; set; } = new();
        public List<string> BlockedTypes { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }

    public class WorkoutNotificationSettingsResponse
    {
        public bool Enabled { get; set; }
        public List<WorkoutReminderResponse> Reminders { get; set; } = new();
        public bool MotivationalMessages { get; set; }
        public bool StreakReminders { get; set; }
        public bool GoalDeadlineAlerts { get; set; }
    }

    public class WorkoutReminderResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeSpan Time { get; set; }
        public List<string> DaysOfWeek { get; set; } = new();
        public bool IsActive { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class SocialNotificationSettingsResponse
    {
        public bool Enabled { get; set; }
        public bool FollowerActivity { get; set; }
        public bool Kudos { get; set; }
        public bool Comments { get; set; }
        public bool NewFollowers { get; set; }
        public bool ChallengeInvites { get; set; }
        public bool LiveCheers { get; set; }
    }

    public class AchievementNotificationSettingsResponse
    {
        public bool Enabled { get; set; }
        public bool BadgesEarned { get; set; }
        public bool LevelUps { get; set; }
        public bool PersonalRecords { get; set; }
        public bool MilestoneReached { get; set; }
        public bool GoalCompleted { get; set; }
    }

    public class ChallengeNotificationSettingsResponse
    {
        public bool Enabled { get; set; }
        public bool NewChallenges { get; set; }
        public bool ChallengeUpdates { get; set; }
        public bool LeaderboardChanges { get; set; }
        public bool ChallengeDeadlines { get; set; }
        public bool ChallengeResults { get; set; }
    }

    public class QuietHoursResponse
    {
        public bool Enabled { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public List<string> DaysOfWeek { get; set; } = new();
        public bool AllowUrgentNotifications { get; set; }
    }

    public class UpdateNotificationPreferencesResponse
    {
        public int UserId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> UpdatedSettings { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }

    public class ScheduleReminderResponse
    {
        public int ReminderId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime NextTriggerTime { get; set; }
        public string RecurrencePattern { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RemindersResponse
    {
        public List<ReminderSummaryResponse> Reminders { get; set; } = new();
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public DateTime? NextReminder { get; set; }
    }

    public class ReminderSummaryResponse
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime NextTriggerTime { get; set; }
        public string RecurrencePattern { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastTriggered { get; set; }
    }

    public class ReminderDetailResponse
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime NextTriggerTime { get; set; }
        public string RecurrencePattern { get; set; } = string.Empty;
        public List<string> DaysOfWeek { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastTriggered { get; set; }
        public int TriggerCount { get; set; }
        public Dictionary<string, object> Settings { get; set; } = new();
    }

    public class UpdateReminderResponse
    {
        public int ReminderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime NextTriggerTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> UpdatedFields { get; set; } = new();
    }

    public class RegisterDeviceResponse
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        public bool PushNotificationsEnabled { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class TestNotificationResponse
    {
        public DateTime SentAt { get; set; }
        public int DeviceCount { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<TestNotificationResultResponse> Results { get; set; } = new();
    }

    public class TestNotificationResultResponse
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    // Placeholder command and query classes (to be implemented in Application layer)
    public class GetNotificationsQuery : IRequest<NotificationsResponse>
    {
        public int UserId { get; set; }
        public string Type { get; set; } = "all";
        public string Status { get; set; } = "all";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetNotificationCountsQuery : IRequest<NotificationCountsResponse>
    {
        public int UserId { get; set; }
    }

    public class MarkNotificationReadCommand : IRequest<MarkNotificationReadResponse>
    {
        public int UserId { get; set; }
        public int NotificationId { get; set; }
    }

    public class BulkMarkReadCommand : IRequest<BulkMarkReadResponse>
    {
        public int UserId { get; set; }
        public List<int> NotificationIds { get; set; } = new();
    }

    public class MarkAllReadCommand : IRequest<MarkAllReadResponse>
    {
        public int UserId { get; set; }
        public string? Type { get; set; }
    }

    public class DeleteNotificationCommand : IRequest
    {
        public int UserId { get; set; }
        public int NotificationId { get; set; }
    }

    public class GetNotificationPreferencesQuery : IRequest<NotificationPreferencesDetailResponse>
    {
        public int UserId { get; set; }
    }

    public class UpdateNotificationPreferencesCommand : IRequest<UpdateNotificationPreferencesResponse>
    {
        public int UserId { get; set; }
        public bool? PushNotificationsEnabled { get; set; }
        public bool? EmailNotificationsEnabled { get; set; }
        public WorkoutNotificationSettingsRequest? WorkoutReminders { get; set; }
        public SocialNotificationSettingsRequest? Social { get; set; }
        public AchievementNotificationSettingsRequest? Achievements { get; set; }
        public ChallengeNotificationSettingsRequest? Challenges { get; set; }
        public QuietHoursRequest? QuietHours { get; set; }
        public List<string>? BlockedTypes { get; set; }
    }

    public class WorkoutNotificationSettingsRequest
    {
        public bool? Enabled { get; set; }
        public bool? MotivationalMessages { get; set; }
        public bool? StreakReminders { get; set; }
        public bool? GoalDeadlineAlerts { get; set; }
    }

    public class SocialNotificationSettingsRequest
    {
        public bool? Enabled { get; set; }
        public bool? FollowerActivity { get; set; }
        public bool? Kudos { get; set; }
        public bool? Comments { get; set; }
        public bool? NewFollowers { get; set; }
        public bool? ChallengeInvites { get; set; }
        public bool? LiveCheers { get; set; }
    }

    public class AchievementNotificationSettingsRequest
    {
        public bool? Enabled { get; set; }
        public bool? BadgesEarned { get; set; }
        public bool? LevelUps { get; set; }
        public bool? PersonalRecords { get; set; }
        public bool? MilestoneReached { get; set; }
        public bool? GoalCompleted { get; set; }
    }

    public class ChallengeNotificationSettingsRequest
    {
        public bool? Enabled { get; set; }
        public bool? NewChallenges { get; set; }
        public bool? ChallengeUpdates { get; set; }
        public bool? LeaderboardChanges { get; set; }
        public bool? ChallengeDeadlines { get; set; }
        public bool? ChallengeResults { get; set; }
    }

    public class QuietHoursRequest
    {
        public bool? Enabled { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public List<string>? DaysOfWeek { get; set; }
        public bool? AllowUrgentNotifications { get; set; }
    }

    public class ScheduleWorkoutReminderCommand : IRequest<ScheduleReminderResponse>
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan ReminderTime { get; set; }
        public List<string> DaysOfWeek { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Dictionary<string, object> Settings { get; set; } = new();
    }

    public class GetRemindersQuery : IRequest<RemindersResponse>
    {
        public int UserId { get; set; }
        public string Type { get; set; } = "all";
        public string Status { get; set; } = "active";
    }

    public class GetReminderDetailQuery : IRequest<ReminderDetailResponse?>
    {
        public int ReminderId { get; set; }
        public int UserId { get; set; }
    }

    public class UpdateReminderCommand : IRequest<UpdateReminderResponse>
    {
        public int UserId { get; set; }
        public int ReminderId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public TimeSpan? ReminderTime { get; set; }
        public List<string>? DaysOfWeek { get; set; }
        public string? Message { get; set; }
        public bool? IsActive { get; set; }
        public Dictionary<string, object>? Settings { get; set; }
    }

    public class DeleteReminderCommand : IRequest
    {
        public int UserId { get; set; }
        public int ReminderId { get; set; }
    }

    public class RegisterDeviceCommand : IRequest<RegisterDeviceResponse>
    {
        public int UserId { get; set; }
        public string DeviceToken { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty; // iOS, Android, Web
        public string DeviceModel { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;
    }

    public class UnregisterDeviceCommand : IRequest
    {
        public int UserId { get; set; }
        public string DeviceToken { get; set; } = string.Empty;
    }

    public class SendTestNotificationCommand : IRequest<TestNotificationResponse>
    {
        public int UserId { get; set; }
    }
}
