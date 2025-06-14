using System;
using MediatR;
using FitnessVibe.Application.Common;
using FitnessVibe.Application.DTOs.Activities;

namespace FitnessVibe.Application.Queries.Activities
{
    /// <summary>
    /// Query to get activity templates.
    /// Like browsing a library of workout plans!
    /// </summary>
    public class GetActivityTemplatesQuery : IRequest<IEnumerable<ActivityTemplateResponse>>, IUserOwnedEntity
    {
        /// <summary>
        /// User ID requesting the templates.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Filter by activity category.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Filter by difficulty level.
        /// </summary>
        public string? DifficultyLevel { get; set; }

        /// <summary>
        /// Filter by duration in minutes.
        /// </summary>
        public int? DurationMinutes { get; set; }

        /// <summary>
        /// Filter by required equipment.
        /// </summary>
        public string? RequiredEquipment { get; set; }

        /// <summary>
        /// Filter for only featured templates.
        /// </summary>
        public bool? IsFeatured { get; set; }

        /// <summary>
        /// Page number for pagination.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Items per page.
        /// </summary>
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Response containing available activity templates.
    /// Like a catalog of workout recipes!
    /// </summary>
    public class ActivityTemplatesResponse
    {
        /// <summary>
        /// List of template details.
        /// </summary>
        public List<ActivityTemplateDto> Templates { get; set; } = new();

        /// <summary>
        /// Pagination information.
        /// </summary>
        public PaginationDto Pagination { get; set; } = new();
    }

    /// <summary>
    /// Activity template details.
    /// </summary>
    public class ActivityTemplateDto
    {
        /// <summary>
        /// Template identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Template name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Template description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Category of activity.
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Type of activity.
        /// </summary>
        public string ActivityType { get; set; } = string.Empty;

        /// <summary>
        /// Estimated duration in minutes.
        /// </summary>
        public int EstimatedDurationMinutes { get; set; }

        /// <summary>
        /// Estimated calories burned.
        /// </summary>
        public int EstimatedCaloriesBurned { get; set; }

        /// <summary>
        /// Difficulty level.
        /// </summary>
        public string DifficultyLevel { get; set; } = string.Empty;

        /// <summary>
        /// Equipment needed.
        /// </summary>
        public List<string> RequiredEquipment { get; set; } = new();

        /// <summary>
        /// Template tags.
        /// </summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>
        /// URL to template icon.
        /// </summary>
        public string IconUrl { get; set; } = string.Empty;

        /// <summary>
        /// Whether this is a featured template.
        /// </summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// Number of times this template has been used.
        /// </summary>
        public int TimesUsed { get; set; }

        /// <summary>
        /// Average rating from users.
        /// </summary>
        public double? AverageRating { get; set; }

        /// <summary>
        /// When the template was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// When the template was last updated.
        /// </summary>
        public DateTime? LastUpdatedAt { get; set; }
    }
}
