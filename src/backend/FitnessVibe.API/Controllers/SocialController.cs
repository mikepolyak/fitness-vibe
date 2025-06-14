using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetC                var command = new GetSocialFeedQuery
                {
                    UserId = userId,
                    FeedType = feedType,
                    Page = page,
                    PageSize = Math.Min(pageSize, 50), // Cap at 50 posts per page
                    ActivityTypeFilter = activityTypeFilter,
                    IncludeLiveActivities = includeLiveActivities
                };rization;
using MediatR;
using FitnessVibe.Application.Commands.Social;
using FitnessVibe.Application.Queries.Social;
using System.Security.Claims;

namespace FitnessVibe.API.Controllers
{
    /// <summary>
    /// Social Controller - the community center of our fitness app.
    /// Think of this as your social hub where you can connect with workout buddies,
    /// share achievements, send encouragement, and build your fitness community network!
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class SocialController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SocialController> _logger;

        public SocialController(IMediator mediator, ILogger<SocialController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get the user's social activity feed.
        /// Like browsing your fitness community's news feed to see what everyone's been up to!
        /// </summary>
        /// <param name="feedType">Type of feed (Friends, Following, Public, Clubs)</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of posts per page</param>
        /// <param name="activityTypeFilter">Filter by specific activity type</param>
        /// <param name="includeLiveActivities">Include live workout sessions</param>
        /// <returns>Social feed with fitness posts and activity updates</returns>
        /// <response code="200">Social feed retrieved successfully</response>
        [HttpGet("feed")]
        [ProducesResponseType(typeof(SocialFeedResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<SocialFeedResponse>> GetSocialFeed(
            [FromQuery] string feedType = "Friends",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? activityTypeFilter = null,
            [FromQuery] bool includeLiveActivities = true)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Getting social feed for user {UserId}, type: {FeedType}", userId, feedType);

                var query = new GetSocialFeedQuery
                {
                    UserId = userId,
                    FeedType = feedType,
                    Page = page,
                    PageSize = Math.Min(pageSize, 50), // Cap at 50 posts per page
                    ActivityTypeFilter = activityTypeFilter,
                    IncludeLiveActivities = includeLiveActivities
                };

                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get social feed for user {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Get Social Feed Failed",
                    Detail = "Unable to retrieve your social feed",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get user's friends, friend requests, and social connections.
        /// Like checking your workout buddy network and pending invitations!
        /// </summary>
        /// <param name="section">Section to view (All, Friends, Pending, Sent, Suggestions)</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of friends per page</param>
        /// <param name="searchQuery">Search for specific friends</param>
        /// <param name="sortBy">Sort by (Name, RecentActivity, MutualFriends)</param>
        /// <returns>Complete friends and social connections data</returns>
        /// <response code="200">Friends data retrieved successfully</response>
        [HttpGet("friends")]
        [ProducesResponseType(typeof(UserFriendsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserFriendsResponse>> GetFriends(
            [FromQuery] string section = "All",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? searchQuery = null,
            [FromQuery] string sortBy = "Name")
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Getting friends for user {UserId}, section: {Section}", userId, section);

                var query = new GetUserFriendsQuery
                {
                    UserId = userId,
                    Section = section,
                    Page = page,
                    PageSize = Math.Min(pageSize, 100), // Cap at 100 friends per page
                    SearchQuery = searchQuery,
                    SortBy = sortBy,
                    IncludeActivityStatus = true,
                    IncludeMutualFriends = true
                };

                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get friends for user {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Get Friends Failed",
                    Detail = "Unable to retrieve your friends list",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Send a friend request to another user.
        /// Like asking someone to be your workout buddy in the fitness community!
        /// </summary>
        /// <param name="targetUserId">ID of the user to send request to</param>
        /// <param name="command">Friend request details</param>
        /// <returns>Friend request confirmation</returns>
        /// <response code="200">Friend request sent successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="409">Already friends or request exists</response>
        [HttpPost("friends/{targetUserId}/request")]
        [ProducesResponseType(typeof(SendFriendRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<SendFriendRequestResponse>> SendFriendRequest(
            Guid targetUserId, 
            [FromBody] SendFriendRequestCommand command)
        {
            try
            {
                command.UserId = GetCurrentUserId();
                command.TargetUserId = targetUserId;

                _logger.LogInformation("Processing friend request from user {UserId} to user {TargetUserId}", 
                    command.UserId, targetUserId);

                var result = await _mediator.Send(command);

                // Return appropriate status code based on result
                return result.Status switch
                {
                    "AlreadyFriends" or "RequestExists" => Conflict(result),
                    _ => Ok(result)
                };
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Friend request failed: {Error}", ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Friend Request Failed",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send friend request from user {UserId} to user {TargetUserId}", 
                    GetCurrentUserId(), targetUserId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Friend Request Failed",
                    Detail = "Unable to send friend request",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Respond to a friend request (accept or decline).
        /// Like deciding whether to accept a workout buddy invitation!
        /// </summary>
        /// <param name="requestId">ID of the friend request to respond to</param>
        /// <param name="command">Response details</param>
        /// <returns>Response confirmation</returns>
        /// <response code="200">Response recorded successfully</response>
        /// <response code="404">Friend request not found</response>
        [HttpPost("friends/requests/{requestId}/respond")]
        [ProducesResponseType(typeof(RespondToFriendRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RespondToFriendRequestResponse>> RespondToFriendRequest(
            Guid requestId, 
            [FromBody] RespondToFriendRequestCommand command)
        {
            try
            {
                command.UserId = GetCurrentUserId();
                command.FriendRequestId = requestId;

                _logger.LogInformation("Processing friend request response {RequestId} from user {UserId}: {Response}", 
                    requestId, command.UserId, command.Response);

                var result = await _mediator.Send(command);

                return Ok(result);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Friend Request Not Found",
                    Detail = $"Friend request {requestId} not found",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to respond to friend request {RequestId}", requestId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Friend Request Response Failed",
                    Detail = "Unable to process friend request response",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Send a cheer or encouragement to a friend during their workout.
        /// Like being a virtual cheerleader for your workout buddy!
        /// </summary>
        /// <param name="targetUserId">ID of the user to cheer</param>
        /// <param name="command">Cheer details</param>
        /// <returns>Cheer delivery confirmation</returns>
        /// <response code="200">Cheer sent successfully</response>
        /// <response code="400">Invalid cheer data</response>
        /// <response code="403">Not authorized to cheer this user</response>
        [HttpPost("cheer/{targetUserId}")]
        [ProducesResponseType(typeof(SendCheerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<SendCheerResponse>> SendCheer(
            Guid targetUserId, 
            [FromBody] SendCheerCommand command)
        {
            try
            {
                command.UserId = GetCurrentUserId();
                command.TargetUserId = targetUserId;

                _logger.LogInformation("Processing cheer from user {UserId} to user {TargetUserId}, type: {CheerType}", 
                    command.UserId, targetUserId, command.CheerType);

                var result = await _mediator.Send(command);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Send cheer failed: {Error}", ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Send Cheer Failed",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send cheer from user {UserId} to user {TargetUserId}", 
                    GetCurrentUserId(), targetUserId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Send Cheer Failed",
                    Detail = "Unable to send cheer",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Share an activity to the social feed.
        /// Like posting your workout achievement on the community bulletin board!
        /// </summary>
        /// <param name="activityId">ID of the activity to share</param>
        /// <param name="command">Share details</param>
        /// <returns>Share confirmation with post details</returns>
        /// <response code="200">Activity shared successfully</response>
        /// <response code="404">Activity not found</response>
        [HttpPost("share/activity/{activityId}")]
        [ProducesResponseType(typeof(ShareActivityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShareActivityResponse>> ShareActivity(
            Guid activityId, 
            [FromBody] ShareActivityCommand command)
        {
            try
            {
                command.UserId = GetCurrentUserId();
                command.ActivityId = activityId;

                _logger.LogInformation("Processing activity share for user {UserId}, activity {ActivityId}", 
                    command.UserId, activityId);

                var result = await _mediator.Send(command);

                return Ok(result);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Activity Not Found",
                    Detail = $"Activity {activityId} not found",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to share activity {ActivityId} for user {UserId}", 
                    activityId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Share Activity Failed",
                    Detail = "Unable to share activity",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Like a post in the social feed.
        /// Like giving a thumbs up to someone's fitness achievement!
        /// </summary>
        /// <param name="postId">ID of the post to like</param>
        /// <returns>Like confirmation</returns>
        /// <response code="200">Post liked successfully</response>
        /// <response code="404">Post not found</response>
        [HttpPost("posts/{postId}/like")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> LikePost(Guid postId)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Processing like for post {PostId} from user {UserId}", postId, userId);

                // This would be a LikePostCommand
                var command = new LikePostCommand
                {
                    UserId = userId,
                    PostId = postId
                };

                await _mediator.Send(command);

                return Ok(new { message = "Post liked successfully", postId });
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Post Not Found",
                    Detail = $"Post {postId} not found",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to like post {PostId} for user {UserId}", postId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Like Post Failed",
                    Detail = "Unable to like post",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Comment on a post in the social feed.
        /// Like leaving an encouraging message on someone's workout post!
        /// </summary>
        /// <param name="postId">ID of the post to comment on</param>
        /// <param name="request">Comment content</param>
        /// <returns>Comment confirmation</returns>
        /// <response code="201">Comment added successfully</response>
        /// <response code="404">Post not found</response>
        [HttpPost("posts/{postId}/comment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CommentOnPost(Guid postId, [FromBody] CommentOnPostRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                _logger.LogDebug("Processing comment for post {PostId} from user {UserId}", postId, userId);

                // This would be a CommentOnPostCommand
                var command = new CommentOnPostCommand
                {
                    UserId = userId,
                    PostId = postId,
                    Content = request.Content
                };

                var result = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetSocialFeed),
                    new { },
                    new { message = "Comment added successfully", commentId = result.CommentId }
                );
            }
            catch (ArgumentException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Post Not Found",
                    Detail = $"Post {postId} not found",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to comment on post {PostId} for user {UserId}", postId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Comment Failed",
                    Detail = "Unable to add comment",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get current user ID from JWT claims.
        /// Like reading your member ID from your gym access card.
        /// </summary>
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }
    }

    // Request/Response DTOs for social features

    /// <summary>
    /// Request model for commenting on a post
    /// </summary>
    public class CommentOnPostRequest
    {
        /// <summary>
        /// The comment text
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }

    // Placeholder command classes for social features
    // These would be implemented similarly to the existing commands

    /// <summary>
    /// Command for liking a post
    /// </summary>
    public class LikePostCommand : IRequest
    {
        /// <summary>
        /// The ID of the user liking the post
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The ID of the post being liked
        /// </summary>
        public Guid PostId { get; set; }
    }

    /// <summary>
    /// Command for commenting on a post
    /// </summary>
    public class CommentOnPostCommand : IRequest<CommentOnPostResponse>
    {
        /// <summary>
        /// The ID of the user commenting
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The ID of the post being commented on
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// The comment text
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response model for post comments
    /// </summary>
    public class CommentOnPostResponse
    {
        /// <summary>
        /// The ID of the created comment
        /// </summary>
        public Guid CommentId { get; set; }

        /// <summary>
        /// When the comment was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
