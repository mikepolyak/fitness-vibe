using System;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;

namespace FitnessVibe.Domain.Entities.Users
{
    /// <summary>
    /// UserGoal - represents a specific, measurable target that a user sets for themselves.
    /// Think of goals like New Year's resolutions, but smarter and more trackable.
    /// Goals are the stepping stones on the user's fitness journey.
    /// </summary>
    public class UserGoal : BaseEntity
    {
        public int UserId { get; private set; }
        public User User { get; private set; }
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public GoalType Type { get; private set; }
        public GoalFrequency Frequency { get; private set; }
        public decimal TargetValue { get; private set; }
        public decimal CurrentValue { get; private set; }
        public string Unit { get; private set; } // e.g., "steps", "minutes", "kilometers"
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public GoalStatus Status { get; private set; }
        public bool IsAdaptive { get; private set; } // Whether the goal adjusts based on performance

        private UserGoal() { } // For EF Core

        public UserGoal(
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
            User = user ?? throw new ArgumentNullException(nameof(user));
            UserId = user.Id;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description;
            Type = type;
            Frequency = frequency;
            TargetValue = targetValue > 0 ? targetValue : throw new ArgumentException("Target value must be positive");
            Unit = unit ?? throw new ArgumentNullException(nameof(unit));
            StartDate = startDate;
            EndDate = endDate > startDate ? endDate : throw new ArgumentException("End date must be after start date");
            Status = GoalStatus.Active;
            CurrentValue = 0;
            IsAdaptive = isAdaptive;
        }

        public void UpdateProgress(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Progress value cannot be negative");

            CurrentValue = value;
            UpdateStatus();
            MarkAsUpdated();

            // Check if goal is completed
            if (Status == GoalStatus.Completed && CurrentValue >= TargetValue)
            {
                AddDomainEvent(new GoalCompletedEvent(this));
            }
        }

        public void AddProgress(decimal additionalValue)
        {
            if (additionalValue <= 0)
                throw new ArgumentException("Additional value must be positive");

            var oldValue = CurrentValue;
            CurrentValue += additionalValue;
            UpdateStatus();
            MarkAsUpdated();

            // Check if goal is completed
            if (Status == GoalStatus.Completed && oldValue < TargetValue)
            {
                AddDomainEvent(new GoalCompletedEvent(this));
            }
        }

        public void UpdateTarget(decimal newTargetValue)
        {
            if (newTargetValue <= 0)
                throw new ArgumentException("Target value must be positive");

            TargetValue = newTargetValue;
            UpdateStatus();
            MarkAsUpdated();
        }

        public void ExtendDeadline(DateTime newEndDate)
        {
            if (newEndDate <= EndDate)
                throw new ArgumentException("New end date must be after current end date");

            EndDate = newEndDate;
            if (Status == GoalStatus.Expired)
                Status = GoalStatus.Active;
            
            MarkAsUpdated();
        }

        public void Abandon()
        {
            Status = GoalStatus.Abandoned;
            MarkAsUpdated();
            AddDomainEvent(new GoalAbandonedEvent(this));
        }

        public decimal GetProgressPercentage()
        {
            if (TargetValue == 0) return 0;
            return Math.Min(100, (CurrentValue / TargetValue) * 100);
        }

        public bool IsOverdue()
        {
            return DateTime.UtcNow > EndDate && Status == GoalStatus.Active;
        }

        public TimeSpan GetTimeRemaining()
        {
            if (DateTime.UtcNow >= EndDate)
                return TimeSpan.Zero;
            
            return EndDate - DateTime.UtcNow;
        }

        public void AdaptTarget(decimal newTarget)
        {
            if (!IsAdaptive)
                throw new InvalidOperationException("Cannot adapt non-adaptive goals");

            TargetValue = newTarget;
            UpdateStatus();
            MarkAsUpdated();
        }

        private void UpdateStatus()
        {
            if (Status == GoalStatus.Abandoned)
                return; // Don't change status if manually abandoned

            if (DateTime.UtcNow > EndDate)
            {
                Status = CurrentValue >= TargetValue ? GoalStatus.Completed : GoalStatus.Expired;
            }
            else if (CurrentValue >= TargetValue)
            {
                Status = GoalStatus.Completed;
            }
            else
            {
                Status = GoalStatus.Active;
            }
        }
    }

    public enum GoalType
    {
        Steps,         // Daily/weekly steps
        Distance,      // Running/walking distance
        Duration,      // Exercise duration
        Frequency,     // Workout frequency
        Weight,        // Weight loss/gain
        Calories,      // Calories burned
        Custom         // User-defined metric
    }

    public enum GoalFrequency
    {
        Daily,
        Weekly,
        Monthly,
        Quarterly,
        Yearly,
        OneTime
    }

    public enum GoalStatus
    {
        Active,
        Completed,
        Expired,
        Abandoned
    }
}
