namespace FitnessVibe.Application.DTOs.Activities;

/// <summary>
/// Represents a point in a GPS route with latitude, longitude, and optional elevation and speed
/// </summary>
public class GpsPointDto
{
    /// <summary>
    /// Gets or sets the latitude of the point
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude of the point
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Gets or sets the elevation in meters, if available
    /// </summary>
    public double? Elevation { get; set; }

    /// <summary>
    /// Gets or sets the speed in meters per second, if available
    /// </summary>
    public double? Speed { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of when this point was recorded
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the accuracy in meters, if available
    /// </summary>
    public double? Accuracy { get; set; }
}

/// <summary>
/// Represents a route for an activity including GPS points and statistics
/// </summary>
public class ActivityRouteDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the route
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the activity this route belongs to
    /// </summary>
    public Guid ActivityId { get; set; }

    /// <summary>
    /// Gets or sets the collection of GPS points in this route
    /// </summary>
    public ICollection<GpsPointDto> Points { get; set; } = new List<GpsPointDto>();

    /// <summary>
    /// Gets or sets the total distance covered in meters
    /// </summary>
    public double? TotalDistance { get; set; }

    /// <summary>
    /// Gets or sets the average speed in meters per second
    /// </summary>
    public double? AverageSpeed { get; set; }

    /// <summary>
    /// Gets or sets the maximum speed reached in meters per second
    /// </summary>
    public double? MaxSpeed { get; set; }

    /// <summary>
    /// Gets or sets the minimum elevation in meters
    /// </summary>
    public double? MinElevation { get; set; }

    /// <summary>
    /// Gets or sets the maximum elevation in meters
    /// </summary>
    public double? MaxElevation { get; set; }

    /// <summary>
    /// Gets or sets the total elevation gain in meters
    /// </summary>
    public double? ElevationGain { get; set; }
}
