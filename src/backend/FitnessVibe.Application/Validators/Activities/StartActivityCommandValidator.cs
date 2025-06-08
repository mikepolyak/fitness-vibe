using FluentValidation;
using FitnessVibe.Application.Commands.Activities;

namespace FitnessVibe.Application.Validators.Activities
{
    /// <summary>
    /// Validator for starting activity sessions - the workout preparation inspector.
    /// This ensures all workout sessions are started with valid parameters,
    /// like having a personal trainer check your workout plan before you begin.
    /// </summary>
    public class StartActivityCommandValidator : AbstractValidator<StartActivityCommand>
    {
        public StartActivityCommandValidator()
        {
            // User ID validation
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("Valid user ID is required");

            // Activity type validation - like choosing the right workout category
            RuleFor(x => x.ActivityType)
                .NotEmpty()
                .WithMessage("Activity type is required")
                .MaximumLength(50)
                .WithMessage("Activity type name is too long")
                .Must(BeValidActivityType)
                .WithMessage("Please select a valid activity type");

            // Activity name validation
            RuleFor(x => x.ActivityName)
                .MaximumLength(100)
                .WithMessage("Activity name is too long")
                .When(x => !string.IsNullOrEmpty(x.ActivityName));

            // GPS coordinates validation - like ensuring valid location data
            RuleFor(x => x.StartLatitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90 degrees")
                .When(x => x.StartLatitude.HasValue);

            RuleFor(x => x.StartLongitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180 degrees")
                .When(x => x.StartLongitude.HasValue);

            RuleFor(x => x.StartAltitude)
                .GreaterThanOrEqualTo(-500) // Dead Sea level
                .LessThanOrEqualTo(9000) // Mount Everest + buffer
                .WithMessage("Altitude must be realistic (between -500m and 9000m)")
                .When(x => x.StartAltitude.HasValue);

            // GPS consistency - if one coordinate is provided, others should be too
            RuleFor(x => x)
                .Must(HaveCompleteGpsData)
                .WithMessage("If providing GPS coordinates, please include both latitude and longitude")
                .When(x => x.StartLatitude.HasValue || x.StartLongitude.HasValue);

            // Planned duration validation - like setting reasonable workout time expectations
            RuleFor(x => x.PlannedDuration)
                .GreaterThan(TimeSpan.FromMinutes(1))
                .WithMessage("Planned duration must be at least 1 minute")
                .LessThanOrEqualTo(TimeSpan.FromHours(24))
                .WithMessage("Planned duration cannot exceed 24 hours")
                .When(x => x.PlannedDuration.HasValue);

            // Planned start time validation
            RuleFor(x => x.PlannedStartTime)
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddMinutes(-5)) // Allow 5 minutes buffer
                .WithMessage("Planned start time cannot be in the past")
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(30))
                .WithMessage("Planned start time cannot be more than 30 days in the future")
                .When(x => x.PlannedStartTime.HasValue);

            // Notes validation
            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Notes are too long (maximum 500 characters)")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            // Tags validation - like ensuring workout labels are appropriate
            RuleFor(x => x.Tags)
                .Must(tags => tags.Count <= 10)
                .WithMessage("You can add up to 10 tags")
                .Must(tags => tags.All(tag => !string.IsNullOrWhiteSpace(tag) && tag.Length <= 30))
                .WithMessage("Tags must be valid and under 30 characters each");

            // Metadata validation
            RuleFor(x => x.Metadata)
                .Must(metadata => metadata.Count <= 20)
                .WithMessage("Too many metadata entries (maximum 20)");
        }

        /// <summary>
        /// Validates that the activity type is supported by our fitness app.
        /// Like ensuring the workout type is available in our gym.
        /// </summary>
        private bool BeValidActivityType(string activityType)
        {
            var validActivityTypes = new[]
            {
                "Running", "Cycling", "Swimming", "Walking", "Hiking",
                "Gym", "WeightLifting", "Cardio", "Yoga", "Pilates",
                "CrossFit", "Boxing", "MartialArts", "Dance", "Climbing",
                "Rowing", "Skiing", "Snowboarding", "Tennis", "Basketball",
                "Football", "Soccer", "Baseball", "Golf", "Volleyball",
                "Badminton", "TableTennis", "Surfing", "Kayaking", "Paddleboarding",
                "RockClimbing", "Bouldering", "Skateboarding", "Rollerblading",
                "Triathlon", "Duathlon", "Marathon", "HalfMarathon", "5K", "10K",
                "HIIT", "Calisthenics", "Stretching", "Meditation", "Other"
            };

            return validActivityTypes.Contains(activityType, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Ensures GPS data is complete if partially provided.
        /// Like making sure we have full location data for outdoor workouts.
        /// </summary>
        private bool HaveCompleteGpsData(StartActivityCommand command)
        {
            // If any GPS data is provided, both lat and lng should be provided
            var hasLatitude = command.StartLatitude.HasValue;
            var hasLongitude = command.StartLongitude.HasValue;

            if (hasLatitude || hasLongitude)
            {
                return hasLatitude && hasLongitude;
            }

            return true; // No GPS data is fine
        }
    }
}
