using Microsoft.EntityFrameworkCore;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.Entities.Gamification;
using FitnessVibe.Domain.Entities.Social;
using FitnessVibe.Domain.Common;
using System.Reflection;

namespace FitnessVibe.Infrastructure.Data
{
    /// <summary>
    /// Application Database Context - the central hub for all data operations.
    /// Think of this as the main database manager for our fitness center,
    /// coordinating all the member records, activity logs, and achievement tracking.
    /// </summary>
    public class FitnessVibeDbContext : DbContext
    {
        public FitnessVibeDbContext(DbContextOptions<FitnessVibeDbContext> options)
            : base(options)
        {
        }

        // User-related entities
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserGoal> UserGoals { get; set; } = null!;
        public DbSet<UserBadge> UserBadges { get; set; } = null!;

        // Activity-related entities
        public DbSet<Activity> Activities { get; set; } = null!;
        public DbSet<UserActivity> UserActivities { get; set; } = null!;

        // Gamification entities
        public DbSet<Badge> Badges { get; set; } = null!;

        // Social entities (to be added in future iterations)
        // public DbSet<Club> Clubs { get; set; } = null!;
        // public DbSet<Friendship> Friendships { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations from the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configure base entity properties globally
            ConfigureBaseEntity(modelBuilder);

            // Configure global query filters for soft deletion
            ConfigureGlobalFilters(modelBuilder);

