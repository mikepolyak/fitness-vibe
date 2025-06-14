using System;
using MediatR;
using FitnessVibe.Application.DTOs.Challenges;

namespace FitnessVibe.Application.Queries.Challenges
{
    /// <summary>
    /// Query to get details of a specific challenge
    /// </summary>
    public class GetChallengeQuery : IRequest<ChallengeResponse>
    {
        /// <summary>
        /// ID of the challenge to retrieve
        /// </summary>
        public Guid ChallengeId { get; set; }

        /// <summary>
        /// ID of the user making the request (for participation info)
        /// </summary>
        public Guid? UserId { get; set; }
    }
}
