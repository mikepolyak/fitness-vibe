using FluentValidation;
using FitnessVibe.Application.Commands.Social;

namespace FitnessVibe.Application.Validators.Social
{
    /// <summary>
    /// Validator for sending cheers - the encouragement quality inspector.
    /// This ensures all cheers and motivational messages meet community standards,
    /// like having a moderator review messages before they reach workout buddies.
    /// </summary>
    public class SendCheerCommandValidator : AbstractValidator<SendCheerCommand>
    {
        public SendCheerCommandValidator()
        {
            // User ID validation
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("Valid sender user ID is required");

            RuleFor(x => x.TargetUserId)
                .GreaterThan(0)
                .WithMessage("Valid target user ID is required");

            // Can't cheer yourself (though that would be amusing!)
            RuleFor(x => x)
                .Must(x => x.UserId != x.TargetUserId)
                .WithMessage("Cannot send cheers to yourself");

            // Activity ID validation
            RuleFor(x => x.ActivityId)
                .GreaterThan(0)
                .WithMessage("Valid activity ID is required")
                .When(x => x.ActivityId.HasValue);

            // Cheer type validation - like ensuring the type of encouragement is appropriate
            RuleFor(x => x.CheerType)
                .NotEmpty()
                .WithMessage("Cheer type is required")
                .Must(BeValidCheerType)
                .WithMessage("Please select a valid cheer type (Text, Audio, Emoji, PowerUp)");

            // Message validation for text cheers
            RuleFor(x => x.Message)
                .NotEmpty()
                .WithMessage("Message is required for text cheers")
                .When(x => x.CheerType.Equals("Text", StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x.Message)
                .MaximumLength(280)
                .WithMessage("Cheer message is too long (maximum 280 characters)")
                .When(x => !string.IsNullOrEmpty(x.Message));

            RuleFor(x => x.Message)
                .Must(BeAppropriateMessage)
                .WithMessage("Message contains inappropriate content")
                .When(x => !string.IsNullOrEmpty(x.Message));

            // Audio URL validation for audio cheers
            RuleFor(x => x.AudioUrl)
                .NotEmpty()
                .WithMessage("Audio URL is required for audio cheers")
                .When(x => x.CheerType.Equals("Audio", StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x.AudioUrl)
                .Must(BeValidUrl)
                .WithMessage("Please provide a valid audio URL")
                .When(x => !string.IsNullOrEmpty(x.AudioUrl));

            // Emoji code validation for emoji cheers
            RuleFor(x => x.EmojiCode)
                .NotEmpty()
                .WithMessage("Emoji code is required for emoji cheers")
                .When(x => x.CheerType.Equals("Emoji", StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x.EmojiCode)
                .Must(BeValidEmojiCode)
                .WithMessage("Please provide a valid emoji code")
                .When(x => !string.IsNullOrEmpty(x.EmojiCode));

            // Power-up value validation for power-up cheers
            RuleFor(x => x.PowerUpValue)
                .GreaterThan(0)
                .WithMessage("Power-up value must be greater than 0")
                .LessThanOrEqualTo(100)
                .WithMessage("Power-up value cannot exceed 100 XP")
                .When(x => x.CheerType.Equals("PowerUp", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Validates that the cheer type is supported.
        /// Like ensuring the type of encouragement is available in our fitness app.
        /// </summary>
        private bool BeValidCheerType(string cheerType)
        {
            var validCheerTypes = new[] { "Text", "Audio", "Emoji", "PowerUp" };
            return validCheerTypes.Contains(cheerType, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Validates that the message content is appropriate for the fitness community.
        /// Like having a content moderator ensure messages are positive and supportive.
        /// </summary>
        private bool BeAppropriateMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return true;

            // Basic inappropriate content filter
            var inappropriateWords = new[]
            {
                // Add inappropriate words/phrases that shouldn't be in fitness cheers
                "spam", "hate", "stupid", "ugly", "loser", "quit", "give up"
            };

            var lowerMessage = message.ToLower();
            return !inappropriateWords.Any(word => lowerMessage.Contains(word));
        }

        /// <summary>
        /// Validates that the URL is properly formatted.
        /// Like ensuring audio cheer links actually work.
        /// </summary>
        private bool BeValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return true;

            try
            {
                var uri = new Uri(url);
                return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates that the emoji code is in a supported format.
        /// Like ensuring emoji cheers will display correctly.
        /// </summary>
        private bool BeValidEmojiCode(string emojiCode)
        {
            if (string.IsNullOrWhiteSpace(emojiCode))
                return true;

            // Basic emoji code validation (could be enhanced with a proper emoji library)
            // Supports Unicode emoji codes like U+1F44D or shortcodes like :thumbs_up:
            return emojiCode.StartsWith("U+") && emojiCode.Length >= 6 ||
                   emojiCode.StartsWith(":") && emojiCode.EndsWith(":") && emojiCode.Length >= 3;
        }
    }
}
