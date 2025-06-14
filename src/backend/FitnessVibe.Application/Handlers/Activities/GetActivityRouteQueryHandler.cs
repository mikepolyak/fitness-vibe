using AutoMapper;
using FitnessVibe.Application.DTOs.Activities;
using FitnessVibe.Application.Queries.Activities;
using FitnessVibe.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FitnessVibe.Application.Handlers.Activities;

/// <summary>
/// Handler for retrieving activity route data
/// </summary>
public class GetActivityRouteQueryHandler : IRequestHandler<GetActivityRouteQuery, ActivityRouteDto>
{
    private readonly IActivityRepository _activityRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetActivityRouteQueryHandler> _logger;

    public GetActivityRouteQueryHandler(
        IActivityRepository activityRepository,
        IMapper mapper,
        ILogger<GetActivityRouteQueryHandler> logger)
    {
        _activityRepository = activityRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ActivityRouteDto> Handle(GetActivityRouteQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(request.ActivityId);
            if (activity == null)
            {
                _logger.LogError("Activity {ActivityId} not found", request.ActivityId);
                throw new NotFoundException(nameof(Activity), request.ActivityId);
            }

            if (activity.Route == null)
            {
                _logger.LogInformation("No route data found for activity {ActivityId}", request.ActivityId);
                return new ActivityRouteDto
                {
                    ActivityId = request.ActivityId,
                    Points = new List<GpsPointDto>()
                };
            }

            return _mapper.Map<ActivityRouteDto>(activity.Route);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving route for activity {ActivityId}", request.ActivityId);
            throw;
        }
    }
}

/// <summary>
/// Handler for retrieving route statistics for a specific time range
/// </summary>
public class GetRouteStatisticsQueryHandler : IRequestHandler<GetRouteStatisticsQuery, ActivityRouteDto>
{
    private readonly IActivityRepository _activityRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetRouteStatisticsQueryHandler> _logger;

    public GetRouteStatisticsQueryHandler(
        IActivityRepository activityRepository,
        IMapper mapper,
        ILogger<GetRouteStatisticsQueryHandler> logger)
    {
        _activityRepository = activityRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ActivityRouteDto> Handle(GetRouteStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(request.ActivityId);
            if (activity == null)
            {
                _logger.LogError("Activity {ActivityId} not found", request.ActivityId);
                throw new NotFoundException(nameof(Activity), request.ActivityId);
            }

            if (activity.Route == null)
            {
                _logger.LogInformation("No route data found for activity {ActivityId}", request.ActivityId);
                return new ActivityRouteDto
                {
                    ActivityId = request.ActivityId,
                    Points = new List<GpsPointDto>()
                };
            }

            var (distance, avgSpeed, elevationGain) = activity.Route.GetStatisticsForTimeRange(request.StartTime, request.EndTime);

            return new ActivityRouteDto
            {
                ActivityId = request.ActivityId,
                TotalDistance = distance,
                AverageSpeed = avgSpeed,
                ElevationGain = elevationGain,
                Points = _mapper.Map<List<GpsPointDto>>(
                    activity.Route.Points.Where(p => p.Timestamp >= request.StartTime && p.Timestamp <= request.EndTime))
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving route statistics for activity {ActivityId}", request.ActivityId);
            throw;
        }
    }
}
