using FitnessVibe.Domain.Entities.Social;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessVibe.Domain.Repositories;

/// <summary>
/// Repository interface for social feature operations
/// </summary>
public interface ISocialRepository
{
    /// <summary>
    /// Creates a new user connection (follow)
    /// </summary>
    Task<UserConnection> CreateConnectionAsync(UserConnection connection);

    /// <summary>
    /// Removes a user connection (unfollow)
    /// </summary>
    Task RemoveConnectionAsync(Guid followerId, Guid followedId);

    /// <summary>
    /// Checks if a connection exists between two users
    /// </summary>
    Task<bool> ConnectionExistsAsync(Guid followerId, Guid followedId);

    /// <summary>
    /// Gets all followers for a user
    /// </summary>
    Task<IEnumerable<UserConnection>> GetFollowersAsync(Guid userId);

    /// <summary>
    /// Gets all users that a user follows
    /// </summary>
    Task<IEnumerable<UserConnection>> GetFollowingAsync(Guid userId);

    /// <summary>
    /// Shares an activity
    /// </summary>
    Task<ActivityShare> ShareActivityAsync(ActivityShare share);

    /// <summary>
    /// Updates an activity share
    /// </summary>
    Task<ActivityShare> UpdateShareAsync(ActivityShare share);

    /// <summary>
    /// Removes an activity share
    /// </summary>
    Task DeleteShareAsync(Guid shareId);

    /// <summary>
    /// Gets a shared activity by ID
    /// </summary>
    Task<ActivityShare?> GetShareByIdAsync(Guid shareId);

    /// <summary>
    /// Gets all shares for a user's feed
    /// </summary>
    Task<IEnumerable<ActivityShare>> GetFeedAsync(Guid userId, int skip = 0, int take = 20);

    /// <summary>
    /// Gets all shares by a specific user
    /// </summary>
    Task<IEnumerable<ActivityShare>> GetUserSharesAsync(Guid userId, int skip = 0, int take = 20);

    /// <summary>
    /// Likes a shared activity
    /// </summary>
    Task<ActivityLike> LikeActivityAsync(ActivityLike like);

    /// <summary>
    /// Removes a like from a shared activity
    /// </summary>
    Task UnlikeActivityAsync(Guid userId, Guid shareId);

    /// <summary>
    /// Gets likes for a shared activity
    /// </summary>
    Task<IEnumerable<ActivityLike>> GetShareLikesAsync(Guid shareId);

    /// <summary>
    /// Adds a comment to a shared activity
    /// </summary>
    Task<ActivityComment> AddCommentAsync(ActivityComment comment);

    /// <summary>
    /// Updates a comment
    /// </summary>
    Task<ActivityComment> UpdateCommentAsync(ActivityComment comment);

    /// <summary>
    /// Deletes a comment
    /// </summary>
    Task DeleteCommentAsync(Guid commentId);

    /// <summary>
    /// Gets comments for a shared activity
    /// </summary>
    Task<IEnumerable<ActivityComment>> GetShareCommentsAsync(Guid shareId, int skip = 0, int take = 20);
}
