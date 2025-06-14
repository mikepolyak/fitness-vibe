using FitnessVibe.Domain.Common;
using FitnessVibe.Domain.ValueObjects;

namespace FitnessVibe.Domain.Entities.Activities;

/// <summary>
/// Represents a GPS route for an activity
/// </summary>
public class ActivityRoute : BaseEntity
{
    private readonly List<GpsPoint> _points = new();
    
    public Guid ActivityId { get; private set; }
    public Activity Activity { get; private set; }
    public IReadOnlyCollection<GpsPoint> Points => _points.AsReadOnly();
    public double? TotalDistance { get; private set; }
    public double? AverageSpeed { get; private set; }
    public double? MaxSpeed { get; private set; }
    public double? MinElevation { get; private set; }
    public double? MaxElevation { get; private set; }
    public double? ElevationGain { get; private set; }

    private ActivityRoute() { } // For EF Core

    public ActivityRoute(Activity activity)
    {
        Activity = activity ?? throw new ArgumentNullException(nameof(activity));
        ActivityId = activity.Id;
    }

    /// <summary>
    /// Adds a new GPS point to the route and updates route statistics
    /// </summary>
    public void AddPoint(GpsPoint point)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point));

        _points.Add(point);
        UpdateStatistics();
    }

    /// <summary>
    /// Updates route statistics based on current points
    /// </summary>
    private void UpdateStatistics()
    {
        if (!_points.Any())
            return;

        // Update speed statistics
        var speeds = _points.Where(p => p.Speed.HasValue).Select(p => p.Speed.Value).ToList();
        if (speeds.Any())
        {
            MaxSpeed = speeds.Max();
            AverageSpeed = speeds.Average();
        }

        // Update elevation statistics
        var elevations = _points.Where(p => p.Elevation.HasValue).Select(p => p.Elevation.Value).ToList();
        if (elevations.Any())
        {
            MinElevation = elevations.Min();
            MaxElevation = elevations.Max();
            
            // Calculate elevation gain (sum of positive elevation changes)
            ElevationGain = 0;
            for (int i = 1; i < _points.Count; i++)
            {
                if (_points[i].Elevation.HasValue && _points[i - 1].Elevation.HasValue)
                {
                    var elevationDiff = _points[i].Elevation.Value - _points[i - 1].Elevation.Value;
                    if (elevationDiff > 0)
                        ElevationGain += elevationDiff;
                }
            }
        }

        // Calculate total distance
        TotalDistance = 0;
        for (int i = 1; i < _points.Count; i++)
        {
            TotalDistance += _points[i-1].CalculateDistanceTo(_points[i]);
        }
    }

    /// <summary>
    /// Gets route statistics for a specific time range
    /// </summary>
    public (double? distance, double? avgSpeed, double? elevationGain) GetStatisticsForTimeRange(DateTime start, DateTime end)
    {
        var rangePoints = _points.Where(p => p.Timestamp >= start && p.Timestamp <= end).ToList();
        
        if (!rangePoints.Any())
            return (null, null, null);

        double distance = 0;
        for (int i = 1; i < rangePoints.Count; i++)
        {
            distance += rangePoints[i-1].CalculateDistanceTo(rangePoints[i]);
        }

        var avgSpeed = rangePoints.Where(p => p.Speed.HasValue).Select(p => p.Speed.Value).DefaultIfEmpty().Average();
        
        double elevationGain = 0;
        for (int i = 1; i < rangePoints.Count; i++)
        {
            if (rangePoints[i].Elevation.HasValue && rangePoints[i - 1].Elevation.HasValue)
            {
                var elevationDiff = rangePoints[i].Elevation.Value - rangePoints[i - 1].Elevation.Value;
                if (elevationDiff > 0)
                    elevationGain += elevationDiff;
            }
        }

        return (distance, avgSpeed, elevationGain);
    }
}
