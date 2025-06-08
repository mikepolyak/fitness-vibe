using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessVibe.Domain.Entities.Gamification;

namespace FitnessVibe.Infrastructure.Data.Configurations
{
    public class BadgeConfiguration : IEntityTypeConfiguration<Badge>
    {
        public void Configure(EntityTypeBuilder<Badge> builder)
        {
            builder.ToTable("Badges");

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(b => b.Requirements)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(b => b.Category)
                .IsRequired()
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(b => b.Category);
        }
    }
}
