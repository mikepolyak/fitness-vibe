using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace FitnessVibe.Application.Commands.Challenges
{
    /// <summary>
    /// Command to join a challenge
    /// </summary>
    public class JoinChallengeCommand : IRequest<JoinChallengeResponse>
    {
        /// <summary>
        /// ID of the challenge to join
        /// </summary>
        [Required]
        public Guid ChallengeId { get; set; }

        /// <summary>
        /// ID of the user joining
        /// </summary>
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Response from joining a challenge
    /// </summary>
    public class JoinChallengeResponse
    {
        /// <summary>
        /// ID of the challenge joined
        /// </summary>
        public Guid ChallengeId { get; set; }

        /// <summary>
        /// When the user joined
        /// </summary>
        public DateTime JoinedAt { get; set; }

        /// <summary>
        /// Target to achieve
        /// </summary>
        public decimal TargetValue { get; set; }

        /// <summary>
        /// Unit of measurement
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// When the challenge ends (if applicable)
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
