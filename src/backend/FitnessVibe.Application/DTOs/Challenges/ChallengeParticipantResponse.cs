namespace FitnessVibe.Application.DTOs.Challenges
{
    /// <summary>
    /// DTO for challenge participant information
    /// </summary>
    public class ChallengeParticipantResponse
    {
        /// <summary>
        /// ID of the participant
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the challenge
        /// </summary>
        public Guid ChallengeId { get; set; }

        /// <summary>
        /// ID of the user
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Name of the user
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// When the user joined
        /// </summary>
        public DateTime JoinedAt { get; set; }

        /// <summary>
        /// Current progress value
        /// </summary>
        public decimal Progress { get; set; }

        /// <summary>
        /// Progress percentage (0-100)
        /// </summary>
        public decimal ProgressPercentage { get; set; }

        /// <summary>
        /// Rank in the challenge
        /// </summary>
        public int? Rank { get; set; }

        /// <summary>
        /// Whether the user has completed the challenge
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// When the user completed the challenge
        /// </summary>
        public DateTime? CompletedAt { get; set; }
    }
}
