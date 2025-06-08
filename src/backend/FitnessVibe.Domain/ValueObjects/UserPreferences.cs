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
        public string TimeZone { get; private set; }
        public bool AllowNotifications { get; private set; }
        public bool ShareActivitiesPublicly { get; private set; }
        public bool ReceiveMotivationalMessages { get; private set; }
        public bool AllowFriendRequests { get; private set; }
        public int QuietHourStart { get; private set; } // 24-hour format
        public int QuietHourEnd { get; private set; }   // 24-hour format
        public string PreferredUnits { get; private set; } // "metric" or "imperial"
        public bool EnableAudioCues { get; private set; }
        public bool ShareToSocialMedia { get; private set; }

        private UserPreferences() { } // For EF Core

        public UserPreferences(
            string timeZone = "UTC",
            bool allowNotifications = true,
            bool shareActivitiesPublicly = false,
            bool receiveMotivationalMessages = true,
            bool allowFriendRequests = true,
            int quietHourStart = 22,
            int quietHourEnd = 7,
            string preferredUnits = "metric",
            bool enableAudioCues = true,
            bool shareToSocialMedia = false)
        {
            TimeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));
            AllowNotifications = allowNotifications;
            ShareActivitiesPublicly = shareActivitiesPublicly;
            ReceiveMotivationalMessages = receiveMotivationalMessages;
            AllowFriendRequests = allowFriendRequests;
            QuietHourStart = ValidateHour(quietHourStart, nameof(quietHourStart));
            QuietHourEnd = ValidateHour(quietHourEnd, nameof(quietHourEnd));
            PreferredUnits = preferredUnits ?? throw new ArgumentNullException(nameof(preferredUnits));
            EnableAudioCues = enableAudioCues;
            ShareToSocialMedia = shareToSocialMedia;
        }

        public static UserPreferences Default() => new UserPreferences();

        public UserPreferences UpdateNotificationSettings(
            bool allowNotifications,
            bool receiveMotivationalMessages,
            int quietHourStart,
            int quietHourEnd)
        {
            return new UserPreferences(
                TimeZone,
                allowNotifications,
                ShareActivitiesPublicly,
                receiveMotivationalMessages,
                AllowFriendRequests,
                quietHourStart,
                quietHourEnd,
                PreferredUnits,
                EnableAudioCues,
                ShareToSocialMedia);
        }

        public UserPreferences UpdatePrivacySettings(
            bool shareActivitiesPublicly,
            bool allowFriendRequests,
            bool shareToSocialMedia)
        {
            return new UserPreferences(
                TimeZone,
                AllowNotifications,
                shareActivitiesPublicly,
                ReceiveMotivationalMessages,
                allowFriendRequests,
                QuietHourStart,
                QuietHourEnd,
                PreferredUnits,
                EnableAudioCues,
                shareToSocialMedia);
        }

        public UserPreferences UpdateDisplaySettings(
            string timeZone,
            string preferredUnits,
            bool enableAudioCues)
        {
            return new UserPreferences(
                timeZone,
                AllowNotifications,
                ShareActivitiesPublicly,
                ReceiveMotivationalMessages,
                AllowFriendRequests,
                QuietHourStart,
                QuietHourEnd,
                preferredUnits,
                enableAudioCues,
                ShareToSocialMedia);
        }

        public bool IsInQuietHours(DateTime dateTime)
        {
            var hour = dateTime.Hour;
            
            // Handle quiet hours that span midnight (e.g., 22:00 to 07:00)
            if (QuietHourStart > QuietHourEnd)
            {
                return hour >= QuietHourStart || hour < QuietHourEnd;
            }
            
            return hour >= QuietHourStart && hour < QuietHourEnd;
        }

        private static int ValidateHour(int hour, string paramName)
        {
            if (hour < 0 || hour > 23)
                throw new ArgumentException("Hour must be between 0 and 23", paramName);
            return hour;
        }

        // Override Equals and GetHashCode for value semantics
        public override bool Equals(object? obj)
        {
            return obj is UserPreferences other &&
                   TimeZone == other.TimeZone &&
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

        public override int GetHashCode()
        {
            return HashCode.Combine(
                TimeZone,
                AllowNotifications,
                ShareActivitiesPublicly,
                ReceiveMotivationalMessages,
                AllowFriendRequests,
                QuietHourStart,
                QuietHourEnd,
                PreferredUnits,
                EnableAudioCues,
                ShareToSocialMedia);
        }
    }
}
