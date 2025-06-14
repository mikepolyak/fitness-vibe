using System;

namespace FitnessVibe.Application.DTOs.Social;

/// <summary>
/// DTO for user connection responses
/// </summary>
public class UserConnectionDto
{
    /// <summary>
    /// ID of the connection
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the follower user
    /// </summary>
    public Guid FollowerId { get; set; }

    /// <summary>
    /// Username of the follower
    /// </summary>
    public string? FollowerUsername { get; set; }

    /// <summary>
    /// Profile picture URL of the follower
    /// </summary>
    public string? FollowerProfilePicture { get; set; }

    /// <summary>
    /// ID of the followed user
    /// </summary>
    public Guid FollowedId { get; set; }

    /// <summary>
    /// Username of the followed user
    /// </summary>
    public string? FollowedUsername { get; set; }

    /// <summary>
    /// Profile picture URL of the followed user
    /// </summary>
    public string? FollowedProfilePicture { get; set; }

    /// <summary>
    /// When the connection was established
    /// </summary>
    public DateTime ConnectedAt { get; set; }
}
