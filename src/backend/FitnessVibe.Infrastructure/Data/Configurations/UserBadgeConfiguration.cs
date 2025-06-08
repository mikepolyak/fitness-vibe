using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessVibe.Domain.Entities.Gamification;

namespace FitnessVibe.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for the UserBadge entity.
    /// Configures the database mapping, relationships, indexes and constraints.
    /// </summary>
    public class UserBadgeConfiguration : IEntityTypeConfiguration<UserBadge>
    {
        /// <summary>
        /// Configures the UserBadge entity
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<UserBadge> builder)
        {
            builder.ToTable("UserBadges");

            // Key configuration
            builder.HasKey(ub => ub.Id);
            builder.Property(ub => ub.Id)
                .ValueGeneratedNever()
                .HasColumnType("uniqueidentifier");

            // Audit properties
            builder.Property(ub => ub.CreatedAt)
                .IsRequired();
            
            builder.Property(ub => ub.UpdatedAt)
                .IsRequired(false)
                .IsConcurrencyToken();

            builder.Property(ub => ub.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Core properties
            builder.Property(ub => ub.EarnedAt)
                .IsRequired();

            builder.Property(ub => ub.EarnedContext)
                .HasMaxLength(2000);

            builder.Property(ub => ub.IsVisible)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasOne(ub => ub.User)
                .WithMany(u => u.Badges)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne(ub => ub.Badge)
                .WithMany()
                .HasForeignKey(ub => ub.BadgeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Indexes
            builder.HasIndex(ub => ub.UserId);
            builder.HasIndex(ub => ub.BadgeId);
            builder.HasIndex(ub => ub.EarnedAt);
            builder.HasIndex(ub => ub.IsDeleted);
            builder.HasIndex(ub => ub.IsVisible);

            // Unique constraint to prevent duplicate badges for a user
            builder.HasIndex(ub => new { ub.UserId, ub.BadgeId }).IsUnique();

            // Query filter
            builder.HasQueryFilter(ub => !ub.IsDeleted);
        }
    }
}
