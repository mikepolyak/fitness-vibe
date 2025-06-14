using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FitnessVibe.Application.Commands.Activities;
using FitnessVibe.Application.Queries.Activities;
using FitnessVibe.Application.Exceptions;
using System.Security.Claims;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Application.DTOs.Activities;

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

        /// <summary>
        /// Initializes an ActivitiesController instance with required services.
        /// </summary>
        /// <param name="mediator">Mediator service for CQRS pattern.</param>
        /// <param name="logger">Logger service for activity tracking.</param>
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
        /// Get details of the currently active workout session.
        /// Track your progress in real-time during your workout!
        /// </summary>
        /// <param name="sessionId">ID of the active session</param>
        /// <returns>Current session status and metrics</returns>
        /// <response code="200">Live activity data retrieved successfully</response>
        /// <response code="404">Session not found</response>
        [HttpGet("{sessionId}/live")]
        [ProducesResponseType(typeof(LiveActivityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LiveActivityResponse>> GetLiveActivity(Guid sessionId)
        {
            try
            {
                var query = new GetLiveActivityQuery { SessionId = sessionId, UserId = GetCurrentUserId() };
                var result = await _mediator.Send(query);

                _logger.LogInformation("Retrieved live activity data for session {SessionId}", sessionId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Live activity not found: {SessionId}", sessionId);
                return NotFound(new ProblemDetails
                {
                    Title = "Session Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (UnauthorizedOperationException)
            {
                _logger.LogWarning("Unauthorized access to live activity: {SessionId}", sessionId);
                return Forbid();
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
        public async Task<ActionResult<CompleteActivityResponse>> CompleteActivity(Guid sessionId, [FromBody] CompleteActivityCommand command)
        {
            try
            {
                command.SessionId = sessionId;
                command.UserId = GetCurrentUserId();

                _logger.LogInformation("Completing activity session {SessionId} for user {UserId}", 
                    sessionId, command.UserId);

                var result = await _mediator.Send(command);

                _logger.LogInformation("Activity session {SessionId} completed successfully", sessionId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Session not found for completion: {SessionId}", sessionId);
                return NotFound(new ProblemDetails
                {
                    Title = "Session Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (UnauthorizedOperationException)
            {
                _logger.LogWarning("Unauthorized attempt to complete session: {SessionId}", sessionId);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete activity session: {SessionId}", sessionId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Complete Activity Failed",
                    Detail = "Unable to complete workout session. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Pause an active workout session.
        /// Take a breather while keeping your progress safe!
        /// </summary>
        /// <param name="sessionId">ID of the session to pause</param>
        /// <returns>Updated session status</returns>
        /// <response code="200">Session paused successfully</response>
        /// <response code="404">Session not found</response>
        [HttpPost("{sessionId}/pause")]
        [ProducesResponseType(typeof(PauseActivityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PauseActivityResponse>> PauseActivity(Guid sessionId)
        {
            try
            {
                var command = new PauseActivityCommand { SessionId = sessionId, UserId = GetCurrentUserId() };
                var result = await _mediator.Send(command);

                _logger.LogInformation("Activity session {SessionId} paused successfully", sessionId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Session not found for pause: {SessionId}", sessionId);
                return NotFound(new ProblemDetails
                {
                    Title = "Session Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (UnauthorizedOperationException)
            {
                _logger.LogWarning("Unauthorized attempt to pause session: {SessionId}", sessionId);
                return Forbid();
            }
        }

        /// <summary>
        /// Resume a paused workout session.
        /// Get back in the zone and continue crushing your goals!
        /// </summary>
        /// <param name="sessionId">ID of the session to resume</param>
        /// <returns>Updated session status</returns>
        /// <response code="200">Session resumed successfully</response>
        /// <response code="404">Session not found</response>
        [HttpPost("{sessionId}/resume")]
        [ProducesResponseType(typeof(ResumeActivityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResumeActivityResponse>> ResumeActivity(Guid sessionId)
        {
            try
            {
                var command = new ResumeActivityCommand { SessionId = sessionId, UserId = GetCurrentUserId() };
                var result = await _mediator.Send(command);

                _logger.LogInformation("Activity session {SessionId} resumed successfully", sessionId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Session not found for resume: {SessionId}", sessionId);
                return NotFound(new ProblemDetails
                {
                    Title = "Session Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (UnauthorizedOperationException)
            {
                _logger.LogWarning("Unauthorized attempt to resume session: {SessionId}", sessionId);
                return Forbid();
            }
        }

        /// <summary>
        /// Cancel an active workout session.
        /// Sometimes life gets in the way, and that's okay!
        /// </summary>
        /// <param name="sessionId">ID of the session to cancel</param>
        /// <returns>Confirmation of cancellation</returns>
        /// <response code="200">Session cancelled successfully</response>
        /// <response code="404">Session not found</response>
        [HttpPost("{sessionId}/cancel")]
        [ProducesResponseType(typeof(CancelActivityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CancelActivityResponse>> CancelActivity(Guid sessionId)
        {
            try
            {
                var command = new CancelActivityCommand { SessionId = sessionId, UserId = GetCurrentUserId() };
                var result = await _mediator.Send(command);

                _logger.LogInformation("Activity session {SessionId} cancelled successfully", sessionId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Session not found for cancellation: {SessionId}", sessionId);
                return NotFound(new ProblemDetails
                {
                    Title = "Session Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (UnauthorizedOperationException)
            {
                _logger.LogWarning("Unauthorized attempt to cancel session: {SessionId}", sessionId);
                return Forbid();
            }
        }

        /// <summary>
        /// Get user's activity history.
        /// Review your fitness journey and track your progress over time!
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination</param>
        /// <returns>List of past activities with detailed metrics</returns>
        /// <response code="200">Activity history retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserActivityResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserActivityResponse>>> GetUserActivities([FromQuery] GetUserActivitiesQuery query)
        {
            query.UserId = GetCurrentUserId();
            var result = await _mediator.Send(query);

            _logger.LogInformation("Retrieved activity history for user {UserId}", query.UserId);
            return Ok(result);
        }

        /// <summary>
        /// Get available activity templates.
        /// Find the perfect workout plan for your fitness goals!
        /// </summary>
        /// <param name="query">Query parameters for filtering templates</param>
        /// <returns>List of available activity templates</returns>
        /// <response code="200">Templates retrieved successfully</response>
        [HttpGet("templates")]
        [ProducesResponseType(typeof(IEnumerable<ActivityTemplateResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ActivityTemplateResponse>>> GetActivityTemplates([FromQuery] GetActivityTemplatesQuery query)
        {
            var result = await _mediator.Send(query);

            _logger.LogInformation("Retrieved activity templates");
            return Ok(result);
        }

        /// <summary>
        /// Create a new activity template.
        /// Share your workout wisdom with the community!
        /// </summary>
        /// <param name="command">Template creation details</param>
        /// <returns>Newly created template</returns>
        /// <response code="201">Template created successfully</response>
        /// <response code="400">Invalid template data</response>
        [HttpPost("templates")]
        [ProducesResponseType(typeof(CreateActivityTemplateResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateActivityTemplateResponse>> CreateActivityTemplate([FromBody] CreateActivityTemplateCommand command)
        {
            try
            {
                command.CreatedById = GetCurrentUserId();
                var result = await _mediator.Send(command);

                _logger.LogInformation("Created activity template {TemplateId}", result.Id);
                return CreatedAtAction(
                    nameof(GetActivityTemplates),
                    new { id = result.Id },
                    result
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create activity template");
                return BadRequest(new ProblemDetails
                {
                    Title = "Create Template Failed",
                    Detail = "Unable to create activity template. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Gets the current user's ID from claims
        /// </summary>
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw UnauthorizedOperationException.For<User>(Guid.Empty, Guid.Empty, "access");
            }
            return userId;
        }

        /// <summary>
        /// Adds a GPS point to the activity's route
        /// </summary>
        /// <param name="id">The activity ID</param>
        /// <param name="command">The route point details</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/route/points")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddRoutePoint(Guid id, AddRoutePointCommand command)
        {
            if (id != command.ActivityId)
            {
                return BadRequest("Activity ID mismatch");
            }
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Gets the complete route for an activity
        /// </summary>
        /// <param name="id">The activity ID</param>
        /// <returns>The activity route including all GPS points</returns>
        [HttpGet("{id}/route")]
        [ProducesResponseType(typeof(ActivityRouteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoute(Guid id)
        {
            var result = await _mediator.Send(new GetActivityRouteQuery { ActivityId = id });
            return Ok(result);
        }

        /// <summary>
        /// Gets route statistics for a specific time range
        /// </summary>
        /// <param name="id">The activity ID</param>
        /// <param name="startTime">Start of the time range</param>
        /// <param name="endTime">End of the time range</param>
        /// <returns>Route statistics for the specified time range</returns>
        [HttpGet("{id}/route/stats")]
        [ProducesResponseType(typeof(ActivityRouteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRouteStats(Guid id, DateTime startTime, DateTime endTime)
        {
            var result = await _mediator.Send(new GetRouteStatisticsQuery 
            { 
                ActivityId = id,
                StartTime = startTime,
                EndTime = endTime
            });
            return Ok(result);
        }
    }

    // Placeholder command classes for pause/resume/cancel operations
    // These would be implemented similarly to the existing commands

    /// <summary>
    /// Command to pause a workout session.
    /// </summary>
    public class PauseActivityCommand : IRequest
    {
        /// <summary>
        /// User ID of the activity owner.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Session ID of the activity to pause.
        /// </summary>
        public Guid SessionId { get; set; }
    }

    /// <summary>
    /// Command to resume a paused workout session.
    /// </summary>
    public class ResumeActivityCommand : IRequest
    {
        /// <summary>
        /// User ID of the activity owner.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Session ID of the activity to resume.
        /// </summary>
        public Guid SessionId { get; set; }
    }

    /// <summary>
    /// Command to cancel a workout session.
    /// </summary>
    public class CancelActivityCommand : IRequest
    {
        /// <summary>
        /// User ID of the activity owner.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Session ID of the activity to cancel.
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// Optional reason for cancellation.
        /// </summary>
        public string? CancellationReason { get; set; }
    }
}
