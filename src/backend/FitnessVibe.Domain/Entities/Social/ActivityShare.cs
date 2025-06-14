using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.Entities.Users;
using System;
using System.Collections.Generic;

namespace FitnessVibe.Domain.Entities.Social;

/// <summary>
/// Represents a shared activity post with social interactions
/// </summary>
public class ActivityShare : BaseEntity, IUserOwnedEntity
{
    /// <summary>
    /// ID of the user who shared the activity
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// ID of the activity being shared
    /// </summary>
    public Guid ActivityId { get; private set; }

    /// <summary>
    /// Optional caption for the shared activity
    /// </summary>
    public string Caption { get; private set; }

    /// <summary>
    /// Privacy setting for the share
    /// </summary>
    public SharePrivacy Privacy { get; private set; }

    /// <summary>
    /// When the activity was shared
    /// </summary>
    public DateTime SharedAt { get; private set; }

    /// <summary>
    /// Collection of likes on this shared activity
    /// </summary>
    public ICollection<ActivityLike> Likes { get; private set; } = new List<ActivityLike>();

    /// <summary>
    /// Collection of comments on this shared activity
    /// </summary>
    public ICollection<ActivityComment> Comments { get; private set; } = new List<ActivityComment>();

    /// <summary>
    /// Navigation property to the user who shared
    /// </summary>
    public User User { get; private set; }

    /// <summary>
    /// Navigation property to the activity
    /// </summary>
    public Activity Activity { get; private set; }

    private ActivityShare() { }

    public static ActivityShare Create(Guid userId, Guid activityId, string caption, SharePrivacy privacy)
    {
        return new ActivityShare
        {
            UserId = userId,
            ActivityId = activityId,
            Caption = caption,
            Privacy = privacy,
            SharedAt = DateTime.UtcNow
        };
    }

    public void UpdatePrivacy(SharePrivacy privacy)
    {
        Privacy = privacy;
    }

    public void UpdateCaption(string caption)
    {
        Caption = caption;
    }
}
