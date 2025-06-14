using FitnessVibe.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessVibe.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the ActivityShare entity
/// </summary>
public class ActivityShareConfiguration : IEntityTypeConfiguration<ActivityShare>
{
    public void Configure(EntityTypeBuilder<ActivityShare> builder)
    {
        builder.ToTable("ActivityShares");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Activity)
            .WithMany()
            .HasForeignKey(x => x.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Caption)
            .HasMaxLength(500);

        builder.Property(x => x.Privacy)
            .IsRequired();

        builder.Property(x => x.SharedAt)
            .IsRequired();

        builder.HasMany(x => x.Likes)
            .WithOne(x => x.ActivityShare)
            .HasForeignKey(x => x.ActivityShareId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.ActivityShare)
            .HasForeignKey(x => x.ActivityShareId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
