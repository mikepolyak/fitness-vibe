using FitnessVibe.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessVibe.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the UserConnection entity
/// </summary>
public class UserConnectionConfiguration : IEntityTypeConfiguration<UserConnection>
{
    public void Configure(EntityTypeBuilder<UserConnection> builder)
    {
        builder.ToTable("UserConnections");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Follower)
            .WithMany()
            .HasForeignKey(x => x.FollowerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Followed)
            .WithMany()
            .HasForeignKey(x => x.FollowedId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.FollowerId, x.FollowedId })
            .IsUnique();

        builder.Property(x => x.ConnectedAt)
            .IsRequired();
    }
}
