using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FitnessVibe.Domain.Entities.Users;

namespace FitnessVibe.Domain.Repositories
{
    /// <summary>
    /// Repository contract for User entity operations.
    /// Think of repositories as the "librarians" of our domain - they know how to find,
    /// store, and manage our domain entities efficiently.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user by their unique identifier
        /// </summary>
        /// <param name="id">The ID of the user to retrieve</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a user by their email address
        /// </summary>
        /// <param name="email">The email address to search for</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all users in the system
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>An enumerable collection of all users</returns>
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new user to the system
        /// </summary>
        /// <param name="user">The user to add</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>The added user with their generated ID</returns>
        Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing user's information
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a user from the system
        /// </summary>
        /// <param name="user">The user to delete</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        Task DeleteAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an email address is already registered
        /// </summary>
        /// <param name="email">The email address to check</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>True if the email exists, false otherwise</returns>
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches for users based on a search term
        /// </summary>
        /// <param name="searchTerm">The term to search for in user profiles</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>An enumerable collection of matching users</returns>
        Task<IEnumerable<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all users at a specific fitness level
        /// </summary>
        /// <param name="level">The level to filter by</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>An enumerable collection of users at the specified level</returns>
        Task<IEnumerable<User>> GetUsersByLevelAsync(int level, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total number of users in the system
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        /// <returns>The total number of users</returns>
        Task<int> GetUserCountAsync(CancellationToken cancellationToken = default);
    }
}
