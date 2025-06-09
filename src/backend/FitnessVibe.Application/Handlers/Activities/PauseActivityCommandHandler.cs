using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Commands.Activities;
using FitnessVibe.Application.Exceptions;
using FitnessVibe.Domain.Events;

namespace FitnessVibe.Application.Handlers.Activities
{
    /// <summary>
    /// Handler for pausing workout sessions - the break time coordinator.
    /// This is like having a personal trainer help you take a proper rest during your workout,
    /// ensuring your session is paused safely and can be resumed when you're ready.
    /// </summary>
    public class PauseActivityCommandHandler : IRequestHandler<PauseActivityCommand, PauseActivityResponse>
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PauseActivityCommandHandler> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the handler
        /// </summary>
        public PauseActivityCommandHandler(
            IActivityRepository activityRepository,
            IUserRepository userRepository,
            IMediator mediator,
            ILogger<PauseActivityCommandHandler> logger)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the pause activity command
        /// </summary>
        public async Task<PauseActivityResponse> Handle(PauseActivityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Attempting to pause activity session {SessionId} for user {UserId}", 
                    request.SessionId, request.UserId);

                // Get the activity session with tracking
                var activity = await _activityRepository.GetByIdWithTrackingAsync(request.SessionId)
                    ?? throw new NotFoundException($"Activity session {request.SessionId} not found");

                // Verify ownership
                if (activity.UserId != request.UserId)
                {
                    _logger.LogWarning("User {UserId} attempted to pause activity {SessionId} owned by {OwnerId}",
                        request.UserId, request.SessionId, activity.UserId);
                    throw new UnauthorizedAccessException("You can only pause your own activities");
                }

                // Verify session can be paused
                if (activity.Status != Domain.Enums.ActivityStatus.Active)
                {
                    throw new InvalidOperationException($"Cannot pause activity - current status is {activity.Status}");
                }

                // Pause the activity - this will raise domain events
                var pausedAt = activity.Pause();

                // Save changes which will dispatch domain events through the DbContext
                await _activityRepository.SaveChangesAsync(cancellationToken);

                // Create the response
                var response = new PauseActivityResponse
                {
                    PausedAt = pausedAt,
                    CurrentDurationMinutes = activity.GetCurrentDuration(),
                    Achievements = activity.GetAchievementsForSession()
                };

                _logger.LogInformation("Activity session {SessionId} paused successfully at {PausedAt}", 
                    request.SessionId, pausedAt);

                return response;
            }
            catch (Exception ex) when (ex is not UnauthorizedAccessException && ex is not NotFoundException)
            {
                _logger.LogError(ex, "Error pausing activity session {SessionId} for user {UserId}", 
                    request.SessionId, request.UserId);
                throw;
            }
        }
    }
}
