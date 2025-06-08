using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FitnessVibe.Application.Commands.Clubs;
using FitnessVibe.Application.Queries.Clubs;
using System.Security.Claims;

namespace FitnessVibe.API.Controllers
{
    /// <summary>
    /// Clubs Controller - the community headquarters where fitness tribes are born.
    /// Think of this as the social club area of your gym where like-minded members form groups,
    /// plan events together, compete in team challenges, and support each other's journeys.
    /// Whether it's a running club, yoga enthusiasts, or weightlifting warriors,
    /// this is where individual fitness becomes a shared adventure.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ClubsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ClubsController> _logger;

        public ClubsController(IMediator mediator, ILogger<ClubsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get clubs that the user is a member of.
        /// Like checking which fitness groups you've joined at the gym.
        /// </summary>
        /// <param name="role">Filter by user's role in clubs (member, admin, owner, all)</param>
        /// <returns>User's club memberships</returns>
        /// <response code="200">User clubs retrieved successfully</response>
        [HttpGet("my-clubs")]
        [ProducesResponseType(typeof(UserClubsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserClubsResponse>> GetMyClubs([FromQuery] string role = "all")
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetUserClubsQuery 
                { 
                    UserId = userId,
                    Role = role
                };

                _logger.LogDebug("Getting clubs for user: {UserId}, role: {Role}", userId, role);

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get clubs for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Clubs",
                    Detail = "Unable to retrieve your clubs.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Search and discover clubs to join.
        /// Like browsing the directory of fitness groups available at your gym.
        /// </summary>
        /// <param name="query">Search query (name, description, interests)</param>
        /// <param name="category">Filter by club category</param>
        /// <param name="type">Filter by club type (public, private, invite-only)</param>
        /// <param name="location">Filter by location/region</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20, max: 50)</param>
        /// <returns>Discoverable clubs</returns>
        /// <response code="200">Clubs retrieved successfully</response>
        [HttpGet("discover")]
        [ProducesResponseType(typeof(DiscoverClubsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<DiscoverClubsResponse>> DiscoverClubs(
            [FromQuery] string? query = null,
            [FromQuery] string? category = null,
            [FromQuery] string? type = null,
            [FromQuery] string? location = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = GetCurrentUserId();
                var searchQuery = new DiscoverClubsQuery 
                { 
                    UserId = userId,
                    SearchTerm = query,
                    Category = category,
                    Type = type,
                    Location = location,
                    Page = Math.Max(1, page),
                    PageSize = Math.Min(50, Math.Max(1, pageSize))
                };

                var result = await _mediator.Send(searchQuery);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to discover clubs for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Discover Clubs",
                    Detail = "Unable to search for clubs.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Create a new fitness club.
        /// Like starting your own special interest group at the gym.
        /// </summary>
        /// <param name="command">Club creation details</param>
        /// <returns>Created club information</returns>
        /// <response code="201">Club created successfully</response>
        /// <response code="400">Invalid club data</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreateClubResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateClubResponse>> CreateClub([FromBody] CreateClubCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.CreatorUserId = userId;

                _logger.LogInformation("Creating club for user {UserId}: {ClubName}", userId, command.Name);

                var result = await _mediator.Send(command);

                _logger.LogInformation("Club created successfully: {ClubId}", result.ClubId);

                return CreatedAtAction(
                    nameof(GetClub),
                    new { clubId = result.ClubId },
                    result
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create club for user: {UserId}", GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Club Creation Failed",
                    Detail = "Unable to create your club. Please check your data and try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get details of a specific club.
        /// Like reading the information board of a fitness group to see what they're about.
        /// </summary>
        /// <param name="clubId">Club ID</param>
        /// <returns>Detailed club information</returns>
        /// <response code="200">Club retrieved successfully</response>
        /// <response code="404">Club not found</response>
        /// <response code="403">Access denied to private club</response>
        [HttpGet("{clubId}")]
        [ProducesResponseType(typeof(ClubDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ClubDetailResponse>> GetClub(int clubId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetClubDetailQuery 
                { 
                    ClubId = clubId,
                    ViewerUserId = userId
                };

                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Club Not Found",
                        Detail = "The specified club could not be found.",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You don't have permission to view this private club.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get club {ClubId} for user: {UserId}", clubId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Club",
                    Detail = "Unable to retrieve club details.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Join a club.
        /// Like signing up to become a member of a fitness group.
        /// </summary>
        /// <param name="clubId">Club ID to join</param>
        /// <param name="command">Join request details</param>
        /// <returns>Join confirmation or pending request details</returns>
        /// <response code="200">Joined club successfully</response>
        /// <response code="202">Join request submitted (for approval)</response>
        /// <response code="404">Club not found</response>
        /// <response code="409">Already a member or request pending</response>
        [HttpPost("{clubId}/join")]
        [ProducesResponseType(typeof(JoinClubResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JoinClubResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<JoinClubResponse>> JoinClub(int clubId, [FromBody] JoinClubCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;
                command.ClubId = clubId;

                _logger.LogInformation("User {UserId} attempting to join club {ClubId}", userId, clubId);

                var result = await _mediator.Send(command);

                if (result.RequiresApproval)
                {
                    return Accepted(result);
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Club Not Found",
                    Detail = "The specified club could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already member") || ex.Message.Contains("pending"))
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Already Member or Request Pending",
                    Detail = "You are already a member of this club or have a pending join request.",
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to join club {ClubId} for user: {UserId}", clubId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Join Club",
                    Detail = "Unable to join the club. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Leave a club.
        /// Like canceling your membership from a fitness group.
        /// </summary>
        /// <param name="clubId">Club ID to leave</param>
        /// <returns>Leave confirmation</returns>
        /// <response code="200">Left club successfully</response>
        /// <response code="404">Club not found or not a member</response>
        [HttpPost("{clubId}/leave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> LeaveClub(int clubId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var command = new LeaveClubCommand 
                { 
                    UserId = userId,
                    ClubId = clubId
                };

                await _mediator.Send(command);

                _logger.LogInformation("User {UserId} left club {ClubId}", userId, clubId);

                return Ok(new { message = "Left club successfully" });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found") || ex.Message.Contains("not a member"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Club Not Found or Not a Member",
                    Detail = "The specified club could not be found or you are not a member.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to leave club {ClubId} for user: {UserId}", clubId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Leave Club",
                    Detail = "Unable to leave the club. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get club members with their roles and activity.
        /// Like viewing the membership roster of a fitness group.
        /// </summary>
        /// <param name="clubId">Club ID</param>
        /// <param name="role">Filter by member role (all, member, admin, owner)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20, max: 100)</param>
        /// <returns>Club members</returns>
        /// <response code="200">Club members retrieved successfully</response>
        /// <response code="404">Club not found</response>
        /// <response code="403">Access denied</response>
        [HttpGet("{clubId}/members")]
        [ProducesResponseType(typeof(ClubMembersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ClubMembersResponse>> GetClubMembers(
            int clubId,
            [FromQuery] string role = "all",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetClubMembersQuery 
                { 
                    ClubId = clubId,
                    ViewerUserId = userId,
                    Role = role,
                    Page = Math.Max(1, page),
                    PageSize = Math.Min(100, Math.Max(1, pageSize))
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You don't have permission to view this club's members.");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Club Not Found",
                    Detail = "The specified club could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get members for club {ClubId}, user: {UserId}", clubId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Club Members",
                    Detail = "Unable to retrieve club members.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get club activity feed and recent events.
        /// Like reading the bulletin board of a fitness group to see what's been happening.
        /// </summary>
        /// <param name="clubId">Club ID</param>
        /// <param name="type">Filter by activity type (all, workout, challenge, event, announcement)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 20, max: 50)</param>
        /// <returns>Club activity feed</returns>
        /// <response code="200">Club activity retrieved successfully</response>
        /// <response code="404">Club not found</response>
        /// <response code="403">Access denied</response>
        [HttpGet("{clubId}/activity")]
        [ProducesResponseType(typeof(ClubActivityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ClubActivityResponse>> GetClubActivity(
            int clubId,
            [FromQuery] string type = "all",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetClubActivityQuery 
                { 
                    ClubId = clubId,
                    ViewerUserId = userId,
                    Type = type,
                    Page = Math.Max(1, page),
                    PageSize = Math.Min(50, Math.Max(1, pageSize))
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You don't have permission to view this club's activity.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity for club {ClubId}, user: {UserId}", clubId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Club Activity",
                    Detail = "Unable to retrieve club activity.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Create a club challenge.
        /// Like organizing a group fitness competition within your club.
        /// </summary>
        /// <param name="clubId">Club ID</param>
        /// <param name="command">Club challenge details</param>
        /// <returns>Created challenge information</returns>
        /// <response code="201">Club challenge created successfully</response>
        /// <response code="404">Club not found</response>
        /// <response code="403">Insufficient permissions</response>
        [HttpPost("{clubId}/challenges")]
        [ProducesResponseType(typeof(CreateClubChallengeResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CreateClubChallengeResponse>> CreateClubChallenge(
            int clubId, 
            [FromBody] CreateClubChallengeCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.CreatorUserId = userId;
                command.ClubId = clubId;

                _logger.LogInformation("Creating club challenge for club {ClubId} by user {UserId}", clubId, userId);

                var result = await _mediator.Send(command);

                return CreatedAtAction(
                    nameof(GetClubChallenge),
                    new { clubId, challengeId = result.ChallengeId },
                    result
                );
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You don't have permission to create challenges in this club.");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Club Not Found",
                    Detail = "The specified club could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create club challenge for club {ClubId}, user: {UserId}", clubId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Create Club Challenge",
                    Detail = "Unable to create the club challenge.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get club challenges.
        /// Like viewing the competition board of a fitness group.
        /// </summary>
        /// <param name="clubId">Club ID</param>
        /// <param name="status">Filter by challenge status (active, upcoming, completed, all)</param>
        /// <returns>Club challenges</returns>
        /// <response code="200">Club challenges retrieved successfully</response>
        /// <response code="404">Club not found</response>
        /// <response code="403">Access denied</response>
        [HttpGet("{clubId}/challenges")]
        [ProducesResponseType(typeof(ClubChallengesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ClubChallengesResponse>> GetClubChallenges(
            int clubId,
            [FromQuery] string status = "active")
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetClubChallengesQuery 
                { 
                    ClubId = clubId,
                    ViewerUserId = userId,
                    Status = status
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You don't have permission to view this club's challenges.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get challenges for club {ClubId}, user: {UserId}", clubId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Club Challenges",
                    Detail = "Unable to retrieve club challenges.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get details of a specific club challenge.
        /// Like examining the details of a group competition.
        /// </summary>
        /// <param name="clubId">Club ID</param>
        /// <param name="challengeId">Challenge ID</param>
        /// <returns>Club challenge details</returns>
        /// <response code="200">Club challenge retrieved successfully</response>
        /// <response code="404">Club or challenge not found</response>
        /// <response code="403">Access denied</response>
        [HttpGet("{clubId}/challenges/{challengeId}")]
        [ProducesResponseType(typeof(ClubChallengeDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ClubChallengeDetailResponse>> GetClubChallenge(int clubId, int challengeId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var query = new GetClubChallengeDetailQuery 
                { 
                    ClubId = clubId,
                    ChallengeId = challengeId,
                    ViewerUserId = userId
                };

                var result = await _mediator.Send(query);

                if (result == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Challenge Not Found",
                        Detail = "The specified club challenge could not be found.",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You don't have permission to view this club challenge.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get club challenge {ChallengeId} for club {ClubId}, user: {UserId}", 
                    challengeId, clubId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Get Club Challenge",
                    Detail = "Unable to retrieve club challenge details.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Update club information (admin/owner only).
        /// Like updating the description and rules posted on your fitness group's board.
        /// </summary>
        /// <param name="clubId">Club ID to update</param>
        /// <param name="command">Updated club information</param>
        /// <returns>Updated club details</returns>
        /// <response code="200">Club updated successfully</response>
        /// <response code="404">Club not found</response>
        /// <response code="403">Insufficient permissions</response>
        [HttpPut("{clubId}")]
        [ProducesResponseType(typeof(UpdateClubResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UpdateClubResponse>> UpdateClub(int clubId, [FromBody] UpdateClubCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.UserId = userId;
                command.ClubId = clubId;

                _logger.LogInformation("Updating club {ClubId} by user: {UserId}", clubId, userId);

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You don't have permission to update this club.");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Club Not Found",
                    Detail = "The specified club could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update club {ClubId} for user: {UserId}", clubId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Club Update Failed",
                    Detail = "Unable to update the club.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Manage club member roles and permissions (admin/owner only).
        /// Like appointing moderators or admins for your fitness group.
        /// </summary>
        /// <param name="clubId">Club ID</param>
        /// <param name="memberId">Member user ID</param>
        /// <param name="command">Role management details</param>
        /// <returns>Role update confirmation</returns>
        /// <response code="200">Member role updated successfully</response>
        /// <response code="404">Club or member not found</response>
        /// <response code="403">Insufficient permissions</response>
        [HttpPost("{clubId}/members/{memberId}/role")]
        [ProducesResponseType(typeof(UpdateMemberRoleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UpdateMemberRoleResponse>> UpdateMemberRole(
            int clubId, 
            int memberId, 
            [FromBody] UpdateMemberRoleCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.RequestingUserId = userId;
                command.ClubId = clubId;
                command.MemberUserId = memberId;

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You don't have permission to manage member roles in this club.");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Club or Member Not Found",
                    Detail = "The specified club or member could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update member role for club {ClubId}, member {MemberId}, user: {UserId}", 
                    clubId, memberId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Update Member Role",
                    Detail = "Unable to update member role.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Remove a member from the club (admin/owner only).
        /// Like asking someone to leave your fitness group.
        /// </summary>
        /// <param name="clubId">Club ID</param>
        /// <param name="memberId">Member user ID to remove</param>
        /// <param name="command">Removal details</param>
        /// <returns>Removal confirmation</returns>
        /// <response code="200">Member removed successfully</response>
        /// <response code="404">Club or member not found</response>
        /// <response code="403">Insufficient permissions</response>
        [HttpPost("{clubId}/members/{memberId}/remove")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> RemoveClubMember(
            int clubId, 
            int memberId, 
            [FromBody] RemoveClubMemberCommand command)
        {
            try
            {
                var userId = GetCurrentUserId();
                command.RequestingUserId = userId;
                command.ClubId = clubId;
                command.MemberUserId = memberId;

                await _mediator.Send(command);

                return Ok(new { message = "Member removed from club successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You don't have permission to remove members from this club.");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Club or Member Not Found",
                    Detail = "The specified club or member could not be found.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove member {MemberId} from club {ClubId} by user: {UserId}", 
                    memberId, clubId, GetCurrentUserId());
                return BadRequest(new ProblemDetails
                {
                    Title = "Failed to Remove Member",
                    Detail = "Unable to remove member from club.",
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

    // Request/Response DTOs for Clubs

    public class UserClubsResponse
    {
        public List<UserClubSummaryResponse> Clubs { get; set; } = new();
        public int TotalCount { get; set; }
        public Dictionary<string, int> ByRole { get; set; } = new();
        public Dictionary<string, int> ByCategory { get; set; } = new();
    }

    public class UserClubSummaryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public int MemberCount { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; }
        public int UnreadActivity { get; set; }
        public ClubActivitySummaryResponse? RecentActivity { get; set; }
    }

    public class ClubActivitySummaryResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? UserName { get; set; }
    }

    public class DiscoverClubsResponse
    {
        public List<ClubDiscoveryResponse> Clubs { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasMore { get; set; }
        public List<ClubCategoryResponse> PopularCategories { get; set; } = new();
        public List<ClubDiscoveryResponse> FeaturedClubs { get; set; } = new();
        public List<ClubDiscoveryResponse> RecommendedClubs { get; set; } = new();
    }

    public class ClubDiscoveryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public int MemberCount { get; set; }
        public bool IsJoined { get; set; }
        public bool HasPendingRequest { get; set; }
        public bool RequiresApproval { get; set; }
        public string? Location { get; set; }
        public List<string> Tags { get; set; } = new();
        public double ActivityLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFeatured { get; set; }
        public string? FeaturedReason { get; set; }
    }

    public class ClubCategoryResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public int ClubCount { get; set; }
        public int MemberCount { get; set; }
    }

    public class CreateClubResponse
    {
        public int ClubId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatorRole { get; set; } = "Owner";
        public string InviteCode { get; set; } = string.Empty;
        public string WelcomeMessage { get; set; } = string.Empty;
    }

    public class ClubDetailResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Location { get; set; }
        public List<string> Tags { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public ClubStatsResponse Stats { get; set; } = new();
        public ClubMembershipInfoResponse Membership { get; set; } = new();
        public List<ClubRuleResponse> Rules { get; set; } = new();
        public List<ClubOfficerResponse> Officers { get; set; } = new();
        public ClubActivitySummaryResponse? RecentActivity { get; set; }
        public bool CanJoin { get; set; }
        public bool RequiresApproval { get; set; }
        public string? JoinRequirement { get; set; }
    }

    public class ClubStatsResponse
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int TotalActivities { get; set; }
        public int ActiveChallenges { get; set; }
        public DateTime LastActivity { get; set; }
        public double ActivityLevel { get; set; }
        public Dictionary<string, int> MembersByLevel { get; set; } = new();
    }

    public class ClubMembershipInfoResponse
    {
        public bool IsMember { get; set; }
        public string? Role { get; set; }
        public DateTime? JoinedAt { get; set; }
        public bool HasPendingRequest { get; set; }
        public DateTime? RequestedAt { get; set; }
    }

    public class ClubRuleResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsRequired { get; set; }
    }

    public class ClubOfficerResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime AppointedAt { get; set; }
        public string? Bio { get; set; }
    }

    public class JoinClubResponse
    {
        public int ClubId { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public bool RequiresApproval { get; set; }
        public string Status { get; set; } = string.Empty; // Joined, Pending, Approved
        public DateTime ActionDate { get; set; }
        public string? WelcomeMessage { get; set; }
        public List<ClubRuleResponse> Rules { get; set; } = new();
    }

    public class ClubMembersResponse
    {
        public List<ClubMemberResponse> Members { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasMore { get; set; }
        public Dictionary<string, int> ByRole { get; set; } = new();
    }

    public class ClubMemberResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int Level { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public DateTime LastActiveDate { get; set; }
        public bool IsOnline { get; set; }
        public ClubMemberStatsResponse Stats { get; set; } = new();
        public List<string> RecentBadges { get; set; } = new();
    }

    public class ClubMemberStatsResponse
    {
        public int ClubActivities { get; set; }
        public int ChallengesCompleted { get; set; }
        public int ContributionPoints { get; set; }
        public string? SpecialTitle { get; set; }
    }

    public class ClubActivityResponse
    {
        public List<ClubActivityItemResponse> Activities { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasMore { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class ClubActivityItemResponse
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public ClubActivityUserResponse User { get; set; } = new();
        public Dictionary<string, object> Data { get; set; } = new();
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool HasLiked { get; set; }
        public List<ClubActivityCommentResponse> TopComments { get; set; } = new();
    }

    public class ClubActivityUserResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    public class ClubActivityCommentResponse
    {
        public int Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public ClubActivityUserResponse User { get; set; } = new();
    }

    public class CreateClubChallengeResponse
    {
        public int ChallengeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoEnrollMembers { get; set; }
        public int InitialParticipants { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ClubChallengesResponse
    {
        public List<ClubChallengeResponse> Challenges { get; set; } = new();
        public int TotalCount { get; set; }
        public Dictionary<string, int> ByStatus { get; set; } = new();
        public ClubChallengeResponse? FeaturedChallenge { get; set; }
    }

    public class ClubChallengeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ParticipantCount { get; set; }
        public bool IsParticipating { get; set; }
        public string? UserRank { get; set; }
        public double? UserProgress { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
        public Dictionary<string, object> Rules { get; set; } = new();
    }

    public class ClubChallengeDetailResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public ClubActivityUserResponse Creator { get; set; } = new();
        public Dictionary<string, object> Rules { get; set; } = new();
        public List<ClubChallengeParticipantResponse> Leaderboard { get; set; } = new();
        public ClubChallengeParticipantResponse? UserParticipation { get; set; }
        public bool CanJoin { get; set; }
        public bool CanLeave { get; set; }
        public ClubChallengeStatsResponse Stats { get; set; } = new();
    }

    public class ClubChallengeParticipantResponse
    {
        public int Rank { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public double Progress { get; set; }
        public string ProgressUnit { get; set; } = string.Empty;
        public DateTime LastActivityDate { get; set; }
        public bool IsCurrentUser { get; set; }
        public List<string> RecentAchievements { get; set; } = new();
    }

    public class ClubChallengeStatsResponse
    {
        public int TotalParticipants { get; set; }
        public int ActiveParticipants { get; set; }
        public double AverageProgress { get; set; }
        public string TopPerformer { get; set; } = string.Empty;
        public int DaysRemaining { get; set; }
        public double CompletionRate { get; set; }
    }

    public class UpdateClubResponse
    {
        public int ClubId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public List<string> UpdatedFields { get; set; } = new();
    }

    public class UpdateMemberRoleResponse
    {
        public int ClubId { get; set; }
        public int MemberUserId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string PreviousRole { get; set; } = string.Empty;
        public string NewRole { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }

    // Placeholder command and query classes (to be implemented in Application layer)
    public class GetUserClubsQuery : IRequest<UserClubsResponse>
    {
        public int UserId { get; set; }
        public string Role { get; set; } = "all";
    }

    public class DiscoverClubsQuery : IRequest<DiscoverClubsResponse>
    {
        public int UserId { get; set; }
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
        public string? Location { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class CreateClubCommand : IRequest<CreateClubResponse>
    {
        public int CreatorUserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Public, Private, InviteOnly
        public string? Location { get; set; }
        public List<string> Tags { get; set; } = new();
        public bool RequireApprovalToJoin { get; set; }
        public string? WelcomeMessage { get; set; }
        public List<CreateClubRuleRequest> Rules { get; set; } = new();
    }

    public class CreateClubRuleRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsRequired { get; set; }
    }

    public class GetClubDetailQuery : IRequest<ClubDetailResponse?>
    {
        public int ClubId { get; set; }
        public int ViewerUserId { get; set; }
    }

    public class JoinClubCommand : IRequest<JoinClubResponse>
    {
        public int UserId { get; set; }
        public int ClubId { get; set; }
        public string? JoinMessage { get; set; }
        public string? InviteCode { get; set; }
    }

    public class LeaveClubCommand : IRequest
    {
        public int UserId { get; set; }
        public int ClubId { get; set; }
        public string? Reason { get; set; }
    }

    public class GetClubMembersQuery : IRequest<ClubMembersResponse>
    {
        public int ClubId { get; set; }
        public int ViewerUserId { get; set; }
        public string Role { get; set; } = "all";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetClubActivityQuery : IRequest<ClubActivityResponse>
    {
        public int ClubId { get; set; }
        public int ViewerUserId { get; set; }
        public string Type { get; set; } = "all";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class CreateClubChallengeCommand : IRequest<CreateClubChallengeResponse>
    {
        public int CreatorUserId { get; set; }
        public int ClubId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoEnrollMembers { get; set; }
        public Dictionary<string, object> Rules { get; set; } = new();
        public List<string> EligibleRoles { get; set; } = new();
    }

    public class GetClubChallengesQuery : IRequest<ClubChallengesResponse>
    {
        public int ClubId { get; set; }
        public int ViewerUserId { get; set; }
        public string Status { get; set; } = "active";
    }

    public class GetClubChallengeDetailQuery : IRequest<ClubChallengeDetailResponse?>
    {
        public int ClubId { get; set; }
        public int ChallengeId { get; set; }
        public int ViewerUserId { get; set; }
    }

    public class UpdateClubCommand : IRequest<UpdateClubResponse>
    {
        public int UserId { get; set; }
        public int ClubId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
        public string? Location { get; set; }
        public List<string>? Tags { get; set; }
        public bool? RequireApprovalToJoin { get; set; }
        public string? WelcomeMessage { get; set; }
        public List<CreateClubRuleRequest>? Rules { get; set; }
    }

    public class UpdateMemberRoleCommand : IRequest<UpdateMemberRoleResponse>
    {
        public int RequestingUserId { get; set; }
        public int ClubId { get; set; }
        public int MemberUserId { get; set; }
        public string NewRole { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }

    public class RemoveClubMemberCommand : IRequest
    {
        public int RequestingUserId { get; set; }
        public int ClubId { get; set; }
        public int MemberUserId { get; set; }
        public string? Reason { get; set; }
        public bool BanUser { get; set; } = false;
    }
}
