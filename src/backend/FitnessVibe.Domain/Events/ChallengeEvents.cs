using System;

namespace FitnessVibe.Domain.Events
{
    /// <summary>
    /// Event raised when a new challenge is created
    /// </summary>
    public record ChallengeCreatedEvent(Guid ChallengeId, string Title, Guid CreatedById) : IDomainEvent;

    /// <summary>
    /// Event raised when a challenge is activated
    /// </summary>
    public record ChallengeActivatedEvent(Guid ChallengeId) : IDomainEvent;

    /// <summary>
    /// Event raised when a challenge is deactivated
    /// </summary>
    public record ChallengeDeactivatedEvent(Guid ChallengeId) : IDomainEvent;

    /// <summary>
    /// Event raised when a challenge is completed
    /// </summary>
    public record ChallengeCompletedEvent(Guid ChallengeId) : IDomainEvent;

    /// <summary>
    /// Event raised when a participant joins a challenge
    /// </summary>
    public record ChallengeParticipantAddedEvent(Guid ChallengeId, Guid UserId) : IDomainEvent;

    /// <summary>
    /// Event raised when a participant updates their progress
    /// </summary>
    public record ChallengeProgressUpdatedEvent(Guid ChallengeId, Guid UserId, decimal NewProgress) : IDomainEvent;

    /// <summary>
    /// Event raised when a participant completes a challenge
    /// </summary>
    public record ChallengeParticipantCompletedEvent(Guid ChallengeId, Guid UserId) : IDomainEvent;
}
