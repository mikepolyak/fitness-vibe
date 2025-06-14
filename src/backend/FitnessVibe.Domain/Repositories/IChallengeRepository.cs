using FitnessVibe.Domain.Entities.Challenges;

namespace FitnessVibe.Domain.Repositories
{
    /// <summary>
    /// Repository interface for managing challenges
    /// </summary>
    public interface IChallengeRepository
    {
        /// <summary>
        /// Gets a challenge by its ID
        /// </summary>
        Task<Challenge> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all active challenges
        /// </summary>
        Task<IEnumerable<Challenge>> GetActiveChallengesAsync();

        /// <summary>
        /// Gets all challenges a user is participating in
        /// </summary>
        Task<IEnumerable<Challenge>> GetUserChallengesAsync(Guid userId);

        /// <summary>
        /// Gets all challenges created by a user
        /// </summary>
        Task<IEnumerable<Challenge>> GetChallengesCreatedByUserAsync(Guid userId);

        /// <summary>
        /// Gets all challenges matching the given criteria
        /// </summary>
        Task<IEnumerable<Challenge>> SearchChallengesAsync(
            bool? isActive = null,
            bool? isPrivate = null,
            DateTime? startDateFrom = null,
            DateTime? startDateTo = null,
            ChallengeType? type = null,
            ActivityType? activityType = null);

        /// <summary>
        /// Adds a new challenge
        /// </summary>
        Task<Challenge> AddAsync(Challenge challenge);

        /// <summary>
        /// Updates an existing challenge
        /// </summary>
        Task UpdateAsync(Challenge challenge);

        /// <summary>
        /// Gets a challenge participant by user ID and challenge ID
        /// </summary>
        Task<ChallengeParticipant> GetParticipantAsync(Guid challengeId, Guid userId);

        /// <summary>
        /// Gets all participants for a challenge
        /// </summary>
        Task<IEnumerable<ChallengeParticipant>> GetParticipantsAsync(Guid challengeId);
    }
}
