using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FitnessVibe.Application.Commands.Goals;
using FitnessVibe.Application.Queries.Goals;
using System.Security.Claims;

namespace FitnessVibe.API.Controllers
{
    /// <summary>
    /// Goals Controller - the vision board and progress tracker of your fitness journey.
    /// Think of this as your personal mission control where you set ambitious targets,
    /// break them down into achievable milestones, and track your progress step by step.
    /// Every great fitness transformation starts with a clear goal - this is where yours come to life.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class GoalsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GoalsController> _logger;

        public GoalsController(IMediator mediator, ILogger<GoalsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all user goals with their current progress.
        /// Like reviewing your fitness vision board to see all your targets and how close you are to achieving them.
        /// </summary>
        /// <param name="status">Filter by goal status (active, completed, paused, all)</param>
        /// <param name="category">Filter by goal category (fitness, weight, strength, endurance, etc.)</param>
        /// <returns>User's goals with progress information</returns>
        /// <response code="200">Goals retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(UserGoalsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserGoalsResponse>> GetUserGoals(
            [FromQuery] string status = "active",
            [FromQuery] string? category = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetUserGoalsQuery 
                { 
                    UserId = userId,
                    Status = status,
                    Category = category
                };

                _logger.LogDebug("Getting goals for user: {UserId}, status: {Status}", userId, status);

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get goals for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Goals",
                    Detail = "Unable to retrieve your goals.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Create a new fitness goal.
        /// Like setting a new target on your fitness vision board - could be running a 5K, losing weight, or building strength.
        /// </summary>
        /// <param name="command">Goal creation details</param>
        /// <returns>Created goal information</returns>
        /// <response code="201">Goal created successfully</response>
        /// <response code="400">Invalid goal data</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreateGoalResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateGoalResponse>> CreateGoal([FromBody] CreateGoalCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;

                _logger.LogInformation("Creating goal for user {UserId}: {GoalTitle}", userId, command.Title);

                var result = await _mediator.Send(command);

                _logger.LogInformation("Goal created successfully: {GoalId}", result.GoalId);

                return CreatedAtAction(
                    nameof(GetGoal),
                    new { goalId = result.GoalId },
                    result
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create goal for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Goal Creation Failed",
                    Detail = "Unable to create your goal. Please check your data and try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get details of a specific goal.
        /// Like examining one target on your vision board to see all the details and progress.
        /// </summary>
        /// <param name="goalId">Goal ID</param>
        /// <returns>Detailed goal information</returns>
        /// <response code="200">Goal retrieved successfully</response>
        /// <response code="404">Goal not found</response>
        [HttpGet("{goalId}")]
        [ProducesResponseType(typeof(GoalDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GoalDetailResponse>> GetGoal(int goalId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetGoalDetailQuery 
                { 
                    GoalId = goalId,
                    UserId = userId
                };

                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Goal Not Found",
                        Detail = "The specified goal could not be found.",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get goal {GoalId} for user: {UserId}", goalId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Goal",
                    Detail = "Unable to retrieve goal details.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Update an existing goal.
        /// Like revising a target on your vision board to make it more realistic or challenging.
        /// </summary>
        /// <param name="goalId">Goal ID to update</param>
        /// <param name="command">Updated goal information</param>
        /// <returns>Updated goal details</returns>
        /// <response code="200">Goal updated successfully</response>
        /// <response code="404">Goal not found</response>
        /// <response code="400">Invalid goal data</response>
        [HttpPut("{goalId}")]
        [ProducesResponseType(typeof(UpdateGoalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UpdateGoalResponse>> UpdateGoal(int goalId, [FromBody] UpdateGoalCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;
                command.GoalId = goalId;

                _logger.LogInformation("Updating goal {GoalId} for user: {UserId}", goalId, userId);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Goal Not Found",
                    Detail = "The specified goal could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update goal {GoalId} for user: {UserId}", goalId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Goal Update Failed",
                    Detail = "Unable to update your goal. Please check your data and try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Delete a goal.
        /// Like removing a target from your vision board because priorities have changed.
        /// </summary>
        /// <param name="goalId">Goal ID to delete</param>
        /// <returns>Deletion confirmation</returns>
        /// <response code="200">Goal deleted successfully</response>
        /// <response code="404">Goal not found</response>
        [HttpDelete("{goalId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteGoal(int goalId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new DeleteGoalCommand 
                { 
                    GoalId = goalId,
                    UserId = userId
                };

                await _mediator.Send(command);

                _logger.LogInformation("Goal {GoalId} deleted for user: {UserId}", goalId, userId);

                return Ok(new { message = "Goal deleted successfully" });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Goal Not Found",
                    Detail = "The specified goal could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete goal {GoalId} for user: {UserId}", goalId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Goal Deletion Failed",
                    Detail = "Unable to delete your goal. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Mark a goal as completed.
        /// Like celebrating and checking off a major achievement on your vision board!
        /// </summary>
        /// <param name="goalId">Goal ID to complete</param>
        /// <param name="command">Completion details</param>
        /// <returns>Completion celebration details</returns>
        /// <response code="200">Goal completed successfully</response>
        /// <response code="404">Goal not found</response>
        /// <response code="409">Goal already completed</response>
        [HttpPost("{goalId}/complete")]
        [ProducesResponseType(typeof(CompleteGoalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CompleteGoalResponse>> CompleteGoal(int goalId, [FromBody] CompleteGoalCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;
                command.GoalId = goalId;

                _logger.LogInformation("Completing goal {GoalId} for user: {UserId}", goalId, userId);

                var result = await _mediator.Send(command);

                _logger.LogInformation("Goal {GoalId} completed for user: {UserId}. XP awarded: {XpAwarded}", 
                    goalId, userId, result.XpAwarded);

                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Goal Not Found",
                    Detail = "The specified goal could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already completed"))
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Goal Already Completed",
                    Detail = "This goal has already been marked as completed.",
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete goal {GoalId} for user: {UserId}", goalId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Goal Completion Failed",
                    Detail = "Unable to mark goal as completed. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Pause or unpause a goal.
        /// Like putting a target on hold when life gets busy, then resuming when you're ready.
        /// </summary>
        /// <param name="goalId">Goal ID to pause/unpause</param>
        /// <param name="command">Pause action details</param>
        /// <returns>Goal status update</returns>
        /// <response code="200">Goal status updated successfully</response>
        /// <response code="404">Goal not found</response>
        [HttpPost("{goalId}/pause")]
        [ProducesResponseType(typeof(PauseGoalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PauseGoalResponse>> PauseGoal(int goalId, [FromBody] PauseGoalCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;
                command.GoalId = goalId;

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Goal Not Found",
                    Detail = "The specified goal could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to pause/unpause goal {GoalId} for user: {UserId}", goalId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Goal Status Update Failed",
                    Detail = "Unable to update goal status. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Update progress on a goal manually.
        /// Like updating your vision board with the latest progress when you've made strides outside the app.
        /// </summary>
        /// <param name="goalId">Goal ID to update progress for</param>
        /// <param name="command">Progress update details</param>
        /// <returns>Updated progress information</returns>
        /// <response code="200">Progress updated successfully</response>
        /// <response code="404">Goal not found</response>
        [HttpPost("{goalId}/progress")]
        [ProducesResponseType(typeof(UpdateProgressResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UpdateProgressResponse>> UpdateProgress(int goalId, [FromBody] UpdateGoalProgressCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;
                command.GoalId = goalId;

                _logger.LogInformation("Updating progress for goal {GoalId}, user: {UserId}, progress: {Progress}", 
                    goalId, userId, command.ProgressValue);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Goal Not Found",
                    Detail = "The specified goal could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update progress for goal {GoalId}, user: {UserId}", goalId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Progress Update Failed",
                    Detail = "Unable to update goal progress. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get goal suggestions based on user's fitness level and history.
        /// Like having a personal trainer suggest realistic and motivating targets for your vision board.
        /// </summary>
        /// <param name="category">Goal category to get suggestions for</param>
        /// <param name="difficulty">Preferred difficulty level (beginner, intermediate, advanced)</param>
        /// <returns>Personalized goal suggestions</returns>
        /// <response code="200">Goal suggestions retrieved successfully</response>
        [HttpGet("suggestions")]
        [ProducesResponseType(typeof(GoalSuggestionsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GoalSuggestionsResponse>> GetGoalSuggestions(
            [FromQuery] string? category = null,
            [FromQuery] string difficulty = "intermediate")
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetGoalSuggestionsQuery 
                { 
                    UserId = userId,
                    Category = category,
                    Difficulty = difficulty
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get goal suggestions for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Goal Suggestions",
                    Detail = "Unable to retrieve goal suggestions.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get goal templates for quick setup.
        /// Like browsing a catalog of proven fitness targets that others have successfully achieved.
        /// </summary>
        /// <param name="category">Filter templates by category</param>
        /// <param name="difficulty">Filter templates by difficulty level</param>
        /// <returns>Available goal templates</returns>
        /// <response code="200">Goal templates retrieved successfully</response>
        [HttpGet("templates")]
        [ProducesResponseType(typeof(GoalTemplatesResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GoalTemplatesResponse>> GetGoalTemplates(
            [FromQuery] string? category = null,
            [FromQuery] string? difficulty = null)
        {
            try
            {
                var query = new GetGoalTemplatesQuery 
                { 
                    Category = category,
                    Difficulty = difficulty
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get goal templates");
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Goal Templates",
                    Detail = "Unable to retrieve goal templates.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Create a goal from a template.
        /// Like choosing a proven target from the catalog and customizing it for your vision board.
        /// </summary>
        /// <param name="templateId">Template ID to use</param>
        /// <param name="command">Template customization details</param>
        /// <returns>Created goal from template</returns>
        /// <response code="201">Goal created from template successfully</response>
        /// <response code="404">Template not found</response>
        [HttpPost("templates/{templateId}/create")]
        [ProducesResponseType(typeof(CreateGoalFromTemplateResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CreateGoalFromTemplateResponse>> CreateGoalFromTemplate(
            int templateId, 
            [FromBody] CreateGoalFromTemplateCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;
                command.TemplateId = templateId;

                _logger.LogInformation("Creating goal from template {TemplateId} for user: {UserId}", templateId, userId);

                var result = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetGoal),
                    new { goalId = result.GoalId },
                    result
                );
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Template Not Found",
                    Detail = "The specified goal template could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create goal from template {TemplateId} for user: {UserId}", templateId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Goal Creation from Template Failed",
                    Detail = "Unable to create goal from template. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get goal progress history over time.
        /// Like looking at a chart that shows how you've been progressing toward your targets.
        /// </summary>
        /// <param name="goalId">Goal ID to get progress history for</param>
        /// <param name="timeframe">Timeframe for progress history (week, month, all)</param>
        /// <returns>Goal progress history</returns>
        /// <response code="200">Progress history retrieved successfully</response>
        /// <response code="404">Goal not found</response>
        [HttpGet("{goalId}/progress-history")]
        [ProducesResponseType(typeof(GoalProgressHistoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GoalProgressHistoryResponse>> GetGoalProgressHistory(
            int goalId, 
            [FromQuery] string timeframe = "month")
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetGoalProgressHistoryQuery 
                { 
                    GoalId = goalId,
                    UserId = userId,
                    Timeframe = timeframe
                };

                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Goal Not Found",
                        Detail = "The specified goal could not be found.",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get progress history for goal {GoalId}, user: {UserId}", goalId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Progress History",
                    Detail = "Unable to retrieve goal progress history.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get overall goal analytics and insights.
        /// Like getting a comprehensive report on how well you're doing with all your fitness targets.
        /// </summary>
        /// <returns>Goal analytics and insights</returns>
        /// <response code="200">Goal analytics retrieved successfully</response>
        [HttpGet("analytics")]
        [ProducesResponseType(typeof(GoalAnalyticsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GoalAnalyticsResponse>> GetGoalAnalytics()
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetGoalAnalyticsQuery { UserId = userId };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get goal analytics for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Goal Analytics",
                    Detail = "Unable to retrieve goal analytics.",
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

    // Request/Response DTOs for Goals

    public class UserGoalsResponse
    {
        public List<GoalSummaryResponse> Goals { get; set; } = new();
        public int TotalGoals { get; set; }
        public int ActiveGoals { get; set; }
        public int CompletedGoals { get; set; }
        public int PausedGoals { get; set; }
        public GoalOverviewStatsResponse OverviewStats { get; set; } = new();
    }

    public class GoalSummaryResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public double CurrentValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public double ProgressPercentage { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DaysRemaining { get; set; }
        public bool IsOverdue { get; set; }
        public string Priority { get; set; } = string.Empty;
    }

    public class GoalOverviewStatsResponse
    {
        public double OverallProgressPercentage { get; set; }
        public int GoalsOnTrack { get; set; }
        public int GoalsBehindSchedule { get; set; }
        public int GoalsCompleted { get; set; }
        public DateTime? NextDeadline { get; set; }
        public string? NextDeadlineGoal { get; set; }
    }

    public class CreateGoalResponse
    {
        public int GoalId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Milestones { get; set; } = new();
        public string MotivationalMessage { get; set; } = string.Empty;
    }

    public class GoalDetailResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public double CurrentValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public double ProgressPercentage { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? PausedAt { get; set; }
        public string? PauseReason { get; set; }
        public List<GoalMilestoneResponse> Milestones { get; set; } = new();
        public List<ProgressEntryResponse> RecentProgress { get; set; } = new();
        public GoalInsightsResponse Insights { get; set; } = new();
        public bool IsOnTrack { get; set; }
        public DateTime? ProjectedCompletionDate { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class GoalMilestoneResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public DateTime TargetDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int Order { get; set; }
    }

    public class ProgressEntryResponse
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime RecordedAt { get; set; }
        public string Source { get; set; } = string.Empty; // Manual, Activity, System
        public int? ActivityId { get; set; }
        public double DeltaFromPrevious { get; set; }
    }

    public class GoalInsightsResponse
    {
        public double AverageProgressPerWeek { get; set; }
        public string ProgressTrend { get; set; } = string.Empty; // Accelerating, Steady, Slowing, Stalled
        public int DaysToCompletion { get; set; }
        public double CompletionProbability { get; set; }
        public List<string> Recommendations { get; set; } = new();
        public List<string> Achievements { get; set; } = new();
        public string MotivationalMessage { get; set; } = string.Empty;
    }

    public class UpdateGoalResponse
    {
        public int GoalId { get; set; }
        public string Title { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public List<string> UpdatedFields { get; set; } = new();
    }

    public class CompleteGoalResponse
    {
        public int GoalId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CompletedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public int XpAwarded { get; set; }
        public List<string> BadgesEarned { get; set; } = new();
        public bool LeveledUp { get; set; }
        public int? NewLevel { get; set; }
        public string CelebrationMessage { get; set; } = string.Empty;
        public List<string> NextGoalSuggestions { get; set; } = new();
    }

    public class PauseGoalResponse
    {
        public int GoalId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; }
        public string? Reason { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class UpdateProgressResponse
    {
        public int GoalId { get; set; }
        public double PreviousValue { get; set; }
        public double NewValue { get; set; }
        public double ProgressChange { get; set; }
        public double NewProgressPercentage { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool MilestoneReached { get; set; }
        public string? MilestoneTitle { get; set; }
        public bool GoalCompleted { get; set; }
        public string MotivationalMessage { get; set; } = string.Empty;
    }

    public class GoalSuggestionsResponse
    {
        public List<GoalSuggestionResponse> Suggestions { get; set; } = new();
        public string ReasoningBasis { get; set; } = string.Empty;
        public UserFitnessProfileResponse UserProfile { get; set; } = new();
    }

    public class GoalSuggestionResponse
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double SuggestedTargetValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int SuggestedDurationDays { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public string Reasoning { get; set; } = string.Empty;
        public List<string> Benefits { get; set; } = new();
        public double SuccessRate { get; set; }
        public string Priority { get; set; } = string.Empty;
    }

    public class UserFitnessProfileResponse
    {
        public string FitnessLevel { get; set; } = string.Empty;
        public string PrimaryGoal { get; set; } = string.Empty;
        public List<string> FavoriteActivities { get; set; } = new();
        public int CurrentStreak { get; set; }
        public double AverageWeeklyActivities { get; set; }
        public List<string> CompletedGoalCategories { get; set; } = new();
    }

    public class GoalTemplatesResponse
    {
        public List<GoalTemplateResponse> Templates { get; set; } = new();
        public List<GoalCategoryResponse> Categories { get; set; } = new();
        public GoalTemplateResponse? FeaturedTemplate { get; set; }
    }

    public class GoalTemplateResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public double DefaultTargetValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int DefaultDurationDays { get; set; }
        public List<string> Milestones { get; set; } = new();
        public int UsageCount { get; set; }
        public double SuccessRate { get; set; }
        public List<string> Tags { get; set; } = new();
        public bool IsFeatured { get; set; }
        public string IconUrl { get; set; } = string.Empty;
    }

    public class GoalCategoryResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public int TemplateCount { get; set; }
        public string ColorCode { get; set; } = string.Empty;
    }

    public class CreateGoalFromTemplateResponse
    {
        public int GoalId { get; set; }
        public int TemplateId { get; set; }
        public string Title { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public DateTime Deadline { get; set; }
        public List<GoalMilestoneResponse> Milestones { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string WelcomeMessage { get; set; } = string.Empty;
    }

    public class GoalProgressHistoryResponse
    {
        public int GoalId { get; set; }
        public string GoalTitle { get; set; } = string.Empty;
        public string Timeframe { get; set; } = string.Empty;
        public List<ProgressHistoryEntryResponse> History { get; set; } = new();
        public ProgressStatsResponse Stats { get; set; } = new();
        public List<MilestoneHistoryResponse> MilestoneHistory { get; set; } = new();
    }

    public class ProgressHistoryEntryResponse
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public double ProgressPercentage { get; set; }
        public string? Note { get; set; }
        public string Source { get; set; } = string.Empty;
        public bool IsMilestone { get; set; }
    }

    public class ProgressStatsResponse
    {
        public double TotalProgress { get; set; }
        public double AverageProgressPerDay { get; set; }
        public double BestSingleDayProgress { get; set; }
        public DateTime? BestProgressDate { get; set; }
        public int ConsistentDays { get; set; }
        public string ProgressTrend { get; set; } = string.Empty;
    }

    public class MilestoneHistoryResponse
    {
        public string Title { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public DateTime TargetDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; }
        public int DaysEarlyOrLate { get; set; }
    }

    public class GoalAnalyticsResponse
    {
        public GoalAnalyticsSummaryResponse Summary { get; set; } = new();
        public List<CategoryAnalyticsResponse> ByCategory { get; set; } = new();
        public List<MonthlyGoalAnalyticsResponse> MonthlyTrends { get; set; } = new();
        public GoalBehaviorInsightsResponse BehaviorInsights { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }

    public class GoalAnalyticsSummaryResponse
    {
        public int TotalGoalsSet { get; set; }
        public int TotalGoalsCompleted { get; set; }
        public double CompletionRate { get; set; }
        public double AverageCompletionTime { get; set; }
        public int CurrentActiveGoals { get; set; }
        public string MostSuccessfulCategory { get; set; } = string.Empty;
        public string PreferredGoalDuration { get; set; } = string.Empty;
    }

    public class CategoryAnalyticsResponse
    {
        public string Category { get; set; } = string.Empty;
        public int GoalsSet { get; set; }
        public int GoalsCompleted { get; set; }
        public double CompletionRate { get; set; }
        public double AverageDuration { get; set; }
        public string PerformanceTrend { get; set; } = string.Empty;
    }

    public class MonthlyGoalAnalyticsResponse
    {
        public DateTime Month { get; set; }
        public int GoalsSet { get; set; }
        public int GoalsCompleted { get; set; }
        public double CompletionRate { get; set; }
        public double AverageProgress { get; set; }
    }

    public class GoalBehaviorInsightsResponse
    {
        public string OptimalGoalDuration { get; set; } = string.Empty;
        public string BestPerformingTimeOfYear { get; set; } = string.Empty;
        public double OptimalNumberOfActiveGoals { get; set; }
        public List<string> SuccessFactors { get; set; } = new();
        public List<string> CommonChallenges { get; set; } = new();
        public string MotivationProfile { get; set; } = string.Empty;
    }

    // Placeholder command and query classes (to be implemented in Application layer)
    public class GetUserGoalsQuery : IRequest<UserGoalsResponse>
    {
        public int UserId { get; set; }
        public string Status { get; set; } = "active";
        public string? Category { get; set; }
    }

    public class CreateGoalCommand : IRequest<CreateGoalResponse>
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public string Priority { get; set; } = "medium";
        public List<CreateMilestoneRequest> Milestones { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class CreateMilestoneRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public DateTime TargetDate { get; set; }
        public int Order { get; set; }
    }

    public class GetGoalDetailQuery : IRequest<GoalDetailResponse?>
    {
        public int GoalId { get; set; }
        public int UserId { get; set; }
    }

    public class UpdateGoalCommand : IRequest<UpdateGoalResponse>
    {
        public int UserId { get; set; }
        public int GoalId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? TargetValue { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Priority { get; set; }
        public List<CreateMilestoneRequest>? Milestones { get; set; }
    }

    public class DeleteGoalCommand : IRequest
    {
        public int UserId { get; set; }
        public int GoalId { get; set; }
    }

    public class CompleteGoalCommand : IRequest<CompleteGoalResponse>
    {
        public int UserId { get; set; }
        public int GoalId { get; set; }
        public string? CompletionNote { get; set; }
        public DateTime? CompletionDate { get; set; }
    }

    public class PauseGoalCommand : IRequest<PauseGoalResponse>
    {
        public int UserId { get; set; }
        public int GoalId { get; set; }
        public bool IsPaused { get; set; }
        public string? Reason { get; set; }
    }

    public class UpdateGoalProgressCommand : IRequest<UpdateProgressResponse>
    {
        public int UserId { get; set; }
        public int GoalId { get; set; }
        public double ProgressValue { get; set; }
        public string? Note { get; set; }
        public DateTime? RecordedAt { get; set; }
    }

    public class GetGoalSuggestionsQuery : IRequest<GoalSuggestionsResponse>
    {
        public int UserId { get; set; }
        public string? Category { get; set; }
        public string Difficulty { get; set; } = "intermediate";
    }

    public class GetGoalTemplatesQuery : IRequest<GoalTemplatesResponse>
    {
        public string? Category { get; set; }
        public string? Difficulty { get; set; }
    }

    public class CreateGoalFromTemplateCommand : IRequest<CreateGoalFromTemplateResponse>
    {
        public int UserId { get; set; }
        public int TemplateId { get; set; }
        public string? CustomTitle { get; set; }
        public double? CustomTargetValue { get; set; }
        public DateTime? CustomDeadline { get; set; }
        public string? CustomPriority { get; set; }
    }

    public class GetGoalProgressHistoryQuery : IRequest<GoalProgressHistoryResponse?>
    {
        public int GoalId { get; set; }
        public int UserId { get; set; }
        public string Timeframe { get; set; } = "month";
    }

    public class GetGoalAnalyticsQuery : IRequest<GoalAnalyticsResponse>
    {
        public int UserId { get; set; }
    }
}
