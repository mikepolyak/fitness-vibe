namespace FitnessVibe.Application.Common
{
    /// <summary>
    /// Interface for entities/commands that are owned by a user.
    /// </summary>
    public interface IUserOwnedEntity
    {
        /// <summary>
        /// Gets or sets the user ID who owns/initiates the action.
        /// </summary>
        Guid UserId { get; set; }
    }
}
