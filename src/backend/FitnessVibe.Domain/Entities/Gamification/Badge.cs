using System;
using FitnessVibe.Domain.Common;

namespace FitnessVibe.Domain.Entities.Gamification
{
    /// <summary>
    /// Badge entity - like merit badges in scouting or achievements in video games.
    /// Each badge represents a milestone or accomplishment in the user's fitness journey.
    /// Think of badges as digital trophies that tell the story of what a user has achieved.
    /// </summary>
    public class Badge : BaseEntity
    {
        /// <summary>
        /// Gets the name of the badge
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the description of what this badge represents
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the URL to the badge's icon image
        /// </summary>
        public string IconUrl { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the category this badge belongs to
        /// </summary>
        public BadgeCategory Category { get; private set; }

        /// <summary>
        /// Gets the rarity level of this badge
        /// </summary>
        public BadgeRarity Rarity { get; private set; }

        /// <summary>
        /// Gets the XP points awarded when this badge is earned
        /// </summary>
        public int Points { get; private set; }

        /// <summary>
        /// Gets the JSON criteria for earning this badge
        /// </summary>
        public string Criteria { get; private set; } = string.Empty;

        /// <summary>
        /// Gets whether this badge can currently be earned
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Protected constructor for EF Core
        /// </summary>
        protected Badge() { }

        /// <summary>
        /// Creates a new badge with the specified details
        /// </summary>
        /// <param name="name">The name of the badge</param>
        /// <param name="description">What the badge represents</param>
        /// <param name="iconUrl">URL to the badge's icon</param>
        /// <param name="category">The badge category</param>
        /// <param name="rarity">How rare/difficult the badge is to earn</param>
        /// <param name="points">XP points awarded when earned</param>
        /// <param name="criteria">JSON criteria for earning the badge</param>
        /// <returns>A new Badge instance</returns>
        public static Badge Create(
            string name,
            string description,
            string iconUrl,
            BadgeCategory category,
            BadgeRarity rarity,
            int points,
            string criteria)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Badge name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Badge description cannot be empty", nameof(description));

            if (string.IsNullOrWhiteSpace(iconUrl))
                throw new ArgumentException("Badge icon URL cannot be empty", nameof(iconUrl));

            if (points < 0)
                throw new ArgumentException("Points cannot be negative", nameof(points));

            if (string.IsNullOrWhiteSpace(criteria))
                throw new ArgumentException("Badge criteria cannot be empty", nameof(criteria));

            var badge = new Badge
            {
                Name = name,
                Description = description,
                IconUrl = iconUrl,
                Category = category,
                Rarity = rarity,
                Points = points,
                Criteria = criteria,
                IsActive = true
            };

            return badge;
        }

        /// <summary>
        /// Updates the badge's details
        /// </summary>
        /// <param name="name">New name for the badge</param>
        /// <param name="description">New description of what the badge represents</param>
        /// <param name="iconUrl">New URL to the badge's icon</param>
        public void UpdateDetails(string name, string description, string iconUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Badge name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Badge description cannot be empty", nameof(description));

            if (string.IsNullOrWhiteSpace(iconUrl))
                throw new ArgumentException("Badge icon URL cannot be empty", nameof(iconUrl));

            Name = name;
            Description = description;
            IconUrl = iconUrl;
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates the badge criteria
        /// </summary>
        /// <param name="criteria">New JSON criteria for earning the badge</param>
        public void UpdateCriteria(string criteria)
        {
            if (string.IsNullOrWhiteSpace(criteria))
                throw new ArgumentException("Badge criteria cannot be empty", nameof(criteria));

            Criteria = criteria;
            MarkAsUpdated();
        }

        /// <summary>
        /// Marks the badge as inactive so it can no longer be earned
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive)
                return;

            IsActive = false;
            MarkAsUpdated();
        }

        /// <summary>
        /// Marks the badge as active so it can be earned
        /// </summary>
        public void Activate()
        {
            if (IsActive)
                return;

            IsActive = true;
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Categories that organize badges into meaningful groups
    /// </summary>
    public enum BadgeCategory
    {
        /// <summary>
        /// Badges awarded for completing activities
        /// </summary>
        Activity,

        /// <summary>
        /// Badges awarded for maintaining activity streaks
        /// </summary>
        Streak,

        /// <summary>
        /// Badges awarded for social interactions
        /// </summary>
        Social,

        /// <summary>
        /// Badges awarded for completing challenges
        /// </summary>
        Challenge,

        /// <summary>
        /// Badges awarded for reaching significant milestones
        /// </summary>
        Milestone,

        /// <summary>
        /// Badges awarded during special events
        /// </summary>
        Special,

        /// <summary>
        /// Badges awarded for general achievements
        /// </summary>
        Achievement
    }

    /// <summary>
    /// Indicates how difficult or rare a badge is to obtain
    /// </summary>
    public enum BadgeRarity
    {
        /// <summary>
        /// Easy to earn, frequently awarded badges
        /// </summary>
        Common,

        /// <summary>
        /// Moderately difficult badges requiring consistent effort
        /// </summary>
        Uncommon,

        /// <summary>
        /// Difficult badges indicating significant achievements
        /// </summary>
        Rare,

        /// <summary>
        /// Very difficult badges marking major accomplishments
        /// </summary>
        Epic,

        /// <summary>
        /// The most prestigious badges for extraordinary achievements
        /// </summary>
        Legendary
    }
}
