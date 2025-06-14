using MediatR;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Commands.Challenges;
using FitnessVibe.Application.Exceptions;

namespace FitnessVibe.Application.Handlers.Challenges
{
    /// <summary>
    /// Handler for activating challenges
    /// </summary>
    public class ActivateChallengeCommandHandler : IRequestHandler<ActivateChallengeCommand, ActivateChallengeResponse>
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ActivateChallengeCommandHandler(
            IChallengeRepository challengeRepository,
            IUnitOfWork unitOfWork)
        {
            _challengeRepository = challengeRepository ?? throw new ArgumentNullException(nameof(challengeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ActivateChallengeResponse> Handle(ActivateChallengeCommand request, CancellationToken cancellationToken)
        {
            var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId)
                ?? throw new NotFoundException("Challenge", request.ChallengeId);

            if (challenge.CreatedById != request.UserId)
            {
                throw new UnauthorizedOperationException(
                    request.UserId,
                    "Challenge",
                    request.ChallengeId,
                    "activate",
                    "Only the creator can activate a challenge");
            }

            challenge.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ActivateChallengeResponse
            {
                ChallengeId = challenge.Id,
                ActivatedAt = DateTime.UtcNow,
                ParticipantCount = challenge.Participants.Count
            };
        }
    }
}
