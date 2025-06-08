using Microsoft.EntityFrameworkCore;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Infrastructure.Data;

namespace FitnessVibe.Infrastructure.Data.Repositories
{
    /// <summary>
    /// User Repository Implementation - the membership database manager.
    /// Think of this as the specialized staff member who handles all
    /// member record operations: finding, storing, updating member information.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly FitnessVibeDbContext _context;

        public UserRepository(FitnessVibeDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Get user by ID with related data.
        /// Like looking up a member's complete profile and history.
        /// </summary>
        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Goals.Where(g => !g.IsDeleted))
                .Include(u => u.Badges.Where(b => !b.IsDeleted))
                    .ThenInclude(ub => ub.Badge)
                .Include(u => u.Activities.Where(a => !a.IsDeleted).OrderByDescending(a => a.CompletedAt).Take(10))
                    .ThenInclude(ua => ua.Activity)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        /// <summary>
        /// Get user by email address.
        /// Like looking up a member by their registered email.
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower().Trim(), cancellationToken);
        }

        /// <summary>
        /// Get all users (with pagination support).
        /// Like getting a directory of all gym members.
        /// </summary>
        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Add a new user to the database.
        /// Like registering a new gym member.
        /// </summary>
        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }

        /// <summary>
        /// Update an existing user.
        /// Like updating a member's profile information.
        /// </summary>
        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Soft delete a user.
        /// Like marking a membership as cancelled but keeping the record.
        /// </summary>
        public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.SoftDelete();
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Check if an email already exists.
        /// Like verifying if someone is already a member before signup.
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower().Trim(), cancellationToken);
        }

        /// <summary>
        /// Search users by name or email.
        /// Like searching the member directory.
        /// </summary>
        public async Task<IEnumerable<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<User>();

            var term = searchTerm.ToLower().Trim();

            return await _context.Users
                .Where(u => 
                    u.FirstName.ToLower().Contains(term) ||
                    u.LastName.ToLower().Contains(term) ||
                    u.Email.ToLower().Contains(term) ||
                    (u.FirstName + " " + u.LastName).ToLower().Contains(term))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Take(20) // Limit results for performance
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get users by fitness level.
        /// Like grouping members by their experience level.
        /// </summary>
        public async Task<IEnumerable<User>> GetUsersByLevelAsync(int level, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Where(u => u.Level == level)
                .OrderByDescending(u => u.ExperiencePoints)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get total user count.
        /// Like getting the total membership count.
        /// </summary>
        public async Task<int> GetUserCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users.CountAsync(cancellationToken);
        }

        /// <summary>
        /// Get users with recent activity.
        /// Like finding members who have been active recently.
        /// </summary>
        public async Task<IEnumerable<User>> GetRecentlyActiveUsersAsync(int days = 7, CancellationToken cancellationToken = default)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            
            return await _context.Users
                .Where(u => u.LastActiveDate >= cutoffDate)
                .OrderByDescending(u => u.LastActiveDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get top users by level and experience.
        /// Like the fitness center leaderboard.
        /// </summary>
        public async Task<IEnumerable<User>> GetTopUsersByExperienceAsync(int count = 10, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .OrderByDescending(u => u.Level)
                .ThenByDescending(u => u.ExperiencePoints)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get users who haven't been active recently.
        /// Like finding members who might need encouragement.
        /// </summary>
        public async Task<IEnumerable<User>> GetInactiveUsersAsync(int days = 30, CancellationToken cancellationToken = default)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            
            return await _context.Users
                .Where(u => u.LastActiveDate < cutoffDate)
                .OrderBy(u => u.LastActiveDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get users by fitness goal.
        /// Like grouping members by what they're trying to achieve.
        /// </summary>
        public async Task<IEnumerable<User>> GetUsersByGoalAsync(FitnessGoal goal, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Where(u => u.PrimaryGoal == goal)
                .OrderByDescending(u => u.ExperiencePoints)
                .ToListAsync(cancellationToken);
        }
    }
}
