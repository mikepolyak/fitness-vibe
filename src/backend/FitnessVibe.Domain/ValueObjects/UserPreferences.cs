using System;

namespace FitnessVibe.Domain.ValueObjects
{
    /// <summary>
    /// User preferences - think of this as the user's "settings" or "personality" in our app.
    /// Like how you might set your phone to dark mode or choose notification preferences,
    /// these settings shape how the user experiences their fitness journey.
    /// </summary>
    public class UserPreferences
    {
        /// <summary>
        /// The user's preferred time zone (e.g., "UTC", "America/New_York")
        /// </summary>
        public required string TimeZone { get; init; } = "UTC";

        /// <summary>
        /// Whether the user wants to receive app notifications
        /// </summary>
        public required bool AllowNotifications { get; init; } = true;

        /// <summary>
        /// Whether the user's activities are visible to the public
        /// </summary>
        public required bool ShareActivitiesPublicly { get; init; } = false;

        /// <summary>
        /// Whether the user wants to receive motivational messages
        /// </summary>
        public required bool ReceiveMotivationalMessages { get; init; } = true;

        /// <summary>
        /// Whether other users can send friend requests
        /// </summary>
        public required bool AllowFriendRequests { get; init; } = true;

        /// <summary>
        /// When quiet hours begin (24-hour format, 0-23)
        /// </summary>
        public required int QuietHourStart { get; init; } = 22;

        /// <summary>
        /// When quiet hours end (24-hour format, 0-23)
        /// </summary>
        public required int QuietHourEnd { get; init; } = 7;

        /// <summary>
        /// Preferred measurement system ("metric" or "imperial")
        /// </summary>
        public required string PreferredUnits { get; init; } = "metric";

        /// <summary>
        /// Whether to play audio cues during activities
        /// </summary>
        public required bool EnableAudioCues { get; init; } = true;

        /// <summary>
        /// Whether to share achievements to social media
        /// </summary>
        public required bool ShareToSocialMedia { get; init; } = false;

        /// <summary>
        /// Creates a new UserPreferences instance with default values
        /// </summary>
        public static UserPreferences Default() => new()
        {
            TimeZone = "UTC",
            AllowNotifications = true,
            ShareActivitiesPublicly = false,
            ReceiveMotivationalMessages = true,
            AllowFriendRequests = true,
            QuietHourStart = 22,
            QuietHourEnd = 7,
            PreferredUnits = "metric",
            EnableAudioCues = true,
            ShareToSocialMedia = false
        };

        /// <summary>
        /// Creates a new UserPreferences instance with the specified values
        /// </summary>
        public static UserPreferences Create(
            string? timeZone = null,
            bool? allowNotifications = null,
            bool? shareActivitiesPublicly = null,
            bool? receiveMotivationalMessages = null,
            bool? allowFriendRequests = null,
            int? quietHourStart = null,
            int? quietHourEnd = null,
            string? preferredUnits = null,
            bool? enableAudioCues = null,
            bool? shareToSocialMedia = null)
        {
            var tz = timeZone ?? "UTC";
            var units = preferredUnits ?? "metric";
            var startHour = quietHourStart ?? 22;
            var endHour = quietHourEnd ?? 7;

            ValidateHour(startHour, nameof(quietHourStart));
            ValidateHour(endHour, nameof(quietHourEnd));
            ValidateUnits(units);

            return new()
            {
                TimeZone = tz,
                AllowNotifications = allowNotifications ?? true,
                ShareActivitiesPublicly = shareActivitiesPublicly ?? false,
                ReceiveMotivationalMessages = receiveMotivationalMessages ?? true,
                AllowFriendRequests = allowFriendRequests ?? true,
                QuietHourStart = startHour,
                QuietHourEnd = endHour,
                PreferredUnits = units,
                EnableAudioCues = enableAudioCues ?? true,
                ShareToSocialMedia = shareToSocialMedia ?? false
            };
        }

