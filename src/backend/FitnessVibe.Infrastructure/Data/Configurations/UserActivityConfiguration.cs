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
        /// <summary>
        /// Configures the UserActivity entity
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<UserActivity> builder)
        {
            builder.ToTable("UserActivities", tb =>
            {
                tb.HasCheckConstraint("CK_UserActivities_DurationMinutes", "[DurationMinutes] > 0");
                tb.HasCheckConstraint("CK_UserActivities_CaloriesBurned", "[CaloriesBurned] >= 0");
                tb.HasCheckConstraint("CK_UserActivities_ExperiencePointsEarned", "[ExperiencePointsEarned] >= 0");
                tb.HasCheckConstraint("CK_UserActivities_IntensityLevel", "[IntensityLevel] BETWEEN 1 AND 5");
            });

            // Key configuration
            builder.HasKey(ua => ua.Id);
            builder.Property(ua => ua.Id)
                .ValueGeneratedNever()
                .HasColumnType("uniqueidentifier");

            // Audit properties
            builder.Property(ua => ua.CreatedAt)
                .IsRequired();
            
            builder.Property(ua => ua.UpdatedAt)
                .IsRequired(false)
                .IsConcurrencyToken();

            builder.Property(ua => ua.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Core properties
            builder.Property(ua => ua.StartedAt)
                .IsRequired();

            builder.Property(ua => ua.CompletedAt)
                .IsRequired(false);

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
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne(ua => ua.Activity)
                .WithMany(a => a.UserActivities)
                .HasForeignKey(ua => ua.ActivityId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Indexes for performance
            builder.HasIndex(ua => ua.UserId);
            builder.HasIndex(ua => ua.ActivityId);
            builder.HasIndex(ua => ua.CompletedAt);
            builder.HasIndex(ua => ua.StartedAt);
            builder.HasIndex(ua => ua.IntensityLevel);
            builder.HasIndex(ua => ua.CreatedAt);
            builder.HasIndex(ua => ua.IsDeleted);

            // Query filter
            builder.HasQueryFilter(ua => !ua.IsDeleted);
        }
    }
}
