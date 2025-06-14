using System;
using MediatR;
using FitnessVibe.Application.DTOs.Challenges;

namespace FitnessVibe.Application.Queries.Challenges
{
    /// <summary>
    /// Query to get challenges for a specific user
    /// </summary>
    public class GetUserChallengesQuery : IRequest<IEnumerable<ChallengeResponse>>
    {
        /// <summary>
        /// ID of the user
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Filter for only active challenges
        /// </summary>
        public bool? OnlyActive { get; set; }

        /// <summary>
        /// Filter for only completed challenges
        /// </summary>
        public bool? OnlyCompleted { get; set; }

        /// <summary>
        /// Include challenges created by the user
        /// </summary>
        public bool IncludeCreated { get; set; } = false;

        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Items per page
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Sort field
        /// </summary>
        public string SortBy { get; set; } = "StartDate";

        /// <summary>
        /// Sort direction
        /// </summary>
        public string SortDirection { get; set; } = "DESC";
    }
}
