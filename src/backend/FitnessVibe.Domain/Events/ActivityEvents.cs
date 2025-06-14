using FitnessVibe.Domain.Common;

namespace FitnessVibe.Domain.Events;

/// <summary>
/// Contains all domain events related to activities
/// </summary>
public static class ActivityEvents
{
    /// <summary>
    /// Event raised when a new activity is created
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user who created the activity</param>
    public record ActivityCreatedEvent(Guid ActivityId, Guid UserId) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event raised when an activity is started
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user who started the activity</param>
    public record ActivityStartedEvent(Guid ActivityId, Guid UserId) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event raised when an activity is paused
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user who paused the activity</param>
    public record ActivityPausedEvent(Guid ActivityId, Guid UserId) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event raised when a paused activity is resumed
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user who resumed the activity</param>
    public record ActivityResumedEvent(Guid ActivityId, Guid UserId) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event raised when an activity is completed
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user who completed the activity</param>
    public record ActivityCompletedEvent(Guid ActivityId, Guid UserId) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event raised when an activity is cancelled
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user who cancelled the activity</param>
    /// <param name="Reason">Reason for cancellation</param>
    public record ActivityCancelledEvent(Guid ActivityId, Guid UserId, string? Reason) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event raised when a GPS point is added to an activity's route
    /// </summary>
    /// <param name="ActivityId">ID of the activity</param>
    /// <param name="UserId">ID of the user whose activity is being tracked</param>
    /// <param name="Latitude">The latitude of the GPS point</param>
    /// <param name="Longitude">The longitude of the GPS point</param>
    /// <param name="Elevation">Optional elevation in meters</param>
    /// <param name="Speed">Optional speed in meters per second</param>
    public record ActivityRoutePointAddedEvent(
        Guid ActivityId,
        Guid UserId,
        double Latitude,
        double Longitude,
        double? Elevation,
        double? Speed) : IDomainEvent
    {
        /// <inheritdoc/>
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
