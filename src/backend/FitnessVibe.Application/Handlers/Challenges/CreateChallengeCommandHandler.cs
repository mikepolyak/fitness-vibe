using MediatR;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Domain.Entities.Challenges;
using FitnessVibe.Application.Commands.Challenges;
using FitnessVibe.Application.Exceptions;

namespace FitnessVibe.Application.Handlers.Challenges
{
    /// <summary>
    /// Handler for creating new challenges
    /// </summary>
    public class CreateChallengeCommandHandler : IRequestHandler<CreateChallengeCommand, CreateChallengeResponse>
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateChallengeCommandHandler(
            IChallengeRepository challengeRepository,
            IUnitOfWork unitOfWork)
        {
            _challengeRepository = challengeRepository ?? throw new ArgumentNullException(nameof(challengeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<CreateChallengeResponse> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
        {
            var challenge = new Challenge(
                request.Title,
                request.Description,
                request.StartDate,
                request.EndDate,
                request.Type,
                request.TargetValue,
                request.Unit,
                request.ActivityType,
                request.IsPrivate,
                request.CreatedById
            );

            await _challengeRepository.AddAsync(challenge);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateChallengeResponse
            {
                Id = challenge.Id,
                Title = challenge.Title,
                StartDate = challenge.StartDate,
                CreatedAt = challenge.CreatedAt
            };
        }
    }
}
