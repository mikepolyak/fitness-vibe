using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Commands.Activities;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.Enums;

namespace FitnessVibe.Application.Handlers.Activities
{
    /// <summary>
    /// Handler for resuming paused workout sessions - the back-to-action coordinator.
    /// This is like having a personal trainer help you get back into your workout groove
    /// after taking a well-deserved break.
    /// </summary>
    public class ResumeActivityCommandHandler : IRequestHandler<ResumeActivityCommand, ResumeActivityResponse>
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

        public async Task<ResumeActivityResponse> Handle(ResumeActivityCommand request, CancellationToken cancellationToken)
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
            if (activity.Status != ActivityStatus.Paused)
            {
                throw new InvalidOperationException($"Cannot resume activity - current status is {activity.Status}");
            }

            var pauseDurationMinutes = activity.GetPauseDurationInMinutes();

            // Resume the activity
            activity.Resume();

            // Update in database
            await _activityRepository.UpdateAsync(activity);
            await _activityRepository.SaveChangesAsync();

            _logger.LogInformation("Activity session {SessionId} resumed successfully", request.SessionId);

            return new ResumeActivityResponse
            {
                ResumedAt = DateTime.UtcNow,
                PauseDurationMinutes = pauseDurationMinutes,
                MotivationalTips = GetMotivationalTips(activity)
            };
        }

        private IEnumerable<string> GetMotivationalTips(Activity activity)
        {
            return new[]
            {
                "Welcome back! You've got this!",
                $"You're doing great - {activity.GetProgressPercentage():P0} complete!",
                "Remember to maintain proper form as you get back into it."
            };
        }
    }
}
