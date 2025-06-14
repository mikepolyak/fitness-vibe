using System;

namespace FitnessVibe.Application.DTOs.Social;

/// <summary>
/// DTO for activity comment responses
/// </summary>
public class ActivityCommentDto
{
    /// <summary>
    /// ID of the comment
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the user who commented
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Username of the commenter
    /// </summary>
    public string? UserUsername { get; set; }

    /// <summary>
    /// Profile picture URL of the commenter
    /// </summary>
    public string? UserProfilePicture { get; set; }

    /// <summary>
    /// The comment text
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// When the comment was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the comment was last modified
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }
}
