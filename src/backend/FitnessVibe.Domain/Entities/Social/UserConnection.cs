using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;
using System;

namespace FitnessVibe.Domain.Entities.Social;

/// <summary>
/// Represents a social connection (follow/friend) between two users
/// </summary>
public class UserConnection : BaseEntity
{
    /// <summary>
    /// ID of the user who initiated the connection (follower)
    /// </summary>
    public Guid FollowerId { get; private set; }

    /// <summary>
    /// ID of the user being followed
    /// </summary>
    public Guid FollowedId { get; private set; }

    /// <summary>
    /// When the connection was established
    /// </summary>
    public DateTime ConnectedAt { get; private set; }

    /// <summary>
    /// Navigation property to the follower user
    /// </summary>
    public User Follower { get; private set; }

    /// <summary>
    /// Navigation property to the followed user
    /// </summary>
    public User Followed { get; private set; }

    private UserConnection() { }

    public static UserConnection Create(Guid followerId, Guid followedId)
    {
        return new UserConnection
        {
            FollowerId = followerId,
            FollowedId = followedId,
            ConnectedAt = DateTime.UtcNow
        };
    }
}
