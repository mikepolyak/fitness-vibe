using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace FitnessVibe.Application.Commands.Challenges
{
    /// <summary>
    /// Command to activate a challenge
    /// </summary>
    public class ActivateChallengeCommand : IRequest<ActivateChallengeResponse>
    {
        /// <summary>
        /// ID of the challenge to activate
        /// </summary>
        [Required]
        public Guid ChallengeId { get; set; }

        /// <summary>
        /// ID of the user activating the challenge
        /// </summary>
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Response from activating a challenge
    /// </summary>
    public class ActivateChallengeResponse
    {
        /// <summary>
        /// ID of the challenge activated
        /// </summary>
        public Guid ChallengeId { get; set; }

        /// <summary>
        /// When the challenge was activated
        /// </summary>
        public DateTime ActivatedAt { get; set; }

        /// <summary>
        /// Number of current participants
        /// </summary>
        public int ParticipantCount { get; set; }
    }
}
