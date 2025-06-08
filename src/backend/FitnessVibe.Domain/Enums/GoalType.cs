namespace FitnessVibe.Domain.Enums
{
    /// <summary>
    /// The type of goal a user is working towards
    /// </summary>
    public enum GoalType
    {
        /// <summary>
        /// Distance-based goals (e.g., run 5km)
        /// </summary>
        Distance = 1,

        /// <summary>
        /// Duration-based goals (e.g., exercise for 30 minutes)
        /// </summary>
        Duration = 2,

        /// <summary>
        /// Frequency-based goals (e.g., work out 3 times per week)
        /// </summary>
        Frequency = 3,

        /// <summary>
        /// Custom numeric goals (e.g., burn 500 calories)
        /// </summary>
        Numeric = 4,

        /// <summary>
        /// Boolean goals (e.g., try a new workout class)
        /// </summary>
        Completion = 5
    }
}
