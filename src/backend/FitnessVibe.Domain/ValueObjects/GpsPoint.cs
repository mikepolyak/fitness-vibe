namespace FitnessVibe.Domain.ValueObjects;

/// <summary>
/// Represents a GPS coordinate point with latitude, longitude, elevation, and timestamp
/// </summary>
public class GpsPoint
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public double? Elevation { get; private set; }
    public DateTime Timestamp { get; private set; }
    public double? Speed { get; private set; }
    public double? Accuracy { get; private set; }

    private GpsPoint() { } // For EF Core

    public GpsPoint(double latitude, double longitude, double? elevation, DateTime timestamp, double? speed = null, double? accuracy = null)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees");
        
        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees");

        Latitude = latitude;
        Longitude = longitude;
        Elevation = elevation;
        Timestamp = timestamp.ToUniversalTime();
        Speed = speed;
        Accuracy = accuracy;
    }

    /// <summary>
    /// Calculates the distance in meters between this point and another point using the Haversine formula
    /// </summary>
    public double CalculateDistanceTo(GpsPoint other)
    {
        const double earthRadius = 6371000; // Earth's radius in meters
        var lat1 = Latitude * Math.PI / 180;
        var lat2 = other.Latitude * Math.PI / 180;
        var deltaLat = (other.Latitude - Latitude) * Math.PI / 180;
        var deltaLon = (other.Longitude - Longitude) * Math.PI / 180;

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadius * c;
    }
}
