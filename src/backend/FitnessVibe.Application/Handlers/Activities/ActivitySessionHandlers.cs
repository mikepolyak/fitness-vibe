using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Domain.Repositories;

namespace FitnessVibe.Application.Handlers.Activities
{
    /// <summary>
    /// Handler for pausing workout sessions - the break time coordinator.
    /// This is like having a personal trainer help you take a proper rest during your workout,
    /// ensuring your session is paused safely and can be resumed when you're ready.
    /// </summary>
    public class PauseActivityCommandHandler : IRequestHandler<PauseActivityCommand>
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PauseActivityCommandHandler> _logger;

        public PauseActivityCommandHandler(
            IActivityRepository activityRepository,
            IUserRepository userRepository,
            ILogger<PauseActivityCommandHandler> logger)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PauseActivityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Pausing activity session {SessionId} for user {UserId}", 
                request.SessionId, request.UserId);

            // Get the activity session
            var activity = await _activityRepository.GetByIdAsync(request.SessionId);
            if (activity == null)
            {
                throw new ArgumentException($"Activity session {request.SessionId} not found");
            }

            // Verify ownership
            if (activity.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You can only pause your own activities");
            }

            // Verify session can be paused
            if (activity.Status != Domain.Enums.ActivityStatus.Active)
            {
                throw new InvalidOperationException($"Cannot pause activity - current status is {activity.Status}");
            }

            // Pause the activity
            activity.Pause();

            // Update in database
            await _activityRepository.UpdateAsync(activity);
            await _activityRepository.SaveChangesAsync();

            _logger.LogInformation("Activity session {SessionId} paused successfully", request.SessionId);
        }
    }

    /// <summary>
    /// Handler for resuming paused workout sessions - the back-to-action coordinator.
    /// This is like having a personal trainer help you get back into your workout groove
    /// after taking a well-deserved break.
    /// </summary>
    public class ResumeActivityCommandHandler : IRequestHandler<ResumeActivityCommand>
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ResumeActivityCommandHandler> _logger;

        public ResumeActivityCommandHandler(
            IActivityRepository activityRepository,
            IUserRepository userRepository,
            ILogger<ResumeActivityCommandHandler> logger)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ResumeActivityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Resuming activity session {SessionId} for user {UserId}", 
                request.SessionId, request.UserId);

            // Get the activity session
            var activity = await _activityRepository.GetByIdAsync(request.SessionId);
            if (activity == null)
            {
                throw new ArgumentException($"Activity session {request.SessionId} not found");
            }

            // Verify ownership
            if (activity.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You can only resume your own activities");
            }

            // Verify session can be resumed
            if (activity.Status != Domain.Enums.ActivityStatus.Paused)
            {
                throw new InvalidOperationException($"Cannot resume activity - current status is {activity.Status}");
            }

            // Resume the activity
            activity.Resume();

            // Update in database
            await _activityRepository.UpdateAsync(activity);
            await _activityRepository.SaveChangesAsync();

            _logger.LogInformation("Activity session {SessionId} resumed successfully", request.SessionId);
        }
    }

    /// <summary>
    /// Handler for cancelling workout sessions - the graceful exit coordinator.
    /// This is like having a personal trainer help you end your workout early if needed,
    /// ensuring everything is properly saved and cleaned up.
    /// </summary>
    public class CancelActivityCommandHandler : IRequestHandler<CancelActivityCommand>
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CancelActivityCommandHandler> _logger;

        public CancelActivityCommandHandler(
            IActivityRepository activityRepository,
            IUserRepository userRepository,
            ILogger<CancelActivityCommandHandler> logger)
        {
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CancelActivityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cancelling activity session {SessionId} for user {UserId}", 
                request.SessionId, request.UserId);

            // Get the activity session
            var activity = await _activityRepository.GetByIdAsync(request.SessionId);
            if (activity == null)
            {
                throw new ArgumentException($"Activity session {request.SessionId} not found");
            }

            // Verify ownership
            if (activity.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You can only cancel your own activities");
            }

            // Verify session can be cancelled
            if (activity.Status == Domain.Enums.ActivityStatus.Completed || 
                activity.Status == Domain.Enums.ActivityStatus.Cancelled)
            {
                throw new InvalidOperationException($"Cannot cancel activity - current status is {activity.Status}");
            }

            // Cancel the activity
            activity.Cancel(request.CancellationReason ?? "User cancelled");

            // Update in database
            await _activityRepository.UpdateAsync(activity);
            await _activityRepository.SaveChangesAsync();

            _logger.LogInformation("Activity session {SessionId} cancelled successfully", request.SessionId);
        }
    }
}

// Command definitions for the handlers above
namespace FitnessVibe.Application.Commands.Activities
{
    public class PauseActivityCommand : IRequest
    {
        public int UserId { get; set; }
        public int SessionId { get; set; }
    }

    public class ResumeActivityCommand : IRequest
    {
        public int UserId { get; set; }
        public int SessionId { get; set; }
    }

    public class CancelActivityCommand : IRequest
    {
        public int UserId { get; set; }
        public int SessionId { get; set; }
        public string? CancellationReason { get; set; }
    }
}
