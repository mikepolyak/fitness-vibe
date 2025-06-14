using Microsoft.EntityFrameworkCore;
using FitnessVibe.Domain.Entities.Challenges;
using FitnessVibe.Domain.Repositories;

namespace FitnessVibe.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for managing challenges
    /// </summary>
    public class ChallengeRepository : IChallengeRepository
    {
        private readonly FitnessVibeDbContext _context;

        public ChallengeRepository(FitnessVibeDbContext context)
        {
            _context = context;
        }

        public async Task<Challenge> GetByIdAsync(Guid id)
        {
            return await _context.Challenges
                .Include(c => c.Participants)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Challenge>> GetActiveChallengesAsync()
        {
            return await _context.Challenges
                .Include(c => c.Participants)
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Challenge>> GetUserChallengesAsync(Guid userId)
        {
            return await _context.Challenges
                .Include(c => c.Participants)
                .Where(c => c.Participants.Any(p => p.UserId == userId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Challenge>> GetChallengesCreatedByUserAsync(Guid userId)
        {
            return await _context.Challenges
                .Include(c => c.Participants)
                .Where(c => c.CreatedById == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Challenge>> SearchChallengesAsync(
            bool? isActive = null,
            bool? isPrivate = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null,
            ChallengeType? type = null,
            ActivityType? activityType = null)
        {
            var query = _context.Challenges.Include(c => c.Participants).AsQueryable();

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            if (isPrivate.HasValue)
                query = query.Where(c => c.IsPrivate == isPrivate.Value);

            if (startDateFrom.HasValue)
                query = query.Where(c => c.StartDate >= startDateFrom.Value);

            if (startDateTo.HasValue)
                query = query.Where(c => c.StartDate <= startDateTo.Value);

            if (type.HasValue)
                query = query.Where(c => c.Type == type.Value);

            if (activityType.HasValue)
                query = query.Where(c => c.ActivityType == activityType.Value);

            return await query.ToListAsync();
        }

        public async Task<Challenge> AddAsync(Challenge challenge)
        {
            await _context.Challenges.AddAsync(challenge);
            return challenge;
        }

        public Task UpdateAsync(Challenge challenge)
        {
            _context.Entry(challenge).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public async Task<ChallengeParticipant> GetParticipantAsync(Guid challengeId, Guid userId)
        {
            return await _context.ChallengeParticipants
                .FirstOrDefaultAsync(p => p.ChallengeId == challengeId && p.UserId == userId);
        }

        public async Task<IEnumerable<ChallengeParticipant>> GetParticipantsAsync(Guid challengeId)
        {
            return await _context.ChallengeParticipants
                .Include(p => p.User)
                .Where(p => p.ChallengeId == challengeId)
                .ToListAsync();
        }
    }
}
