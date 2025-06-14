using AutoMapper;
using FitnessVibe.Application.Commands.Activities;
using FitnessVibe.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FitnessVibe.Application.Handlers.Activities;

/// <summary>
/// Handler for adding GPS points to an activity's route
/// </summary>
public class AddRoutePointCommandHandler : IRequestHandler<AddRoutePointCommand, bool>
{
    private readonly IActivityRepository _activityRepository;
    private readonly ILogger<AddRoutePointCommandHandler> _logger;

    public AddRoutePointCommandHandler(
        IActivityRepository activityRepository,
        ILogger<AddRoutePointCommandHandler> logger)
    {
        _activityRepository = activityRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(AddRoutePointCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await _activityRepository.GetByIdAsync(request.ActivityId);
            if (activity == null)
            {
                _logger.LogError("Activity {ActivityId} not found", request.ActivityId);
                throw new NotFoundException(nameof(Activity), request.ActivityId);
            }

            activity.AddRoutePoint(
                request.Latitude,
                request.Longitude,
                request.Elevation,
                request.Speed,
                request.Accuracy);

            await _activityRepository.UpdateAsync(activity);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding route point to activity {ActivityId}", request.ActivityId);
            throw;
        }
    }
}
