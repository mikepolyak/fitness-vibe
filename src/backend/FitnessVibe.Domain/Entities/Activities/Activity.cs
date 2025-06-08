using System;
using System.Collections.Generic;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.ValueObjects;

namespace FitnessVibe.Domain.Entities.Activities
{
    /// <summary>
    /// Activity entity - the core of our fitness tracking.
    /// Think of each activity as a chapter in the user's fitness story.
    /// Activities are where users spend their time, burn calories, and make progress.
    /// </summary>
    public class Activity : BaseEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public ActivityType Type { get; private set; }
        public ActivityCategory Category { get; private set; }
        public string? IconUrl { get; private set; }
        public decimal MetValue { get; private set; } // Metabolic Equivalent of Task
        public bool IsActive { get; private set; }

        // Navigation properties
        public ICollection<UserActivity> UserActivities { get; private set; } = new List<UserActivity>();

        private Activity() { } // For EF Core

        public Activity(
            string name,
            ActivityType type,
            ActivityCategory category,
            decimal metValue,
            string? description = null,
            string? iconUrl = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Type = type;
            Category = category;
            MetValue = metValue >= 0 ? metValue : throw new ArgumentException("MET value cannot be negative");
            IconUrl = iconUrl;
            IsActive = true;
        }

        public void UpdateDetails(string name, string? description = null, string? iconUrl = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            IconUrl = iconUrl;
            MarkAsUpdated();
        }

        public void UpdateMetValue(decimal metValue)
        {
            MetValue = metValue >= 0 ? metValue : throw new ArgumentException("MET value cannot be negative");
            MarkAsUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// UserActivity - a specific instance of a user performing an activity.
    /// Think of this as a diary entry: "Today I ran 5 kilometers in 30 minutes."
    /// Each UserActivity captures the who, what, when, where, and how of a fitness session.
    /// </summary>
    public class UserActivity : BaseEntity
    {
        public int UserId { get; private set; }
        public User User { get; private set; }
        public int ActivityId { get; private set; }
        public Activity Activity { get; private set; }
        
        // When and where
        public DateTime StartedAt { get; private set; }
        public DateTime CompletedAt { get; private set; }
        public TimeSpan Duration => CompletedAt - StartedAt;
        public string? Location { get; private set; }
        
        // Performance metrics
        public decimal? Distance { get; private set; } // in kilometers
        public int? Steps { get; private set; }
        public decimal? CaloriesBurned { get; private set; }
        public decimal? AverageHeartRate { get; private set; }
        public decimal? MaxHeartRate { get; private set; }
        public decimal? AveragePace { get; private set; } // minutes per kilometer
        public decimal? ElevationGain { get; private set; } // in meters
        
        // User input
        public int? UserRating { get; private set; } // 1-5 scale, how did they feel?
        public string? Notes { get; private set; }
        public List<string> Photos { get; private set; } = new();
        
        // Social and engagement
        public bool IsPublic { get; private set; }
        public int ExperiencePointsEarned { get; private set; }
        
        // GPS and route data
        public string? RouteData { get; private set; } // JSON GPS coordinates
        public string? WeatherConditions { get; private set; } // JSON weather data

        private UserActivity() { } // For EF Core

        public UserActivity(
            User user,
            Activity activity,
            DateTime startedAt,
            DateTime completedAt,
            decimal? distance = null,
            int? steps = null,
            string? location = null,
            bool isPublic = false)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            UserId = user.Id;
            Activity = activity ?? throw new ArgumentNullException(nameof(activity));
            ActivityId = activity.Id;
            StartedAt = startedAt;
            CompletedAt = completedAt > startedAt ? completedAt : throw new ArgumentException("Completed time must be after start time");
            Distance = distance;
            Steps = steps;
            Location = location;
            IsPublic = isPublic;

            // Calculate calories and XP
            CalculateCaloriesBurned();
            CalculateExperiencePoints();

            // Award XP to user
            user.AddExperience(ExperiencePointsEarned);
            user.RecordActivity();

            AddDomainEvent(new ActivityCompletedEvent(this));
        }

        public void UpdateMetrics(
            decimal? distance = null,
            int? steps = null,
            decimal? averageHeartRate = null,
            decimal? maxHeartRate = null,
            decimal? elevationGain = null)
        {
            Distance = distance;
            Steps = steps;
            AverageHeartRate = averageHeartRate;
            MaxHeartRate = maxHeartRate;
            ElevationGain = elevationGain;

            if (distance.HasValue && Duration.TotalMinutes > 0)
            {
                AveragePace = (decimal)(Duration.TotalMinutes / (double)distance.Value);
            }

            CalculateCaloriesBurned();
            MarkAsUpdated();
        }

        public void UpdateUserFeedback(int rating, string? notes = null)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            UserRating = rating;
            Notes = notes;
            MarkAsUpdated();
        }

        public void AddPhoto(string photoUrl)
        {
            if (string.IsNullOrWhiteSpace(photoUrl))
                throw new ArgumentException("Photo URL cannot be empty");

            Photos.Add(photoUrl);
            MarkAsUpdated();
        }

        public void UpdatePrivacy(bool isPublic)
        {
            IsPublic = isPublic;
            MarkAsUpdated();
        }

        public void UpdateRouteData(string routeData)
        {
            RouteData = routeData;
            MarkAsUpdated();
        }

        public void UpdateWeatherConditions(string weatherConditions)
        {
            WeatherConditions = weatherConditions;
            MarkAsUpdated();
        }

        private void CalculateCaloriesBurned()
        {
            // Simplified calorie calculation: MET * weight * time
            // In a real app, you'd get the user's weight from their profile
            var defaultWeight = 70m; // kg - would come from user profile
            var hours = (decimal)Duration.TotalHours;
            CaloriesBurned = Activity.MetValue * defaultWeight * hours;
        }

        private void CalculateExperiencePoints()
        {
            // Base XP calculation
            var basePoints = (int)Duration.TotalMinutes; // 1 XP per minute
            
            // Bonus points for different factors
            var distanceBonus = Distance.HasValue ? (int)(Distance.Value * 10) : 0; // 10 XP per km
            var categoryBonus = Activity.Category switch
            {
                ActivityCategory.Cardio => 5,
                ActivityCategory.Strength => 8,
                ActivityCategory.Sports => 10,
                ActivityCategory.Outdoor => 12,
                _ => 0
            };

            ExperiencePointsEarned = basePoints + distanceBonus + categoryBonus;
        }

        public decimal GetAverageSpeed()
        {
            if (!Distance.HasValue || Duration.TotalHours == 0)
                return 0;

            return Distance.Value / (decimal)Duration.TotalHours; // km/h
        }

        public string GetFormattedDuration()
        {
            if (Duration.TotalHours >= 1)
                return $"{Duration.Hours}h {Duration.Minutes}m";
            
            return $"{Duration.Minutes}m {Duration.Seconds}s";
        }
    }

    public enum ActivityType
    {
        Indoor,
        Outdoor,
        Virtual,
        Manual
    }

    public enum ActivityCategory
    {
        Cardio,
        Strength,
        Flexibility,
        Sports,
        Recreation,
        Outdoor,
        Water,
        Winter,
        Martial_Arts,
        Dance,
        Other
    }
}
