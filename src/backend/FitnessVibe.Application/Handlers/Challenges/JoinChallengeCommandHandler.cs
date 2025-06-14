using MediatR;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Commands.Challenges;
using FitnessVibe.Application.Exceptions;

namespace FitnessVibe.Application.Handlers.Challenges
{
    /// <summary>
    /// Handler for joining challenges
    /// </summary>
    public class JoinChallengeCommandHandler : IRequestHandler<JoinChallengeCommand, JoinChallengeResponse>
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public JoinChallengeCommandHandler(
            IChallengeRepository challengeRepository,
            IUnitOfWork unitOfWork)
        {
            _challengeRepository = challengeRepository ?? throw new ArgumentNullException(nameof(challengeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<JoinChallengeResponse> Handle(JoinChallengeCommand request, CancellationToken cancellationToken)
        {
            var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId)
                ?? throw new NotFoundException("Challenge", request.ChallengeId);

            if (!challenge.IsActive)
                throw new InvalidOperationException("Cannot join an inactive challenge");

            if (challenge.IsPrivate)
            {
                // TODO: Add logic for private challenge invitations
                throw new UnauthorizedOperationException(
                    request.UserId,
                    "Challenge",
                    request.ChallengeId,
                    "join",
                    "Private challenges require an invitation");
            }

            var participant = challenge.AddParticipant(request.UserId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new JoinChallengeResponse
            {
                ChallengeId = challenge.Id,
                JoinedAt = participant.JoinedAt,
                TargetValue = challenge.TargetValue,
                Unit = challenge.Unit,
                EndDate = challenge.EndDate
            };
        }
    }
}
