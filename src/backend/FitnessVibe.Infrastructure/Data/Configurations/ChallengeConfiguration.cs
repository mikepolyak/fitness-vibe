using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessVibe.Domain.Entities.Challenges;

namespace FitnessVibe.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for the Challenge entity
    /// </summary>
    public class ChallengeConfiguration : IEntityTypeConfiguration<Challenge>
    {
        public void Configure(EntityTypeBuilder<Challenge> builder)
        {
            builder.ToTable("Challenges");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired(false);

            builder.Property(x => x.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(x => x.TargetValue)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.Unit)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ActivityType)
                .IsRequired(false)
                .HasConversion<string>();

            builder.Property(x => x.IsPrivate)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedById)
                .IsRequired();

            builder.HasMany(x => x.Participants)
                .WithOne(x => x.Challenge)
                .HasForeignKey(x => x.ChallengeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
