using System;
using System.Collections.Generic;
using MediatR;
using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Application.Commands.Activities
{
    /// <summary>
    /// Command to create a new activity template
    /// </summary>
    public record CreateActivityTemplateCommand : IRequest<CreateActivityTemplateResponse>
    {
        /// <summary>
        /// Name of the template
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Description of what the template involves
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Type of activity
        /// </summary>
        public ActivityType Type { get; init; }

        /// <summary>
        /// Category of activity
        /// </summary>
        public ActivityCategory Category { get; init; }

        /// <summary>
        /// Estimated duration in minutes
        /// </summary>
        public int EstimatedDurationMinutes { get; init; }

        /// <summary>
        /// Estimated calories burned
        /// </summary>
        public int EstimatedCaloriesBurned { get; init; }

        /// <summary>
        /// Difficulty level (1-5)
        /// </summary>
        public int DifficultyLevel { get; init; }

        /// <summary>
        /// Required equipment
        /// </summary>        public IEnumerable<string>? RequiredEquipment { get; init; }

        /// <summary>
        /// Search tags
        /// </summary>
        public IEnumerable<string>? Tags { get; init; }

        /// <summary>
        /// URL to template icon
        /// </summary>
        public string? IconUrl { get; init; }

        /// <summary>
        /// Whether this is a featured template
        /// </summary>
        public bool IsFeatured { get; init; }

        /// <summary>
        /// ID of the user creating the template
        /// </summary>
        public Guid CreatedById { get; set; }
    }

    /// <summary>
    /// Response for creating an activity template
    /// </summary>
    public record CreateActivityTemplateResponse
    {
        /// <summary>
        /// ID of the created template
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Name of the template
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// When the template was created
        /// </summary>
        public DateTime CreatedAt { get; init; }
    }
}
