using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessVibe.Domain.Entities.Users;

namespace FitnessVibe.Infrastructure.Data.Configurations
{
    public class UserGoalConfiguration : IEntityTypeConfiguration<UserGoal>
    {
        public void Configure(EntityTypeBuilder<UserGoal> builder)
        {
            builder.ToTable("UserGoals");

            builder.Property(ug => ug.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ug => ug.Description)
                .HasMaxLength(500);

            builder.Property(ug => ug.TargetValue)
                .IsRequired();

            builder.Property(ug => ug.CurrentValue)
                .IsRequired();

            builder.Property(ug => ug.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ug => ug.Status)
                .IsRequired()
                .HasMaxLength(20);

            // Relationships
            builder.HasOne(ug => ug.User)
                .WithMany(u => u.Goals)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(ug => ug.UserId);
            builder.HasIndex(ug => ug.Type);
            builder.HasIndex(ug => ug.Status);
            builder.HasIndex(ug => ug.DueDate);
        }
    }
}
