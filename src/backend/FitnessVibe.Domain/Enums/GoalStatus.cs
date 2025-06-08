namespace FitnessVibe.Domain.Enums
{
    /// <summary>
    /// The current status of a user's goal
    /// </summary>
    public enum GoalStatus
    {
        /// <summary>
        /// Goal has been created but not yet started
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Goal is currently active and being tracked
        /// </summary>
        Active = 2,

        /// <summary>
        /// Goal has been completed successfully
        /// </summary>
        Completed = 3,

        /// <summary>
        /// User abandoned or cancelled the goal
        /// </summary>
        Abandoned = 4,

        /// <summary>
        /// Goal end date passed without completion
        /// </summary>
        Failed = 5,

        /// <summary>
        /// Goal has been paused temporarily
        /// </summary>
        Paused = 6
    }
}
