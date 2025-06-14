using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;
using System;

namespace FitnessVibe.Domain.Entities.Social;

/// <summary>
/// Represents a like on a shared activity
/// </summary>
public class ActivityLike : BaseEntity, IUserOwnedEntity
{
    /// <summary>
    /// ID of the user who liked the activity
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// ID of the shared activity that was liked
    /// </summary>
    public Guid ActivityShareId { get; private set; }

    /// <summary>
    /// When the like was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public User? User { get; private set; }

    /// <summary>
    /// Navigation property to the shared activity
    /// </summary>
    public ActivityShare? ActivityShare { get; private set; }

    private ActivityLike() { }

    /// <summary>
    /// Creates a new activity like
    /// </summary>
    /// <param name="userId">ID of the user liking the activity</param>
    /// <param name="activityShareId">ID of the shared activity being liked</param>
    /// <returns>A new ActivityLike instance</returns>
    public static ActivityLike Create(Guid userId, Guid activityShareId)
    {
        return new ActivityLike
        {
            UserId = userId,
            ActivityShareId = activityShareId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
