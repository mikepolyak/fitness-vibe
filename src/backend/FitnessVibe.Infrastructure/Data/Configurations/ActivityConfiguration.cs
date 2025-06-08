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

            // Key configuration
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .ValueGeneratedNever()
                .HasColumnType("uniqueidentifier");

            // Audit properties
            builder.Property(a => a.CreatedAt)
                .IsRequired();
            
            builder.Property(a => a.UpdatedAt)
                .IsRequired(false);

            builder.Property(a => a.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Core properties
            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .HasMaxLength(1000);

            builder.Property(a => a.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.Category)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.IconUrl)
                .HasMaxLength(2048);

            builder.Property(a => a.IsFeatured)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(a => a.DifficultyLevel)
                .IsRequired()
                .HasDefaultValue(1);

            // Navigation properties
            builder.HasMany(a => a.UserActivities)
                .WithOne(ua => ua.Activity)
                .HasForeignKey(ua => ua.ActivityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(a => a.Type);
            builder.HasIndex(a => a.Category);
            builder.HasIndex(a => a.IsFeatured);
            builder.HasIndex(a => a.DifficultyLevel);
            builder.HasIndex(a => a.IsDeleted);
        }
    }
}
