using FitnessVibe.Domain.Entities.Challenges;
using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Application.DTOs.Challenges
{
    /// <summary>
    /// DTO for challenge information
    /// </summary>
    public class ChallengeResponse
    {
        /// <summary>
        /// Unique identifier of the challenge
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the challenge
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description of the challenge
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// When the challenge starts
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// When the challenge ends (if applicable)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Type of challenge
        /// </summary>
        public ChallengeType Type { get; set; }

        /// <summary>
        /// Target value to achieve
        /// </summary>
        public decimal TargetValue { get; set; }

        /// <summary>
        /// Unit of measurement
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Type of activity (if applicable)
        /// </summary>
        public ActivityType? ActivityType { get; set; }

        /// <summary>
        /// Whether the challenge is private
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Whether the challenge is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// ID of the user who created the challenge
        /// </summary>
        public Guid CreatedById { get; set; }

        /// <summary>
        /// Name of the user who created the challenge
        /// </summary>
        public string CreatedByName { get; set; } = string.Empty;

        /// <summary>
        /// When the challenge was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Number of current participants
        /// </summary>
        public int ParticipantCount { get; set; }

        /// <summary>
        /// List of top participants
        /// </summary>
        public List<ChallengeParticipantResponse> TopParticipants { get; set; } = new();

        /// <summary>
        /// Current user's participation info (if participating)
        /// </summary>
        public ChallengeParticipantResponse? UserParticipation { get; set; }
    }
}
