namespace FitnessVibe.Domain.Enums;

/// <summary>
/// Represents the privacy level for shared content
/// </summary>
public enum SharePrivacy
{
    /// <summary>
    /// Visible to everyone
    /// </summary>
    Public = 0,

    /// <summary>
    /// Visible only to followers
    /// </summary>
    FollowersOnly = 1,

    /// <summary>
    /// Visible only to specified users
    /// </summary>
    Private = 2
}
