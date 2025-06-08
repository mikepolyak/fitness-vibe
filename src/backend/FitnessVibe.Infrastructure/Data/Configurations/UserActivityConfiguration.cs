using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessVibe.Domain.Entities.Activities;

namespace FitnessVibe.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuration for the UserActivity entity
    /// </summary>
    public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
    {
        public void Configure(EntityTypeBuilder<UserActivity> builder)
        {
            builder.ToTable("UserActivities");

            builder.Property(ua => ua.Id)
                .ValueGeneratedNever();

            builder.Property(ua => ua.CreatedAt)
                .IsRequired();

            builder.Property(ua => ua.StartedAt)
                .IsRequired();

            builder.Property(ua => ua.DurationMinutes)
                .IsRequired();

            builder.Property(ua => ua.IntensityLevel)
                .IsRequired();

            builder.Property(ua => ua.CaloriesBurned)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(ua => ua.ExperiencePointsEarned)
                .IsRequired();

            builder.Property(ua => ua.Notes)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(ua => ua.User)
                .WithMany(u => u.Activities)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ua => ua.Activity)
                .WithMany(a => a.UserActivities)
                .HasForeignKey(ua => ua.ActivityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for performance
            builder.HasIndex(ua => ua.UserId);
            builder.HasIndex(ua => ua.ActivityId);
            builder.HasIndex(ua => ua.CompletedAt);
            builder.HasIndex(ua => ua.StartedAt);
            builder.HasIndex(ua => ua.IntensityLevel);
            builder.HasIndex(ua => ua.CreatedAt);

            // Filters
            builder.HasQueryFilter(ua => !ua.IsDeleted);
        }
    }
}
