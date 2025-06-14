using FitnessVibe.Domain.Entities.Activities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessVibe.Infrastructure.Data.Configurations;

public class ActivityRouteConfiguration : IEntityTypeConfiguration<ActivityRoute>
{
    public void Configure(EntityTypeBuilder<ActivityRoute> builder)
    {
        builder.ToTable("ActivityRoutes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ActivityId)
            .IsRequired();

        builder.Property(x => x.TotalDistance)
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2);

        builder.Property(x => x.AverageSpeed)
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2);

        builder.Property(x => x.MaxSpeed)
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2);

        builder.Property(x => x.MinElevation)
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2);

        builder.Property(x => x.MaxElevation)
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2);

        builder.Property(x => x.ElevationGain)
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2);

        // Configure the Points collection as a JSON column
        builder.Property(x => x.Points)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<GpsPoint>>(v, (JsonSerializerOptions)null))
            .HasColumnName("RoutePoints");

        // Configure relationship with Activity
        builder.HasOne(x => x.Activity)
            .WithOne(x => x.Route)
            .HasForeignKey<ActivityRoute>(x => x.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure soft delete
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
