namespace FitnessVibe.Domain.Enums
{
    /// <summary>
    /// Represents the current status of an activity
    /// </summary>
    public enum ActivityStatus
    {
        /// <summary>
        /// Activity is created but not yet started
        /// </summary>
        Created = 0,

        /// <summary>
        /// Activity is currently in progress
        /// </summary>
        Active = 1,

        /// <summary>
        /// Activity is temporarily paused
        /// </summary>
        Paused = 2,

        /// <summary>
        /// Activity has been completed
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Activity was cancelled before completion
        /// </summary>
        Cancelled = 4
    }
}
