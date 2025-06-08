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
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetUsersByLevelAsync(int level, CancellationToken cancellationToken = default);
        Task<int> GetUserCountAsync(CancellationToken cancellationToken = default);
    }
}
