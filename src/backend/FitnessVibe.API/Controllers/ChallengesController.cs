using FitnessVibe.Application.Commands.Challenges;
using FitnessVibe.Application.DTOs.Challenges;
using FitnessVibe.Application.Queries.Challenges;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessVibe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChallengesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChallengesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new challenge
    /// </summary>
    /// <param name="command">The challenge creation command</param>
    /// <returns>The created challenge details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ChallengeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateChallenge(CreateChallengeCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific challenge by ID
    /// </summary>
    /// <param name="id">The challenge ID</param>
    /// <returns>The challenge details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ChallengeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChallenge(Guid id)
    {
        var result = await _mediator.Send(new GetChallengeQuery { Id = id });
        return Ok(result);
    }

    /// <summary>
    /// Searches for challenges based on criteria
    /// </summary>
    /// <param name="query">The search query parameters</param>
    /// <returns>A list of matching challenges</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ChallengeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchChallenges([FromQuery] SearchChallengesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets all challenges for the current user
    /// </summary>
    /// <returns>A list of challenges the user is participating in</returns>
    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<ChallengeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserChallenges()
    {
        var result = await _mediator.Send(new GetUserChallengesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Join an existing challenge
    /// </summary>
    /// <param name="id">The challenge ID</param>
    /// <returns>The challenge participant details</returns>
    [HttpPost("{id}/join")]
    [ProducesResponseType(typeof(ChallengeParticipantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> JoinChallenge(Guid id)
    {
        var result = await _mediator.Send(new JoinChallengeCommand { ChallengeId = id });
        return Ok(result);
    }

    /// <summary>
    /// Updates the progress for a challenge participant
    /// </summary>
    /// <param name="id">The challenge ID</param>
    /// <param name="command">The progress update command</param>
    /// <returns>The updated challenge participant details</returns>
    [HttpPut("{id}/progress")]
    [ProducesResponseType(typeof(ChallengeParticipantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProgress(Guid id, UpdateChallengeProgressCommand command)
    {
        if (id != command.ChallengeId)
        {
            return BadRequest("Challenge ID mismatch");
        }
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Activates a challenge that is in draft state
    /// </summary>
    /// <param name="id">The challenge ID</param>
    /// <returns>The activated challenge details</returns>
    [HttpPost("{id}/activate")]
    [ProducesResponseType(typeof(ChallengeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActivateChallenge(Guid id)
    {
        var result = await _mediator.Send(new ActivateChallengeCommand { ChallengeId = id });
        return Ok(result);
    }
}
