using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FitnessVibe.Application.Commands.Activities;
using FitnessVibe.Application.Queries.Activities;
using System.Security.Claims;

namespace FitnessVibe.API.Controllers
{
    /// <summary>
    /// Activities Controller - the workout command center of our fitness app.
    /// Think of this as your personal fitness dashboard where you can start workouts,
    /// track progress, complete sessions, and review your entire fitness journey.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ActivitiesController> _logger;

        public ActivitiesController(IMediator mediator, ILogger<ActivitiesController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Start a new workout session.
        /// Like pressing the "start" button on your fitness tracker for a new adventure!
        /// </summary>
        /// <param name="command">Workout start details</param>
        /// <returns>Session details and motivational information</returns>
        /// <response code="201">Workout session started successfully</response>
        /// <response code="400">Invalid workout parameters</response>
        /// <response code="409">User already has an active session</response>
        [HttpPost("start")]
        [ProducesResponseType(typeof(StartActivityResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<StartActivityResponse>> StartActivity([FromBody] StartActivityCommand command)
        {
            try
            {
                command.UserId = GetCurrentUserId();
                
                _logger.LogInformation("Starting activity session for user {UserId}, type: {ActivityType}", 
                    command.UserId, command.ActivityType);

                var result = await _mediator.Send(command);

                _logger.LogInformation("Activity session {SessionId} started successfully for user {UserId}", 
                    result.SessionId, command.UserId);

                return CreatedAtAction(
                    nameof(GetLiveActivity),
                    new { sessionId = result.SessionId },
                    result
                );
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("active session"))
            {
                _logger.LogWarning("Start activity failed - user already has active session: {UserId}", command.UserId);
                return Conflict(new ProblemDetails
                {
                    Title = "Active Session Exists",
                    Detail = ex.Message,
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start activity for user: {UserId}", command.UserId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Start Activity Failed",
                    Detail = "Unable to start workout session. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Complete an active workout session.
        /// Like crossing the finish line and celebrating your achievement!
        /// </summary>
        /// <param name="sessionId">ID of the session to complete</param>
        /// <param name="command">Completion details and final metrics</param>
        /// <returns>Workout results with stats, rewards, and achievements</returns>
        /// <response code="200">Workout completed successfully</response>
        /// <response code="400">Invalid completion data</response>
        /// <response code="404">Session not found</response>
        [HttpPost("{sessionId}/complete")]
        [ProducesResponseType(typeof(CompleteActivityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompleteActivityResponse>> CompleteActivity(
            int sessionId, 
            [FromBody] CompleteActivityCommand command)
        {
            try
            {
                command.UserId = GetCurrentUserId();
                command.SessionId = sessionId;

                _logger.LogInformation("Completing activity session {SessionId} for user {UserId}", 
                    sessionId, command.UserId);

                var result = await _mediator.Send(command);

                _logger.LogInformation("Activity session {SessionId} completed successfully for user {UserId}", 
                    sessionId, command.UserId);

                return Ok(result);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found"))
            {
                _logger.LogWarning("Complete activity failed - session not found: {SessionId}", sessionId);
                return NotFound(new ProblemDetails
                {
                    Title = "Session Not Found",
                    Detail = $"Activity session {sessionId} not found",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Complete activity failed - invalid operation: {Error}", ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Operation",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete activity session {SessionId}", sessionId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Complete Activity Failed",
                    Detail = "Unable to complete workout session. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get real-time information about an active workout session.
        /// Like checking your fitness tracker during a workout for live stats and encouragement!
        /// </summary>
        /// <param name="sessionId">ID of the live session (optional - gets current active session if not provided)</param>
        /// <returns>Live workout data with real-time metrics and social features</returns>
        /// <response code="200">Live session data retrieved successfully</response>
        /// <response code="404">No active session found</response>
        [HttpGet("live/{sessionId?}")]
        [ProducesResponseType(typeof(LiveActivityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LiveActivityResponse>> GetLiveActivity(int? sessionId = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Getting live activity for user {UserId}, session {SessionId}", userId, sessionId);

                var query = new GetLiveActivityQuery
                {
                    UserId = userId,
                    SessionId = sessionId,
                    IncludeRealTimeMetrics = true,
                    IncludeFriendsWatching = true,
                    IncludeCheers = true
                };

                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("No active"))
            {
                _logger.LogInformation("No active session found for user {UserId}", GetCurrentUserId());
                return NotFound(new ProblemDetails
                {
                    Title = "No Active Session",
                    Detail = "You don't have any active workout sessions",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get live activity for session {SessionId}", sessionId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Get Live Activity Failed",
                    Detail = "Unable to retrieve live activity data",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get user's activity history and workout timeline.
        /// Like browsing through your fitness journal to see all your progress and achievements!
        /// </summary>
        /// <param name="startDate">Filter activities from this date</param>
        /// <param name="endDate">Filter activities until this date</param>
        /// <param name="activityType">Filter by specific activity type</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of activities per page</param>
        /// <param name="sortBy">Sort field (Date, Distance, Duration, Calories)</param>
        /// <param name="sortDirection">Sort direction (Asc, Desc)</param>
        /// <returns>Paginated activity history with comprehensive statistics</returns>
        /// <response code="200">Activity history retrieved successfully</response>
        [HttpGet("history")]
        [ProducesResponseType(typeof(UserActivitiesResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserActivitiesResponse>> GetActivityHistory(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? activityType = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string sortBy = "Date",
            [FromQuery] string sortDirection = "Desc")
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Getting activity history for user {UserId}, page {Page}", userId, page);

                var query = new GetUserActivitiesQuery
                {
                    UserId = userId,
                    StartDate = startDate,
                    EndDate = endDate,
                    ActivityType = activityType,
                    Page = page,
                    PageSize = Math.Min(pageSize, 100), // Cap at 100 items per page
                    SortBy = sortBy,
                    SortDirection = sortDirection,
                    IncludeStats = true
                };

                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity history for user {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Get Activity History Failed",
                    Detail = "Unable to retrieve activity history",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get detailed information about a specific activity.
        /// Like opening a specific workout file to see all the details and stats!
        /// </summary>
        /// <param name="activityId">ID of the activity to retrieve</param>
        /// <returns>Detailed activity information</returns>
        /// <response code="200">Activity details retrieved successfully</response>
        /// <response code="404">Activity not found</response>
        [HttpGet("{activityId}")]
        [ProducesResponseType(typeof(ActivitySummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ActivitySummaryDto>> GetActivity(int activityId)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Getting activity {ActivityId} for user {UserId}", activityId, userId);

                // This would typically be a GetActivityByIdQuery
                var query = new GetUserActivitiesQuery
                {
                    UserId = userId,
                    Page = 1,
                    PageSize = 1,
                    IncludeStats = false
                };

                var result = await _mediator.Send(query);
                var activity = result.Activities.FirstOrDefault(a => a.Id == activityId);

                if (activity == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Activity Not Found",
                        Detail = $"Activity {activityId} not found",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return Ok(activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity {ActivityId}", activityId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Get Activity Failed",
                    Detail = "Unable to retrieve activity details",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Pause an active workout session.
        /// Like taking a water break during your workout!
        /// </summary>
        /// <param name="sessionId">ID of the session to pause</param>
        /// <returns>Updated session status</returns>
        /// <response code="200">Session paused successfully</response>
        /// <response code="404">Session not found</response>
        [HttpPost("{sessionId}/pause")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PauseActivity(int sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogInformation("Pausing activity session {SessionId} for user {UserId}", sessionId, userId);

                // This would be a PauseActivityCommand
                var command = new PauseActivityCommand
                {
                    UserId = userId,
                    SessionId = sessionId
                };

                await _mediator.Send(command);

                return Ok(new { message = "Activity paused successfully", sessionId });
            }
            catch (ArgumentException)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Session Not Found",
                    Detail = $"Activity session {sessionId} not found",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to pause activity session {SessionId}", sessionId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Pause Activity Failed",
                    Detail = "Unable to pause workout session",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Resume a paused workout session.
        /// Like getting back to your workout after a quick break!
        /// </summary>
        /// <param name="sessionId">ID of the session to resume</param>
        /// <returns>Updated session status</returns>
        /// <response code="200">Session resumed successfully</response>
        /// <response code="404">Session not found</response>
        [HttpPost("{sessionId}/resume")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ResumeActivity(int sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogInformation("Resuming activity session {SessionId} for user {UserId}", sessionId, userId);

                // This would be a ResumeActivityCommand
                var command = new ResumeActivityCommand
                {
                    UserId = userId,
                    SessionId = sessionId
                };

                await _mediator.Send(command);

                return Ok(new { message = "Activity resumed successfully", sessionId });
            }
            catch (ArgumentException)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Session Not Found",
                    Detail = $"Activity session {sessionId} not found",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resume activity session {SessionId}", sessionId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Resume Activity Failed",
                    Detail = "Unable to resume workout session",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Cancel an active workout session.
        /// Like deciding to end your workout early and cleaning up.
        /// </summary>
        /// <param name="sessionId">ID of the session to cancel</param>
        /// <returns>Cancellation confirmation</returns>
        /// <response code="200">Session cancelled successfully</response>
        /// <response code="404">Session not found</response>
        [HttpPost("{sessionId}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CancelActivity(int sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogInformation("Cancelling activity session {SessionId} for user {UserId}", sessionId, userId);

                // This would be a CancelActivityCommand
                var command = new CancelActivityCommand
                {
                    UserId = userId,
                    SessionId = sessionId
                };

                await _mediator.Send(command);

                return Ok(new { message = "Activity cancelled successfully", sessionId });
            }
            catch (ArgumentException)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Session Not Found",
                    Detail = $"Activity session {sessionId} not found",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel activity session {SessionId}", sessionId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Cancel Activity Failed",
                    Detail = "Unable to cancel workout session",
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

    // Placeholder command classes for pause/resume/cancel operations
    // These would be implemented similarly to the existing commands

    public class PauseActivityCommand : IRequest
    {
        public int UserId { get; set; }
        public int SessionId { get; set; }
    }

    public class ResumeActivityCommand : IRequest
    {
        public int UserId { get; set; }
        public int SessionId { get; set; }
    }

    public class CancelActivityCommand : IRequest
    {
        public int UserId { get; set; }
        public int SessionId { get; set; }
        public string? CancellationReason { get; set; }
    }
}
