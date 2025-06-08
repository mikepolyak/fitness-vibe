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
        public virtual User User { get; private set; } = null!;

        /// <summary>
        /// The title or name of the goal
        /// </summary>
        public string Title { get; private set; } = string.Empty;

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
        public string Unit { get; private set; } = string.Empty;

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

        /// <summary>
        /// Protected constructor for EF Core
        /// </summary>
        protected UserGoal() { }

        /// <summary>
        /// Creates a new user goal
        /// </summary>
        /// <param name="user">The user creating the goal</param>
        /// <param name="title">Title of the goal</param>
        /// <param name="type">Type of goal (Distance, Duration, etc.)</param>
        /// <param name="frequency">How often the goal is evaluated</param>
        /// <param name="targetValue">The target value to achieve</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="startDate">When the goal becomes active</param>
        /// <param name="endDate">When the goal should be completed by</param>
        /// <param name="description">Optional detailed description</param>
        /// <param name="isAdaptive">Whether the goal adjusts based on performance</param>
        /// <returns>A new UserGoal instance</returns>
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
                throw new ArgumentException("Goal title cannot be empty", nameof(title));

            if (string.IsNullOrWhiteSpace(unit))
                throw new ArgumentException("Goal unit cannot be empty", nameof(unit));

            if (targetValue <= 0)
                throw new ArgumentException("Target value must be greater than zero", nameof(targetValue));

            if (endDate <= startDate)
                throw new ArgumentException("End date must be after start date", nameof(endDate));

            var goal = new UserGoal
            {
                UserId = user.Id,
                User = user,
                Title = title,
                Description = description,
                Type = type,
                Frequency = frequency,
                TargetValue = targetValue,
                CurrentValue = 0,
                Unit = unit,
                StartDate = startDate,
                EndDate = endDate,
                Status = GoalStatus.Active,
                IsAdaptive = isAdaptive
            };

            return goal;
        }

        /// <summary>
        /// Updates progress towards the goal's target
        /// </summary>
        /// <param name="value">The value to add to current progress</param>
        public void UpdateProgress(decimal value)
        {
            if (Status != GoalStatus.Active)
                return;

            CurrentValue += value;
            MarkAsUpdated();

            if (CurrentValue >= TargetValue)
            {
                CompleteGoal();
            }
        }

        /// <summary>
        /// Marks the goal as completed and raises appropriate events
        /// </summary>
        public void CompleteGoal()
        {
            if (Status == GoalStatus.Completed)
                return;

            Status = GoalStatus.Completed;
            MarkAsUpdated();
            AddDomainEvent(new GoalCompletedEvent(this));
        }

        /// <summary>
        /// Marks the goal as failed if it's past the end date and not completed
        /// </summary>
        public void CheckFailure()
        {
            if (Status == GoalStatus.Failed || Status == GoalStatus.Completed)
                return;

            if (DateTime.UtcNow > EndDate && CurrentValue < TargetValue)
            {
                Status = GoalStatus.Failed;
                MarkAsUpdated();
                AddDomainEvent(new GoalFailedEvent(this));
            }
        }

        /// <summary>
        /// Updates the goal's end date
        /// </summary>
        /// <param name="newEndDate">The new end date for the goal</param>
        public void ExtendDeadline(DateTime newEndDate)
        {
            if (Status != GoalStatus.Active)
                return;

            if (newEndDate <= EndDate)
                throw new ArgumentException("New end date must be later than current end date", nameof(newEndDate));

            EndDate = newEndDate;
            MarkAsUpdated();
        }

        /// <summary>
        /// Abandons the goal, marking it as inactive and raising appropriate events
        /// </summary>
        public void AbandonGoal()
        {
            if (Status != GoalStatus.Active)
                return;

            Status = GoalStatus.Abandoned;
            MarkAsUpdated();
            AddDomainEvent(new GoalAbandonedEvent(this));
        }
    }
}
