using FitnessVibe.Application.DTOs.Activities;
using MediatR;

namespace FitnessVibe.Application.Queries.Activities;

/// <summary>
/// Query to get the route for an activity
/// </summary>
public class GetActivityRouteQuery : IRequest<ActivityRouteDto>
{
    /// <summary>
    /// Gets or sets the ID of the activity
    /// </summary>
    public Guid ActivityId { get; set; }
}

/// <summary>
/// Query to get route statistics for a specific time range
/// </summary>
public class GetRouteStatisticsQuery : IRequest<ActivityRouteDto>
{
    /// <summary>
    /// Gets or sets the ID of the activity
    /// </summary>
    public Guid ActivityId { get; set; }

    /// <summary>
    /// Gets or sets the start time of the range
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the range
    /// </summary>
    public DateTime EndTime { get; set; }
}
