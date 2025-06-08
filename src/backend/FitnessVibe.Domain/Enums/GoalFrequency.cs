namespace FitnessVibe.Domain.Enums
{
    /// <summary>
    /// How often a goal should be evaluated or measured
    /// </summary>
    public enum GoalFrequency
    {
        /// <summary>
        /// Goal is evaluated once per day
        /// </summary>
        Daily = 1,

        /// <summary>
        /// Goal is evaluated once per week
        /// </summary>
        Weekly = 2,

        /// <summary>
        /// Goal is evaluated once per month
        /// </summary>
        Monthly = 3,

        /// <summary>
        /// Goal is evaluated at a specific date
        /// </summary>
        OneTime = 4,

        /// <summary>
        /// Custom frequency or timing
        /// </summary>
        Custom = 5
    }
}
