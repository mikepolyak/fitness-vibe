using System;
using System.Threading.Tasks;
using FitnessVibe.Domain.Entities.Activities;

namespace FitnessVibe.Domain.Repositories
{
    /// <summary>
    /// Repository interface for managing Activity entities
    /// </summary>
    public interface IActivityRepository
    {
        /// <summary>
        /// Gets an activity by its ID
        /// </summary>
        /// <param name="id">The ID of the activity to retrieve</param>
        /// <returns>The activity if found, null otherwise</returns>
        Task<Activity?> GetByIdAsync(Guid id);

        /// <summary>
        /// Updates an activity
        /// </summary>
        /// <param name="activity">The activity to update</param>
        Task UpdateAsync(Activity activity);

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        Task SaveChangesAsync();
    }
}
