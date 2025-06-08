using System;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Enums;
using FitnessVibe.Domain.Events;

namespace FitnessVibe.Domain.Entities.Users
{
    /// <summary>
    /// UserGoal - represents a specific, measurable target that a user sets for themselves.
    /// Think of goals like New Year's resolutions, but smarter and more trackable.
    /// Goals are the stepping stones on the user's fitness journey.
    /// </summary>
    public class UserGoal : BaseEntity
    {
        /// <summary>
        /// The ID of the user who created this goal
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Navigation property to the user who created this goal
        /// </summary>
        public required User User { get; private set; }

        /// <summary>
        /// The title or name of the goal
        /// </summary>
        public required string Title { get; private set; }

        /// <summary>
        /// Optional detailed description of the goal
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// The type of goal (Distance, Duration, Frequency, etc.)
        /// </summary>
        public GoalType Type { get; private set; }

        /// <summary>
        /// How often the goal is evaluated (Daily, Weekly, etc.)
        /// </summary>
        public GoalFrequency Frequency { get; private set; }

        /// <summary>
        /// The target value the user is aiming for
        /// </summary>
        public decimal TargetValue { get; private set; }

        /// <summary>
        /// The current progress towards the target value
        /// </summary>
        public decimal CurrentValue { get; private set; }

        /// <summary>
        /// The unit of measurement (e.g., "steps", "minutes", "kilometers")
        /// </summary>
        public required string Unit { get; private set; }

        /// <summary>
        /// When the goal becomes active
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// When the goal should be completed by
        /// </summary>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// The current status of the goal
        /// </summary>
        public GoalStatus Status { get; private set; }

        /// <summary>
        /// Whether the goal adjusts based on performance
        /// </summary>
        public bool IsAdaptive { get; private set; }

        // For EF Core
        protected UserGoal() { }

        /// <summary>
        /// Creates a new user goal
        /// </summary>
        public static UserGoal Create(
            User user,
            string title,
            GoalType type,
            GoalFrequency frequency,
            decimal targetValue,
            string unit,
            DateTime startDate,
            DateTime endDate,
            string? description = null,
            bool isAdaptive = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            if (string.IsNullOrWhiteSpace(unit))
                throw new ArgumentException("Unit cannot be empty", nameof(unit));
            if (targetValue <= 0)
                throw new ArgumentException("Target value must be greater than zero", nameof(targetValue));
            if (startDate >= endDate)
                throw new ArgumentException("End date must be after start date");
            if (startDate < DateTime.UtcNow.Date)
                throw new ArgumentException("Start date cannot be in the past");

            var goal = new UserGoal
            {
                User = user,
                UserId = user.Id,
                Title = title,
                Description = description,
                Type = type,
                Frequency = frequency,
                TargetValue = targetValue,
                CurrentValue = 0,
                Unit = unit,
                StartDate = startDate.Date,
                EndDate = endDate.Date,
                Status = GoalStatus.Pending,
                IsAdaptive = isAdaptive
            };

            return goal;
        }

        /// <summary>
        /// Updates the goal's target value
        /// </summary>
        public void UpdateTarget(decimal newTargetValue)
        {
            if (newTargetValue <= 0)
                throw new ArgumentException("Target value must be greater than zero");

            if (Status == GoalStatus.Completed || Status == GoalStatus.Failed || Status == GoalStatus.Abandoned)
                throw new InvalidOperationException("Cannot update target of a completed, failed, or abandoned goal");

            TargetValue = newTargetValue;
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates the goal's end date
        /// </summary>
        public void ExtendDeadline(DateTime newEndDate)
        {
            if (newEndDate <= DateTime.UtcNow.Date)
                throw new ArgumentException("New end date must be in the future");
            if (Status == GoalStatus.Completed || Status == GoalStatus.Failed || Status == GoalStatus.Abandoned)
                throw new InvalidOperationException("Cannot extend deadline of a completed, failed, or abandoned goal");

            EndDate = newEndDate.Date;
            MarkAsUpdated();
        }

        /// <summary>
        /// Records progress towards the goal's target
        /// </summary>
        public void RecordProgress(decimal progressValue)
        {
            if (progressValue <= 0)
                throw new ArgumentException("Progress must be greater than zero");
            if (Status != GoalStatus.Active && Status != GoalStatus.Pending)
                throw new InvalidOperationException("Cannot record progress for inactive goal");

            if (Status == GoalStatus.Pending)
                Status = GoalStatus.Active;

            CurrentValue += progressValue;
            MarkAsUpdated();

            if (CurrentValue >= TargetValue)
            {
                Status = GoalStatus.Completed;
                MarkAsUpdated();
                AddDomainEvent(new GoalCompletedEvent(this));
            }
        }

        /// <summary>
        /// Sets a new value for the current progress
        /// </summary>
        public void SetProgress(decimal newValue)
        {
            if (newValue < 0)
                throw new ArgumentException("Progress cannot be negative");
            if (Status != GoalStatus.Active && Status != GoalStatus.Pending)
                throw new InvalidOperationException("Cannot set progress for inactive goal");

            if (Status == GoalStatus.Pending)
                Status = GoalStatus.Active;

            CurrentValue = newValue;
            MarkAsUpdated();

            if (CurrentValue >= TargetValue)
            {
                Status = GoalStatus.Completed;
                MarkAsUpdated();
                AddDomainEvent(new GoalCompletedEvent(this));
            }
        }

        /// <summary>
        /// Pauses progress tracking for this goal
        /// </summary>
        public void Pause()
        {
            if (Status != GoalStatus.Active)
                throw new InvalidOperationException("Can only pause active goals");

            Status = GoalStatus.Paused;
            MarkAsUpdated();
        }

        /// <summary>
        /// Resumes progress tracking for this goal
        /// </summary>
        public void Resume()
        {
            if (Status != GoalStatus.Paused)
                throw new InvalidOperationException("Can only resume paused goals");

            Status = GoalStatus.Active;
            MarkAsUpdated();
        }

        /// <summary>
        /// Abandons the goal
        /// </summary>
        public void Abandon()
        {
            if (Status == GoalStatus.Completed || Status == GoalStatus.Failed || Status == GoalStatus.Abandoned)
                throw new InvalidOperationException("Goal is already in a final state");

            Status = GoalStatus.Abandoned;
            MarkAsUpdated();
            AddDomainEvent(new GoalAbandonedEvent(this));
        }

        /// <summary>
        /// Checks if the goal is overdue based on the current date
        /// </summary>
        public void CheckOverdue()
        {
            if (Status == GoalStatus.Active && DateTime.UtcNow.Date > EndDate)
            {
                Status = GoalStatus.Failed;
                MarkAsUpdated();
            }
        }
    }
}
