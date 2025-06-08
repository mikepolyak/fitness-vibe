using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessVibe.Domain.Entities.Activities;

namespace FitnessVibe.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuration for the Activity entity
    /// </summary>
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        /// <summary>
        /// Configures the Activity entity
        /// </summary>
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.ToTable("Activities");

            builder.Property(a => a.Id)
                .ValueGeneratedNever();

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .HasMaxLength(1000);

            builder.Property(a => a.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.Category)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(a => a.IconUrl)
                .HasMaxLength(2048);

            builder.Property(a => a.IsFeatured)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(a => a.DifficultyLevel)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(a => a.EstimatedCaloriesPerHour)
                .IsRequired()
                .HasDefaultValue(0);

            // Relationships are handled by UserActivityConfiguration

            // Indexes for filtering and sorting
            builder.HasIndex(a => a.Type);
            builder.HasIndex(a => a.Category);
            builder.HasIndex(a => a.IsFeatured);
            builder.HasIndex(a => a.DifficultyLevel);
            builder.HasIndex(a => a.Name);
            builder.HasIndex(a => a.CreatedAt);

            // Filter out soft-deleted activities
            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
}
