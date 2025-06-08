namespace FitnessVibe.Domain.Enums
{
    /// <summary>
    /// Defines the category of activity based on its primary focus/benefit
    /// </summary>
    public enum ActivityCategory
    {
        /// <summary>
        /// Activities focused on cardiovascular fitness
        /// </summary>
        Cardio = 1,

        /// <summary>
        /// Activities focused on building strength
        /// </summary>
        Strength = 2,

        /// <summary>
        /// Activities focused on improving flexibility
        /// </summary>
        Flexibility = 3,

        /// <summary>
        /// Activities that combine multiple fitness aspects
        /// </summary>
        CrossTraining = 4,

        /// <summary>
        /// Activities focused on balance and coordination
        /// </summary>
        Balance = 5,

        /// <summary>
        /// Activities focused on endurance
        /// </summary>
        Endurance = 6,

        /// <summary>
        /// Activities focused on recovery and wellness
        /// </summary>
        Recovery = 7,

        /// <summary>
        /// Activities focused on sport-specific training
        /// </summary>
        Sport = 8,

        /// <summary>
        /// Activities focused on mindfulness and mental wellness
        /// </summary>
        MindBody = 9
    }
}
