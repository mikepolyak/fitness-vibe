using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessVibe.Domain.Entities.Activities;

namespace FitnessVibe.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for activity templates
    /// </summary>
    public class ActivityTemplateConfiguration : IEntityTypeConfiguration<ActivityTemplate>
    {
        public void Configure(EntityTypeBuilder<ActivityTemplate> builder)
        {
            builder.ToTable("ActivityTemplates");

            // Base entity properties
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt).IsRequired();
            builder.Property(e => e.IsDeleted).IsRequired();

            // Activity template properties
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(e => e.Type)
                .IsRequired();

            builder.Property(e => e.Category)
                .IsRequired();

            builder.Property(e => e.IconUrl)
                .HasMaxLength(500);

            builder.Property(e => e.EstimatedDurationMinutes)
                .IsRequired();

            builder.Property(e => e.EstimatedCaloriesBurned)
                .IsRequired();

            builder.Property(e => e.DifficultyLevel)
                .IsRequired();

            // Store equipment and tags as JSON arrays
            builder.Property("_requiredEquipment")
                .HasColumnName("RequiredEquipment")
                .HasColumnType("nvarchar(max)");

            builder.Property("_tags")
                .HasColumnName("Tags")
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.IsFeatured)
                .IsRequired();

            builder.Property(e => e.UsageCount)
                .IsRequired();

            builder.Property(e => e.AverageRating)
                .IsRequired()
                .HasPrecision(3, 2);

            builder.Property(e => e.RatingCount)
                .IsRequired();

            // Global query filters
            builder.HasQueryFilter(e => !e.IsDeleted);

            // Indexes
            builder.HasIndex(e => e.Name);
            builder.HasIndex(e => e.Category);
            builder.HasIndex(e => e.DifficultyLevel);
            builder.HasIndex(e => e.IsFeatured);
            builder.HasIndex(e => e.UsageCount);
            builder.HasIndex(e => e.AverageRating);
        }
    }
}
