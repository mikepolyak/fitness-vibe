using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;
using System;

namespace FitnessVibe.Domain.Entities.Social;

/// <summary>
/// Represents a comment on a shared activity
/// </summary>
public class ActivityComment : BaseEntity, IUserOwnedEntity
{
    /// <summary>
    /// ID of the user who made the comment
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// ID of the shared activity being commented on
    /// </summary>
    public Guid ActivityShareId { get; private set; }

    /// <summary>
    /// The comment text
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public User? User { get; private set; }

    /// <summary>
    /// Navigation property to the shared activity
    /// </summary>
    public ActivityShare? ActivityShare { get; private set; }

    private ActivityComment() { }

    /// <summary>
    /// Creates a new activity comment
    /// </summary>
    /// <param name="userId">ID of the user making the comment</param>
    /// <param name="activityShareId">ID of the shared activity being commented on</param>
    /// <param name="content">The comment text</param>
    /// <returns>A new ActivityComment instance</returns>
    public static ActivityComment Create(Guid userId, Guid activityShareId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Comment content cannot be empty", nameof(content));
        }

        return new ActivityComment
        {
            UserId = userId,
            ActivityShareId = activityShareId,
            Content = content
        };
    }

    /// <summary>
    /// Updates the comment content
    /// </summary>
    /// <param name="content">The new comment text</param>
    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Comment content cannot be empty", nameof(content));
        }

        Content = content;
    }
}
