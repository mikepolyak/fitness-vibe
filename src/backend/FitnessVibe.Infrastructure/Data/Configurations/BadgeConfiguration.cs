using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessVibe.Domain.Entities.Gamification;

namespace FitnessVibe.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for the Badge entity.
    /// Configures the database mapping, relationships, indexes and constraints.
    /// </summary>
    public class BadgeConfiguration : IEntityTypeConfiguration<Badge>
    {
        /// <summary>
        /// Configures the Badge entity
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<Badge> builder)
        {
            builder.ToTable("Badges");

            // Key configuration
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedNever()
                .HasColumnType("uniqueidentifier");

            // Audit properties
            builder.Property(b => b.CreatedAt)
                .IsRequired();
            
            builder.Property(b => b.UpdatedAt)
                .IsRequired(false)
                .IsConcurrencyToken();

            builder.Property(b => b.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Core properties
            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.IconUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(b => b.Category)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(b => b.Rarity)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(b => b.Points)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(b => b.Criteria)
                .IsRequired()
                .HasMaxLength(2000);

            // Indexes
            builder.HasIndex(b => b.Category);
            builder.HasIndex(b => b.Name);
            builder.HasIndex(b => b.Rarity);
            builder.HasIndex(b => b.Points);
            builder.HasIndex(b => b.IsDeleted);

            // Query filter
            builder.HasQueryFilter(b => !b.IsDeleted);

            // Check constraints
            builder.ToTable(tb => tb.HasCheckConstraint("CK_Badges_Points", "[Points] >= 0"));
        }
    }
}
