using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.ValueObjects;

namespace FitnessVibe.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for User entity.
    /// Think of this as the blueprint for how user membership records
    /// are stored and organized in our database.
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table configuration
            builder.ToTable("Users");
            
            // Primary key
            builder.HasKey(u => u.Id);
            
            // Properties
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
                
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);
                
            builder.Property(u => u.AvatarUrl)
                .HasMaxLength(500);

            // Enum conversions
            builder.Property(u => u.Gender)
                .HasConversion<string>()
                .HasMaxLength(50);
                
            builder.Property(u => u.FitnessLevel)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(u => u.PrimaryGoal)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            // Value object - UserPreferences
            builder.OwnsOne(u => u.Preferences, prefs =>
            {
                prefs.Property(p => p.TimeZone)
                    .HasMaxLength(100)
                    .HasDefaultValue("UTC");
                    
                prefs.Property(p => p.PreferredUnits)
                    .HasMaxLength(20)
                    .HasDefaultValue("metric");
            });

            // Indexes
            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");
                
            builder.HasIndex(u => u.Level)
                .HasDatabaseName("IX_Users_Level");
                
            builder.HasIndex(u => u.LastActiveDate)
                .HasDatabaseName("IX_Users_LastActiveDate");

            // Relationships
            builder.HasMany(u => u.Goals)
                .WithOne(g => g.User)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasMany(u => u.Badges)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasMany(u => u.Activities)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
