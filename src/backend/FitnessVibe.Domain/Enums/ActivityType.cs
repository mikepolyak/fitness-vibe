namespace FitnessVibe.Domain.Enums
{
    /// <summary>
    /// Defines the type of activity based on where/how it's performed
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        /// Activities performed indoors (gym, home, etc.)
        /// </summary>
        Indoor = 1,

        /// <summary>
        /// Activities performed outdoors
        /// </summary>
        Outdoor = 2,

        /// <summary>
        /// Virtual/online guided activities
        /// </summary>
        Virtual = 3,

        /// <summary>
        /// Manually tracked activities
        /// </summary>
        Manual = 4,

        /// <summary>
        /// Team sports activities
        /// </summary>
        TeamSport = 5,

        /// <summary>
        /// Activities that combine multiple types
        /// </summary>
        Hybrid = 6
    }
}
