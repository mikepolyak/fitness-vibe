using System;
using MediatR;
using FitnessVibe.Domain.Enums;
using FitnessVibe.Application.DTOs.Challenges;

namespace FitnessVibe.Application.Queries.Challenges
{
    /// <summary>
    /// Query to search for challenges
    /// </summary>
    public class SearchChallengesQuery : IRequest<IEnumerable<ChallengeResponse>>
    {
        /// <summary>
        /// Filter for active/inactive challenges
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Filter for private/public challenges
        /// </summary>
        public bool? IsPrivate { get; set; }

        /// <summary>
        /// Filter by start date range from
        /// </summary>
        public DateTime? StartDateFrom { get; set; }

        /// <summary>
        /// Filter by start date range to
        /// </summary>
        public DateTime? StartDateTo { get; set; }

        /// <summary>
        /// Filter by challenge type
        /// </summary>
        public ChallengeType? Type { get; set; }

        /// <summary>
        /// Filter by activity type
        /// </summary>
        public ActivityType? ActivityType { get; set; }

        /// <summary>
        /// Search text (matches title and description)
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// ID of the user making the request (for participation info)
        /// </summary>
        public Guid? UserId { get; set; }

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
