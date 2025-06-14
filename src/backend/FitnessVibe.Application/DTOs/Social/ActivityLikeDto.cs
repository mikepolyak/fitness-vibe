using System;

namespace FitnessVibe.Application.DTOs.Social;

/// <summary>
/// DTO for activity like responses
/// </summary>
public class ActivityLikeDto
{
    /// <summary>
    /// ID of the like
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the user who liked
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Username of the user who liked
    /// </summary>
    public string? UserUsername { get; set; }

    /// <summary>
    /// Profile picture URL of the user who liked
    /// </summary>
    public string? UserProfilePicture { get; set; }

    /// <summary>
    /// When the like was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
