using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for the UserGoal entity.
    /// Configures the database mapping, relationships, indexes and constraints.
    /// </summary>
    public class UserGoalConfiguration : IEntityTypeConfiguration<UserGoal>
    {
        /// <summary>
        /// Configures the UserGoal entity
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<UserGoal> builder)
        {
            builder.ToTable("UserGoals", tb =>
            {
                tb.HasCheckConstraint("CK_UserGoals_TargetValue", "[TargetValue] >= 0");
                tb.HasCheckConstraint("CK_UserGoals_CurrentValue", "[CurrentValue] >= 0");
            });

            // Key configuration
            builder.HasKey(ug => ug.Id);
            builder.Property(ug => ug.Id)
                .ValueGeneratedNever()
                .HasColumnType("uniqueidentifier");

            // Audit properties
            builder.Property(ug => ug.CreatedAt)
                .IsRequired();
            
            builder.Property(ug => ug.UpdatedAt)
                .IsRequired(false)
                .IsConcurrencyToken();

            builder.Property(ug => ug.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Core properties
            builder.Property(ug => ug.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ug => ug.Description)
                .HasMaxLength(500);

            builder.Property(ug => ug.TargetValue)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ug => ug.CurrentValue)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ug => ug.Unit)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(ug => ug.StartDate)
                .IsRequired();

            builder.Property(ug => ug.EndDate)
                .IsRequired();

            builder.Property(ug => ug.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);            builder.Property(ug => ug.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(GoalStatus.Active);

            builder.Property(ug => ug.Frequency)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(ug => ug.IsAdaptive)
                .IsRequired()
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(ug => ug.User)
                .WithMany(u => u.Goals)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Indexes
            builder.HasIndex(ug => ug.UserId);
            builder.HasIndex(ug => ug.Type);
            builder.HasIndex(ug => ug.Status);
            builder.HasIndex(ug => ug.StartDate);
            builder.HasIndex(ug => ug.EndDate);
            builder.HasIndex(ug => ug.IsDeleted);

            // Query filter
            builder.HasQueryFilter(ug => !ug.IsDeleted);
        }
    }
}
