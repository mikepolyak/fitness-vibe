using System;
using MediatR;
using FitnessVibe.Application.Common;

namespace FitnessVibe.Application.Commands.Social;

/// <summary>
/// Command to like a shared activity.
/// Think of this as giving a virtual high-five to someone's workout achievement!
/// </summary>
public class LikeActivityCommand : IRequest<LikeActivityResponse>, IUserOwnedEntity
{
    /// <summary>
    /// ID of the user giving the like
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// ID of the shared activity being liked
    /// </summary>
    public Guid ActivityShareId { get; set; }
}

/// <summary>
/// Response for liking a shared activity
/// </summary>
public class LikeActivityResponse
{
    /// <summary>
    /// ID of the newly created like
    /// </summary>
    public Guid LikeId { get; set; }

    /// <summary>
    /// When the like was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Current total number of likes for this activity
    /// </summary>
    public int TotalLikes { get; set; }
}
