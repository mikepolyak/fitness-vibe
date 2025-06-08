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
        /// <summary>
        /// Configures the User entity mapping
        /// </summary>
        /// <param name="builder">The entity type builder</param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table configuration
            builder.ToTable("Users", tb => 
            {
                tb.HasCheckConstraint("CK_Users_ExperiencePoints", "[ExperiencePoints] >= 0");
                tb.HasCheckConstraint("CK_Users_Level", "[Level] >= 1");
            });
            
            // Key configuration
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .ValueGeneratedNever()
                .HasColumnType("uniqueidentifier");

            // Concurrency token for optimistic concurrency
            builder.Property(u => u.UpdatedAt)
                .IsConcurrencyToken();

            // Audit properties
            builder.Property(u => u.CreatedAt)
                .IsRequired();
            
            builder.Property(u => u.UpdatedAt)
                .IsRequired(false);

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
            
            // Core properties
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
                
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            // Computed column for full name
            builder.Property<string>("FullName")
                .HasComputedColumnSql("[FirstName] + ' ' + [LastName]", stored: true);

            builder.Property(u => u.DateOfBirth)
                .IsRequired(false);
                
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
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.PrimaryGoal)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            // Game mechanics properties
            builder.Property(u => u.ExperiencePoints)
                .IsRequired()
                .HasDefaultValue(0);
                
            builder.Property(u => u.Level)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(u => u.LastActiveDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.IsEmailVerified)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Value object configuration
            builder.OwnsOne(u => u.Preferences, prefs =>
            {
                prefs.Property(p => p.TimeZone)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("UTC");

                prefs.Property(p => p.PreferredUnits)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("metric");

                prefs.Property(p => p.AllowNotifications)
                    .IsRequired()
                    .HasDefaultValue(true);

                prefs.Property(p => p.ShareActivitiesPublicly)
                    .IsRequired()
                    .HasDefaultValue(false);

                prefs.Property(p => p.ReceiveMotivationalMessages)
                    .IsRequired()
                    .HasDefaultValue(true);

                prefs.Property(p => p.AllowFriendRequests)
                    .IsRequired()
                    .HasDefaultValue(true);

                prefs.Property(p => p.QuietHourStart)
                    .IsRequired()
                    .HasDefaultValue(22);

                prefs.Property(p => p.QuietHourEnd)
                    .IsRequired()
                    .HasDefaultValue(7);
            });

            // Navigation properties with explicit delete behavior
            builder.HasMany(u => u.Goals)
                .WithOne(g => g.User)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasMany(u => u.Activities)
                .WithOne(ua => ua.User)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasMany(u => u.Badges)
                .WithOne(ub => ub.User)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            // Indexes for performance
            builder.HasIndex(u => u.Email)
                .IsUnique();
            
            builder.HasIndex(u => u.Level);
            builder.HasIndex(u => u.ExperiencePoints);
            builder.HasIndex(u => u.LastActiveDate);
            builder.HasIndex(u => u.IsDeleted);
            builder.HasIndex(u => new { u.FirstName, u.LastName });
            builder.HasIndex(u => u.PrimaryGoal);  // New index for filtering by primary goal
            builder.HasIndex(u => u.FitnessLevel); // New index for filtering by fitness level
            builder.HasIndex("FullName");          // Index on computed full name column
        }
    }
}
