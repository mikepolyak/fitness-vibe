using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.Entities.Gamification;
using FitnessVibe.Domain.Entities.Challenges;
using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.Enums;
using System.Reflection;
using MediatR;

namespace FitnessVibe.Infrastructure.Data
{
    /// <summary>
    /// Application Database Context - the central hub for all data operations.
    /// Think of this as the main database manager for our fitness center,
    /// coordinating all the member records, activity logs, and achievement tracking.
    /// </summary>
    public class FitnessVibeDbContext : DbContext
    {
        private readonly IMediator? _mediator;

        /// <summary>
        /// Initializes a new instance of the FitnessVibeDbContext
        /// </summary>
        /// <param name="options">The DbContext options</param>
        /// <param name="mediator">Optional mediator for publishing domain events</param>
        public FitnessVibeDbContext(DbContextOptions<FitnessVibeDbContext> options, IMediator? mediator = null)
            : base(options)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets or sets the users in the system
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user goals
        /// </summary>
        public DbSet<UserGoal> UserGoals { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user badges
        /// </summary>
        public DbSet<UserBadge> UserBadges { get; set; } = null!;

        /// <summary>
        /// Gets or sets the available activities
        /// </summary>
        public DbSet<Activity> Activities { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user activity records
        /// </summary>
        public DbSet<UserActivity> UserActivities { get; set; } = null!;

        /// <summary>
        /// Gets or sets the available badges
        /// </summary>
        public DbSet<Badge> Badges { get; set; } = null!;

        /// <summary>
        /// Gets or sets the challenges in the system
        /// </summary>
        public DbSet<Challenge> Challenges { get; set; } = null!;

        /// <summary>
        /// Gets or sets the challenge participants
        /// </summary>
        public DbSet<ChallengeParticipant> ChallengeParticipants { get; set; } = null!;

        /// <summary>
        /// Configures the model for this context
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations from the assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configure base entity properties globally
            ConfigureBaseEntity(modelBuilder);

            // Configure global query filters for soft deletion
            ConfigureGlobalFilters(modelBuilder);
        }

        /// <summary>
        /// Configure common properties for all entities that inherit from BaseEntity
        /// </summary>
        private void ConfigureBaseEntity(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime>("CreatedAt")
                        .HasDefaultValueSql("GETUTCDATE()");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime>("UpdatedAt")
                        .HasDefaultValueSql("GETUTCDATE()");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property<bool>("IsDeleted")
                        .HasDefaultValue(false);
                }
            }
        }

        /// <summary>
        /// Configure global query filters for soft deletion
        /// </summary>
        private void ConfigureGlobalFilters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var filter = Expression.Lambda(Expression.Not(property), parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }

        /// <summary>
        /// Saves all changes made in this context to the database and dispatches domain events
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation requests</param>
        /// <returns>The number of state entries written to the database</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Update timestamps for modified entities
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = DateTime.UtcNow;
                        entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Property(nameof(BaseEntity.IsDeleted)).CurrentValue = true;
                        entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
                        break;
                }
            }

            // Save changes first
            var result = await base.SaveChangesAsync(cancellationToken);

            // Then dispatch domain events if mediator is available
            if (_mediator != null)
            {
                var entities = ChangeTracker.Entries<BaseEntity>()
                    .Select(e => e.Entity)
                    .Where(e => e.DomainEvents.Any())
                    .ToList();

                foreach (var entity in entities)
                {
                    var events = entity.DomainEvents.ToArray();
                    entity.ClearDomainEvents();

                    foreach (var domainEvent in events)
                    {
                        await _mediator.Publish(domainEvent, cancellationToken);
                    }
                }
            }

            return result;
        }
    }
}
