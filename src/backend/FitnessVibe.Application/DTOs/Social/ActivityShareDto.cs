using FitnessVibe.Domain.Enums;
using System;
using System.Collections.Generic;

namespace FitnessVibe.Application.DTOs.Social;

/// <summary>
/// DTO for activity share responses
/// </summary>
public class ActivityShareDto
{
    /// <summary>
    /// ID of the share
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the user who shared
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Username of the sharer
    /// </summary>
    public string? UserUsername { get; set; }

    /// <summary>
    /// Profile picture URL of the sharer
    /// </summary>
    public string? UserProfilePicture { get; set; }

    /// <summary>
    /// ID of the shared activity
    /// </summary>
    public Guid ActivityId { get; set; }

    /// <summary>
    /// Title of the shared activity
    /// </summary>
    public string? ActivityTitle { get; set; }

    /// <summary>
    /// Type of the shared activity
    /// </summary>
    public ActivityType ActivityType { get; set; }

    /// <summary>
    /// Share caption
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    /// Privacy setting
    /// </summary>
    public SharePrivacy Privacy { get; set; }

    /// <summary>
    /// When it was shared
    /// </summary>
    public DateTime SharedAt { get; set; }

    /// <summary>
    /// Total number of likes
    /// </summary>
    public int LikesCount { get; set; }

    /// <summary>
    /// Whether the current user has liked this share
    /// </summary>
    public bool IsLikedByCurrentUser { get; set; }

    /// <summary>
    /// Total number of comments
    /// </summary>
    public int CommentsCount { get; set; }

    /// <summary>
    /// Recent comments on this share
    /// </summary>
    public List<ActivityCommentDto>? RecentComments { get; set; }
}
