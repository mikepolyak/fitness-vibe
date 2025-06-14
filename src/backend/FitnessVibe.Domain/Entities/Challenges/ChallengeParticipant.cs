using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.Events;

namespace FitnessVibe.Domain.Entities.Challenges
{
    /// <summary>
    /// Represents a participant in a challenge and tracks their progress
    /// </summary>
    public class ChallengeParticipant : BaseEntity
    {
        /// <summary>
        /// The challenge this participation is for
        /// </summary>
        public Guid ChallengeId { get; private set; }

        /// <summary>
        /// Navigation property for the challenge
        /// </summary>
        public Challenge Challenge { get; private set; }

        /// <summary>
        /// The participating user's ID
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Navigation property for the user
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        /// When the user joined the challenge
        /// </summary>
        public DateTime JoinedAt { get; private set; }

        /// <summary>
        /// The user's current progress value
        /// </summary>
        public decimal Progress { get; private set; }

        /// <summary>
        /// Whether the user has completed the challenge
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// When the user completed the challenge (if completed)
        /// </summary>
        public DateTime? CompletedAt { get; private set; }

        private ChallengeParticipant() { }

        public ChallengeParticipant(Guid challengeId, Guid userId)
        {
            ChallengeId = challengeId;
            UserId = userId;
            JoinedAt = DateTime.UtcNow;
            Progress = 0;
            IsCompleted = false;
        }

        public void UpdateProgress(decimal newProgress)
        {
            if (IsCompleted)
                throw new InvalidOperationException("Cannot update progress for completed challenge");

            if (newProgress < Progress)
                throw new InvalidOperationException("New progress cannot be less than current progress");

            Progress = newProgress;
            AddDomainEvent(new ChallengeProgressUpdatedEvent(ChallengeId, UserId, Progress));
        }

        public void Complete()
        {
            if (IsCompleted)
                return;

            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
        }
    }
}
