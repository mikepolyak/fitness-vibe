using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.Events;
using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Domain.Entities.Challenges
{
    /// <summary>
    /// Represents a fitness challenge that users can participate in.
    /// A challenge can be individual or group-based, time-bound or ongoing,
    /// and can track various fitness metrics like distance, calories, or activity count.
    /// </summary>
    public class Challenge : BaseEntity
    {
        /// <summary>
        /// The title of the challenge
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Detailed description of the challenge
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// When the challenge starts
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// When the challenge ends (optional for ongoing challenges)
        /// </summary>
        public DateTime? EndDate { get; private set; }

        /// <summary>
        /// The type of challenge (e.g., Distance, Calories, ActivityCount)
        /// </summary>
        public ChallengeType Type { get; private set; }

        /// <summary>
        /// The target value to achieve (e.g., 100km, 5000 calories)
        /// </summary>
        public decimal TargetValue { get; private set; }

        /// <summary>
        /// The unit of measurement (e.g., km, calories, activities)
        /// </summary>
        public string Unit { get; private set; }

        /// <summary>
        /// The type of activity this challenge is for (optional)
        /// </summary>
        public ActivityType? ActivityType { get; private set; }

        /// <summary>
        /// Whether the challenge is private (invitation only)
        /// </summary>
        public bool IsPrivate { get; private set; }

        /// <summary>
        /// Whether the challenge is currently active
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// The user who created the challenge
        /// </summary>
        public Guid CreatedById { get; private set; }

        /// <summary>
        /// Navigation property for the creator
        /// </summary>
        public User CreatedBy { get; private set; }

        /// <summary>
        /// Challenge participants and their progress
        /// </summary>
        public ICollection<ChallengeParticipant> Participants { get; private set; }

        private Challenge()
        {
            Participants = new List<ChallengeParticipant>();
        }

        public Challenge(
            string title,
            string description,
            DateTime startDate,
            DateTime? endDate,
            ChallengeType type,
            decimal targetValue,
            string unit,
            ActivityType? activityType,
            bool isPrivate,
            Guid createdById) : this()
        {
            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Type = type;
            TargetValue = targetValue;
            Unit = unit;
            ActivityType = activityType;
            IsPrivate = isPrivate;
            CreatedById = createdById;
            IsActive = false;

            AddDomainEvent(new ChallengeCreatedEvent(Id, title, createdById));
        }

        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("Challenge is already active");

            if (StartDate > DateTime.UtcNow)
                throw new InvalidOperationException("Challenge cannot be activated before its start date");

            if (EndDate.HasValue && EndDate.Value < DateTime.UtcNow)
                throw new InvalidOperationException("Challenge cannot be activated after its end date");

            IsActive = true;
            AddDomainEvent(new ChallengeActivatedEvent(Id));
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("Challenge is not active");

            IsActive = false;
            AddDomainEvent(new ChallengeDeactivatedEvent(Id));
        }

        public ChallengeParticipant AddParticipant(Guid userId)
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot join an inactive challenge");

            if (EndDate.HasValue && EndDate.Value < DateTime.UtcNow)
                throw new InvalidOperationException("Cannot join a completed challenge");

            if (Participants.Any(p => p.UserId == userId))
                throw new InvalidOperationException("User is already participating in this challenge");

            var participant = new ChallengeParticipant(Id, userId);
            Participants.Add(participant);

            AddDomainEvent(new ChallengeParticipantAddedEvent(Id, userId));

            return participant;
        }

        public void UpdateProgress(Guid userId, decimal progress)
        {
            var participant = Participants.FirstOrDefault(p => p.UserId == userId)
                ?? throw new InvalidOperationException("User is not participating in this challenge");

            participant.UpdateProgress(progress);

            if (participant.Progress >= TargetValue)
            {
                participant.Complete();
                AddDomainEvent(new ChallengeParticipantCompletedEvent(Id, userId));
            }
        }

        public bool IsCompleted(Guid userId)
        {
            var participant = Participants.FirstOrDefault(p => p.UserId == userId)
                ?? throw new InvalidOperationException("User is not participating in this challenge");

            return participant.IsCompleted;
        }

        public decimal GetProgress(Guid userId)
        {
            var participant = Participants.FirstOrDefault(p => p.UserId == userId)
                ?? throw new InvalidOperationException("User is not participating in this challenge");

            return participant.Progress;
        }

        public void Complete()
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot complete an inactive challenge");

            IsActive = false;
            AddDomainEvent(new ChallengeCompletedEvent(Id));
        }
    }
}
