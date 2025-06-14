namespace FitnessVibe.Domain.Enums
{
    /// <summary>
    /// Types of challenges that can be created
    /// </summary>
    public enum ChallengeType
    {
        /// <summary>
        /// Challenge based on total distance covered
        /// </summary>
        Distance = 0,

        /// <summary>
        /// Challenge based on total calories burned
        /// </summary>
        Calories = 1,

        /// <summary>
        /// Challenge based on number of activities completed
        /// </summary>
        ActivityCount = 2,

        /// <summary>
        /// Challenge based on total active minutes
        /// </summary>
        Duration = 3,

        /// <summary>
        /// Challenge based on achieving specific milestones
        /// </summary>
        Milestone = 4,

        /// <summary>
        /// Challenge based on streak of consecutive days with activity
        /// </summary>
        Streak = 5,

        /// <summary>
        /// Challenge based on improvement in performance metrics
        /// </summary>
        Improvement = 6,

        /// <summary>
        /// Custom challenge with user-defined criteria
        /// </summary>
        Custom = 7
    }
}
