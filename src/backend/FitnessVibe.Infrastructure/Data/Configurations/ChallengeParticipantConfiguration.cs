using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessVibe.Domain.Entities.Challenges;

namespace FitnessVibe.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for the ChallengeParticipant entity
    /// </summary>
    public class ChallengeParticipantConfiguration : IEntityTypeConfiguration<ChallengeParticipant>
    {
        public void Configure(EntityTypeBuilder<ChallengeParticipant> builder)
        {
            builder.ToTable("ChallengeParticipants");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ChallengeId)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.JoinedAt)
                .IsRequired();

            builder.Property(x => x.Progress)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.IsCompleted)
                .IsRequired();

            builder.Property(x => x.CompletedAt)
                .IsRequired(false);

            builder.HasOne(x => x.Challenge)
                .WithMany(x => x.Participants)
                .HasForeignKey(x => x.ChallengeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ChallengeId, x.UserId })
                .IsUnique();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
