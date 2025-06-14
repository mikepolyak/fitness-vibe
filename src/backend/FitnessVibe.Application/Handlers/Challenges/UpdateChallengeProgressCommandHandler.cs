using MediatR;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Commands.Challenges;
using FitnessVibe.Application.Exceptions;

namespace FitnessVibe.Application.Handlers.Challenges
{
    /// <summary>
    /// Handler for updating challenge progress
    /// </summary>
    public class UpdateChallengeProgressCommandHandler : IRequestHandler<UpdateChallengeProgressCommand, UpdateChallengeProgressResponse>
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateChallengeProgressCommandHandler(
            IChallengeRepository challengeRepository,
            IUnitOfWork unitOfWork)
        {
            _challengeRepository = challengeRepository ?? throw new ArgumentNullException(nameof(challengeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<UpdateChallengeProgressResponse> Handle(UpdateChallengeProgressCommand request, CancellationToken cancellationToken)
        {
            var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId)
                ?? throw new NotFoundException("Challenge", request.ChallengeId);

            if (!challenge.IsActive)
                throw new InvalidOperationException("Cannot update progress for an inactive challenge");

            challenge.UpdateProgress(request.UserId, request.Progress);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var isCompleted = challenge.IsCompleted(request.UserId);
            var progressPercentage = Math.Min(100m, (request.Progress / challenge.TargetValue) * 100m);

            return new UpdateChallengeProgressResponse
            {
                Progress = request.Progress,
                TargetValue = challenge.TargetValue,
                ProgressPercentage = progressPercentage,
                IsCompleted = isCompleted,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
