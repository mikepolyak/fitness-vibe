using FitnessVibe.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessVibe.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the ActivityLike entity
/// </summary>
public class ActivityLikeConfiguration : IEntityTypeConfiguration<ActivityLike>
{
    public void Configure(EntityTypeBuilder<ActivityLike> builder)
    {
        builder.ToTable("ActivityLikes");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ActivityShare)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.ActivityShareId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.ActivityShareId })
            .IsUnique();
    }
}
