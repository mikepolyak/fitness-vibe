using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace FitnessVibe.Application.Commands.Challenges
{
    /// <summary>
    /// Command to update progress in a challenge
    /// </summary>
    public class UpdateChallengeProgressCommand : IRequest<UpdateChallengeProgressResponse>
    {
        /// <summary>
        /// ID of the challenge
        /// </summary>
        [Required]
        public Guid ChallengeId { get; set; }

        /// <summary>
        /// ID of the user
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// New total progress value
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Progress { get; set; }
    }

    /// <summary>
    /// Response from updating challenge progress
    /// </summary>
    public class UpdateChallengeProgressResponse
    {
        /// <summary>
        /// Current progress value
        /// </summary>
        public decimal Progress { get; set; }

        /// <summary>
        /// Target value
        /// </summary>
        public decimal TargetValue { get; set; }

        /// <summary>
        /// Progress percentage (0-100)
        /// </summary>
        public decimal ProgressPercentage { get; set; }

        /// <summary>
        /// Whether the challenge is completed
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// When the update was made
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
