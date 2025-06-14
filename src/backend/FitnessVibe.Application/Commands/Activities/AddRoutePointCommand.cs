using MediatR;

namespace FitnessVibe.Application.Commands.Activities;

/// <summary>
/// Command to add a new GPS point to an activity's route
/// </summary>
public class AddRoutePointCommand : IRequest<bool>
{
    /// <summary>
    /// Gets or sets the ID of the activity
    /// </summary>
    public Guid ActivityId { get; set; }

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
    /// Gets or sets the accuracy in meters, if available
    /// </summary>
    public double? Accuracy { get; set; }
}
