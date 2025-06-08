using FluentValidation;

namespace FitnessVibe.Application.Validators
{
    /// <summary>
    /// Base validator - the foundation of good form in our fitness app.
    /// Like how proper form is essential in any exercise, proper validation is essential for data integrity.
    /// This provides common validation rules that all our fitness app validators can build upon.
    /// </summary>
    public abstract class BaseValidator<T> : AbstractValidator<T>
    {
        protected BaseValidator()
        {
            // Set cascade mode to continue validation even after failures
            ClassLevelCascadeMode = CascadeMode.Continue;
        }

        /// <summary>
        /// Validates email format - like checking if someone's gym membership email is valid.
        /// </summary>
        protected void ValidateEmail(string propertyName = "Email")
        {
            RuleFor(x => GetProperty(x, propertyName))
                .NotEmpty()
                .WithMessage($"{propertyName} is required for your fitness account")
                .EmailAddress()
                .WithMessage($"Please provide a valid {propertyName.ToLower()} address")
                .MaximumLength(255)
                .WithMessage($"{propertyName} cannot exceed 255 characters");
        }

        /// <summary>
        /// Validates password strength - like ensuring your gym locker code is secure.
        /// </summary>
        protected void ValidatePassword(string propertyName = "Password")
        {
            RuleFor(x => GetProperty(x, propertyName))
                .NotEmpty()
                .WithMessage($"{propertyName} is required for account security")
                .MinimumLength(8)
                .WithMessage($"{propertyName} must be at least 8 characters long")
                .MaximumLength(128)
                .WithMessage($"{propertyName} cannot exceed 128 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
                .WithMessage($"{propertyName} must contain at least one uppercase letter, one lowercase letter, and one number");
        }

        /// <summary>
        /// Validates names - like ensuring proper names on gym membership cards.
        /// </summary>
        protected void ValidateName(string propertyName)
        {
            RuleFor(x => GetProperty(x, propertyName))
                .NotEmpty()
                .WithMessage($"{propertyName} is required")
                .Length(1, 50)
                .WithMessage($"{propertyName} must be between 1 and 50 characters")
                .Matches(@"^[a-zA-Z\s\-'\.]+$")
                .WithMessage($"{propertyName} can only contain letters, spaces, hyphens, apostrophes, and periods");
        }

        /// <summary>
        /// Validates user ID - like checking if a gym member ID is valid.
        /// </summary>
        protected void ValidateUserId(string propertyName = "UserId")
        {
            RuleFor(x => GetIntProperty(x, propertyName))
                .GreaterThan(0)
                .WithMessage($"{propertyName} must be a valid positive number");
        }

        /// <summary>
        /// Validates activity duration - like ensuring workout times make sense.
        /// </summary>
        protected void ValidateDuration(string propertyName)
        {
            RuleFor(x => GetTimeSpanProperty(x, propertyName))
                .Must(duration => duration.TotalMinutes >= 1 && duration.TotalHours <= 24)
                .WithMessage($"{propertyName} must be between 1 minute and 24 hours");
        }

        /// <summary>
        /// Validates dates are not in the future (for completed activities) - like ensuring you can't log a workout for tomorrow.
        /// </summary>
        protected void ValidatePastDate(string propertyName)
        {
            RuleFor(x => GetDateTimeProperty(x, propertyName))
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage($"{propertyName} cannot be in the future");
        }

        /// <summary>
        /// Validates that a date is within a reasonable range for fitness activities.
        /// </summary>
        protected void ValidateReasonableDate(string propertyName)
        {
            var earliestDate = DateTime.UtcNow.AddYears(-10); // 10 years ago
            var latestDate = DateTime.UtcNow.AddDays(365); // 1 year in the future

            RuleFor(x => GetDateTimeProperty(x, propertyName))
                .GreaterThanOrEqualTo(earliestDate)
                .WithMessage($"{propertyName} cannot be more than 10 years ago")
                .LessThanOrEqualTo(latestDate)
                .WithMessage($"{propertyName} cannot be more than 1 year in the future");
        }

        /// <summary>
        /// Validates positive numbers - like ensuring step counts, weights, and distances are positive.
        /// </summary>
        protected void ValidatePositiveNumber(string propertyName)
        {
            RuleFor(x => GetDoubleProperty(x, propertyName))
                .GreaterThan(0)
                .WithMessage($"{propertyName} must be greater than zero");
        }

        /// <summary>
        /// Validates non-negative numbers - like allowing zero for some metrics that might start at zero.
        /// </summary>
        protected void ValidateNonNegativeNumber(string propertyName)
        {
            RuleFor(x => GetDoubleProperty(x, propertyName))
                .GreaterThanOrEqualTo(0)
                .WithMessage($"{propertyName} cannot be negative");
        }

        // Helper methods to get properties dynamically
        private static string GetProperty(T instance, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            return property?.GetValue(instance)?.ToString() ?? string.Empty;
        }

        private static int GetIntProperty(T instance, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            return (int)(property?.GetValue(instance) ?? 0);
        }

        private static double GetDoubleProperty(T instance, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            return Convert.ToDouble(property?.GetValue(instance) ?? 0);
        }

        private static TimeSpan GetTimeSpanProperty(T instance, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            return (TimeSpan)(property?.GetValue(instance) ?? TimeSpan.Zero);
        }

        private static DateTime GetDateTimeProperty(T instance, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);
            return (DateTime)(property?.GetValue(instance) ?? DateTime.MinValue);
        }
    }
}