        /// <summary>
        /// Creates a new UserPreferences instance with some values updated
        /// </summary>
        public UserPreferences With(
            string? timeZone = null,
            bool? allowNotifications = null,
            bool? shareActivitiesPublicly = null,
            bool? receiveMotivationalMessages = null,
            bool? allowFriendRequests = null,
            int? quietHourStart = null,
            int? quietHourEnd = null,
            string? preferredUnits = null,
            bool? enableAudioCues = null,
            bool? shareToSocialMedia = null)
        {
            if (quietHourStart.HasValue)
                ValidateHour(quietHourStart.Value, nameof(quietHourStart));
            if (quietHourEnd.HasValue)
                ValidateHour(quietHourEnd.Value, nameof(quietHourEnd));
            if (preferredUnits != null)
                ValidateUnits(preferredUnits);

            return new()
            {
                TimeZone = timeZone ?? this.TimeZone,
                AllowNotifications = allowNotifications ?? this.AllowNotifications,
                ShareActivitiesPublicly = shareActivitiesPublicly ?? this.ShareActivitiesPublicly,
                ReceiveMotivationalMessages = receiveMotivationalMessages ?? this.ReceiveMotivationalMessages,
                AllowFriendRequests = allowFriendRequests ?? this.AllowFriendRequests,
                QuietHourStart = quietHourStart ?? this.QuietHourStart,
                QuietHourEnd = quietHourEnd ?? this.QuietHourEnd,
                PreferredUnits = preferredUnits ?? this.PreferredUnits,
                EnableAudioCues = enableAudioCues ?? this.EnableAudioCues,
                ShareToSocialMedia = shareToSocialMedia ?? this.ShareToSocialMedia
            };
        }

        private static int ValidateHour(int hour, string paramName)
        {
            if (hour < 0 || hour > 23)
                throw new ArgumentOutOfRangeException(paramName, hour, "Hour must be between 0 and 23");
            return hour;
        }

        private static string ValidateUnits(string units)
        {
            if (string.IsNullOrWhiteSpace(units))
                throw new ArgumentException("Units cannot be empty", nameof(units));

            return units.ToLower() switch
            {
                "metric" or "imperial" => units.ToLower(),
                _ => throw new ArgumentException("Units must be either 'metric' or 'imperial'", nameof(units))
            };
        }

        /// <summary>
        /// Checks if another UserPreferences object has the same values as this one
        /// </summary>
        /// <param name="obj">The object to compare with</param>
        /// <returns>True if the objects have the same values, false otherwise</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (UserPreferences)obj;
            return TimeZone == other.TimeZone &&
                   AllowNotifications == other.AllowNotifications &&
                   ShareActivitiesPublicly == other.ShareActivitiesPublicly &&
                   ReceiveMotivationalMessages == other.ReceiveMotivationalMessages &&
                   AllowFriendRequests == other.AllowFriendRequests &&
                   QuietHourStart == other.QuietHourStart &&
                   QuietHourEnd == other.QuietHourEnd &&
                   PreferredUnits == other.PreferredUnits &&
                   EnableAudioCues == other.EnableAudioCues &&
                   ShareToSocialMedia == other.ShareToSocialMedia;
        }

        /// <summary>
        /// Generates a hash code based on all the preference values
        /// </summary>
        /// <returns>A hash code that can be used for comparisons and collections</returns>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(TimeZone);
            hashCode.Add(AllowNotifications);
            hashCode.Add(ShareActivitiesPublicly);
            hashCode.Add(ReceiveMotivationalMessages);
            hashCode.Add(AllowFriendRequests);
            hashCode.Add(QuietHourStart);
            hashCode.Add(QuietHourEnd);
            hashCode.Add(PreferredUnits);
            hashCode.Add(EnableAudioCues);
            hashCode.Add(ShareToSocialMedia);
            return hashCode.ToHashCode();
        }
    }
}
