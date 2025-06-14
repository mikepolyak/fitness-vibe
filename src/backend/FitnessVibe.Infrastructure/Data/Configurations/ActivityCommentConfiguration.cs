using FitnessVibe.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessVibe.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the ActivityComment entity
/// </summary>
public class ActivityCommentConfiguration : IEntityTypeConfiguration<ActivityComment>
{
    public void Configure(EntityTypeBuilder<ActivityComment> builder)
    {
        builder.ToTable("ActivityComments");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ActivityShare)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.ActivityShareId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(1000);
    }
}