            // Seed initial data
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Configure common properties for all entities that inherit from BaseEntity.
        /// Like setting up standard membership card features for all gym members.
        /// </summary>
        private static void ConfigureBaseEntity(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // Configure primary key
                    modelBuilder.Entity(entityType.ClrType)
                        .HasKey(nameof(BaseEntity.Id));

                    // Configure audit fields
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(BaseEntity.CreatedAt))
                        .HasDefaultValueSql("GETUTCDATE()");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(BaseEntity.UpdatedAt))
                        .HasDefaultValue(null);

                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(BaseEntity.IsDeleted))
                        .HasDefaultValue(false);

                    // Ignore domain events (not persisted)
                    modelBuilder.Entity(entityType.ClrType)
                        .Ignore(nameof(BaseEntity.DomainEvents));
                }
            }
        }

        /// <summary>
        /// Configure global query filters for soft deletion.
        /// Like automatically filtering out cancelled memberships from normal queries.
        /// </summary>
        private static void ConfigureGlobalFilters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var condition = Expression.Equal(property, Expression.Constant(false));
                    var lambda = Expression.Lambda(condition, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        /// <summary>
        /// Seed initial data for the application.
        /// Like setting up the initial gym equipment and activity types.
        /// </summary>
        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Activities
            modelBuilder.Entity<Activity>().HasData(
                new Activity[]
                {
                    // Cardio Activities
                    new(1, "Running", ActivityType.Outdoor, ActivityCategory.Cardio, 8.0m, "Outdoor running", "/assets/icons/running.svg"),
                    new(2, "Walking", ActivityType.Outdoor, ActivityCategory.Cardio, 3.5m, "Outdoor walking", "/assets/icons/walking.svg"),
                    new(3, "Cycling", ActivityType.Outdoor, ActivityCategory.Cardio, 7.5m, "Outdoor cycling", "/assets/icons/cycling.svg"),
                    new(4, "Treadmill", ActivityType.Indoor, ActivityCategory.Cardio, 8.5m, "Treadmill running", "/assets/icons/treadmill.svg"),
                    new(5, "Elliptical", ActivityType.Indoor, ActivityCategory.Cardio, 6.0m, "Elliptical machine", "/assets/icons/elliptical.svg"),
                    
                    // Strength Training
                    new(6, "Weight Training", ActivityType.Indoor, ActivityCategory.Strength, 4.5m, "Free weights and machines", "/assets/icons/weights.svg"),
                    new(7, "Bodyweight", ActivityType.Indoor, ActivityCategory.Strength, 3.8m, "Bodyweight exercises", "/assets/icons/bodyweight.svg"),
                    new(8, "CrossFit", ActivityType.Indoor, ActivityCategory.Strength, 7.0m, "High-intensity functional training", "/assets/icons/crossfit.svg"),
                    
                    // Flexibility & Recovery
                    new(9, "Yoga", ActivityType.Indoor, ActivityCategory.Flexibility, 2.5m, "Yoga practice", "/assets/icons/yoga.svg"),
                    new(10, "Stretching", ActivityType.Indoor, ActivityCategory.Flexibility, 2.0m, "Stretching routine", "/assets/icons/stretching.svg"),
                    new(11, "Pilates", ActivityType.Indoor, ActivityCategory.Flexibility, 3.0m, "Pilates workout", "/assets/icons/pilates.svg"),
                    
                    // Sports
                    new(12, "Tennis", ActivityType.Outdoor, ActivityCategory.Sports, 6.0m, "Tennis match or practice", "/assets/icons/tennis.svg"),
                    new(13, "Basketball", ActivityType.Outdoor, ActivityCategory.Sports, 7.0m, "Basketball game", "/assets/icons/basketball.svg"),
                    new(14, "Soccer", ActivityType.Outdoor, ActivityCategory.Sports, 8.0m, "Soccer/football game", "/assets/icons/soccer.svg"),
                    
                    // Swimming
                    new(15, "Swimming", ActivityType.Indoor, ActivityCategory.Water, 8.0m, "Pool swimming", "/assets/icons/swimming.svg"),
                    new(16, "Water Aerobics", ActivityType.Indoor, ActivityCategory.Water, 4.0m, "Water aerobics class", "/assets/icons/water-aerobics.svg")
                }.Select((activity, index) => 
                {
                    // Use reflection to create activities with proper IDs
                    var activityInstance = (Activity)Activator.CreateInstance(typeof(Activity), true)!;
                    typeof(Activity).GetProperty("Id")!.SetValue(activityInstance, index + 1);
                    typeof(Activity).GetProperty("Name")!.SetValue(activityInstance, activity.Name);
                    typeof(Activity).GetProperty("Type")!.SetValue(activityInstance, activity.Type);
                    typeof(Activity).GetProperty("Category")!.SetValue(activityInstance, activity.Category);
                    typeof(Activity).GetProperty("MetValue")!.SetValue(activityInstance, activity.MetValue);
                    typeof(Activity).GetProperty("Description")!.SetValue(activityInstance, activity.Description);
                    typeof(Activity).GetProperty("IconUrl")!.SetValue(activityInstance, activity.IconUrl);
                    typeof(Activity).GetProperty("IsActive")!.SetValue(activityInstance, true);
                    return activityInstance;
                }).ToArray()
            );

            // Seed Badges
            modelBuilder.Entity<Badge>().HasData(
                new Badge[]
                {
                    // Welcome badges
                    CreateBadge(1, "Welcome to FitnessVibe", "Completed registration and profile setup", 
                        BadgeCategory.Achievement, BadgeRarity.Common, 50, "/assets/badges/welcome.svg"),
                    CreateBadge(2, "First Steps", "Logged your first activity", 
                        BadgeCategory.Activity, BadgeRarity.Common, 100, "/assets/badges/first-steps.svg"),
                    
                    // Activity badges
                    CreateBadge(3, "Early Bird", "Completed a workout before 7 AM", 
                        BadgeCategory.Activity, BadgeRarity.Uncommon, 150, "/assets/badges/early-bird.svg"),
                    CreateBadge(4, "Night Owl", "Completed a workout after 10 PM", 
                        BadgeCategory.Activity, BadgeRarity.Uncommon, 150, "/assets/badges/night-owl.svg"),
                    
                    // Streak badges
                    CreateBadge(5, "Consistency", "Maintained a 7-day activity streak", 
                        BadgeCategory.Streak, BadgeRarity.Uncommon, 300, "/assets/badges/consistency.svg"),
                    CreateBadge(6, "Dedication", "Maintained a 30-day activity streak", 
                        BadgeCategory.Streak, BadgeRarity.Rare, 1000, "/assets/badges/dedication.svg"),
                    CreateBadge(7, "Unstoppable", "Maintained a 100-day activity streak", 
                        BadgeCategory.Streak, BadgeRarity.Epic, 5000, "/assets/badges/unstoppable.svg"),
                    
                    // Distance milestones
                    CreateBadge(8, "First Mile", "Ran or walked your first mile", 
                        BadgeCategory.Milestone, BadgeRarity.Common, 200, "/assets/badges/first-mile.svg"),
                    CreateBadge(9, "5K Finisher", "Completed a 5 kilometer distance", 
                        BadgeCategory.Milestone, BadgeRarity.Uncommon, 500, "/assets/badges/5k-finisher.svg"),
                    CreateBadge(10, "Marathon Runner", "Completed a marathon distance (42.2 km)", 
                        BadgeCategory.Milestone, BadgeRarity.Legendary, 10000, "/assets/badges/marathon.svg"),
                    
                    // Social badges
                    CreateBadge(11, "Team Player", "Joined your first club", 
                        BadgeCategory.Social, BadgeRarity.Common, 200, "/assets/badges/team-player.svg"),
                    CreateBadge(12, "Motivator", "Gave 100 kudos to other members", 
                        BadgeCategory.Social, BadgeRarity.Rare, 1000, "/assets/badges/motivator.svg"),
                    
                    // Special achievement badges
                    CreateBadge(13, "Level Up!", "Reached Level 10", 
                        BadgeCategory.Achievement, BadgeRarity.Rare, 2000, "/assets/badges/level-up.svg"),
                    CreateBadge(14, "Fitness Legend", "Reached Level 50", 
                        BadgeCategory.Achievement, BadgeRarity.Legendary, 25000, "/assets/badges/legend.svg")
                }
            );
        }

        /// <summary>
        /// Helper method to create badge seed data.
        /// Like setting up achievement certificates with all the right details.
        /// </summary>
        private static Badge CreateBadge(int id, string name, string description, 
            BadgeCategory category, BadgeRarity rarity, int points, string iconUrl)
        {
            // Use reflection to create a badge with the private constructor
            var badge = (Badge)Activator.CreateInstance(typeof(Badge), true)!;
            typeof(Badge).GetProperty("Id")!.SetValue(badge, id);
            typeof(Badge).GetProperty("Name")!.SetValue(badge, name);
            typeof(Badge).GetProperty("Description")!.SetValue(badge, description);
            typeof(Badge).GetProperty("Category")!.SetValue(badge, category);
            typeof(Badge).GetProperty("Rarity")!.SetValue(badge, rarity);
            typeof(Badge).GetProperty("Points")!.SetValue(badge, points);
            typeof(Badge).GetProperty("IconUrl")!.SetValue(badge, iconUrl);
            typeof(Badge).GetProperty("IsActive")!.SetValue(badge, true);
            
            // Set criteria as JSON string
            var criteria = new
            {
                type = category.ToString().ToLower(),
                requirements = new { description = description }
            };
            typeof(Badge).GetProperty("Criteria")!.SetValue(badge, 
                System.Text.Json.JsonSerializer.Serialize(criteria));
            
            return badge;
        }

        /// <summary>
        /// Save changes with domain event processing.
        /// Like processing all membership updates and triggering appropriate notifications.
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Update timestamps for modified entities
            var entries = ChangeTracker.Entries<BaseEntity>();
            
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.MarkAsUpdated();
                }
            }

            // Process domain events (implement domain event dispatcher here)
            var domainEvents = entries
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            // Clear domain events
            foreach (var entry in entries)
            {
                entry.Entity.ClearDomainEvents();
            }

            // Save changes
            var result = await base.SaveChangesAsync(cancellationToken);

            // Dispatch domain events after successful save
            // TODO: Implement domain event dispatcher
            // await _domainEventDispatcher.DispatchEventsAsync(domainEvents);

            return result;
        }
    }

    // Extension class for Activity creation in seed data
    file record Activity(int Id, string Name, ActivityType Type, ActivityCategory Category, 
        decimal MetValue, string? Description, string? IconUrl);
}
