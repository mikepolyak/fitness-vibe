using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.Common;

namespace FitnessVibe.Domain.Repositories
{
    /// <summary>
    /// Repository interface for managing activity templates
    /// </summary>
    public interface IActivityTemplateRepository : IRepository<ActivityTemplate>
    {
        /// <summary>
        /// Gets all featured templates
        /// </summary>
        Task<IEnumerable<ActivityTemplate>> GetFeaturedTemplatesAsync();

        /// <summary>
        /// Gets templates by category
        /// </summary>
        Task<IEnumerable<ActivityTemplate>> GetByCategoryAsync(ActivityCategory category);

        /// <summary>
        /// Gets templates by difficulty level range
        /// </summary>
        Task<IEnumerable<ActivityTemplate>> GetByDifficultyRangeAsync(int minDifficulty, int maxDifficulty);

        /// <summary>
        /// Gets templates that require specific equipment
        /// </summary>
        Task<IEnumerable<ActivityTemplate>> GetByEquipmentAsync(string equipment);

        /// <summary>
        /// Gets templates by tag
        /// </summary>
        Task<IEnumerable<ActivityTemplate>> GetByTagAsync(string tag);

        /// <summary>
        /// Gets the top N most used templates
        /// </summary>
        Task<IEnumerable<ActivityTemplate>> GetMostPopularAsync(int count);

        /// <summary>
        /// Gets templates by average rating range
        /// </summary>
        Task<IEnumerable<ActivityTemplate>> GetByRatingRangeAsync(decimal minRating, decimal maxRating);

        /// <summary>
        /// Searches templates by name and description
        /// </summary>
        Task<IEnumerable<ActivityTemplate>> SearchAsync(string searchTerm);

        /// <summary>
        /// Gets templates suitable for a given estimated duration range (in minutes)
        /// </summary>
        Task<IEnumerable<ActivityTemplate>> GetByDurationRangeAsync(int minMinutes, int maxMinutes);

        /// <summary>
        /// Gets similar templates to a given template
        /// </summary>
        Task<IEnumerable<ActivityTemplate>> GetSimilarTemplatesAsync(Guid templateId, int count = 5);
    }
}
