using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessVibe.Domain.Entities.Gamification;

namespace FitnessVibe.Infrastructure.Data.Configurations
{
    public class UserBadgeConfiguration : IEntityTypeConfiguration<UserBadge>
    {
        public void Configure(EntityTypeBuilder<UserBadge> builder)
        {
            builder.ToTable("UserBadges");

            builder.Property(ub => ub.AwardedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(ub => ub.User)
                .WithMany(u => u.Badges)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ub => ub.Badge)
                .WithMany()
                .HasForeignKey(ub => ub.BadgeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(ub => ub.UserId);
            builder.HasIndex(ub => ub.BadgeId);
            builder.HasIndex(ub => ub.AwardedAt);

            // Add unique constraint to prevent duplicate badges for a user
            builder.HasIndex(ub => new { ub.UserId, ub.BadgeId }).IsUnique();
        }
    }
}
