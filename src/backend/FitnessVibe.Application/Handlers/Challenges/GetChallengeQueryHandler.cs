using MediatR;
using AutoMapper;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Queries.Challenges;
using FitnessVibe.Application.DTOs.Challenges;
using FitnessVibe.Application.Exceptions;

namespace FitnessVibe.Application.Handlers.Challenges
{
    /// <summary>
    /// Handler for retrieving a single challenge
    /// </summary>
    public class GetChallengeQueryHandler : IRequestHandler<GetChallengeQuery, ChallengeResponse>
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IMapper _mapper;

        public GetChallengeQueryHandler(IChallengeRepository challengeRepository, IMapper mapper)
        {
            _challengeRepository = challengeRepository ?? throw new ArgumentNullException(nameof(challengeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ChallengeResponse> Handle(GetChallengeQuery request, CancellationToken cancellationToken)
        {
            var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId)
                ?? throw new NotFoundException("Challenge", request.ChallengeId);

            var response = _mapper.Map<ChallengeResponse>(challenge);

            if (request.UserId.HasValue)
            {
                var participant = challenge.Participants.FirstOrDefault(p => p.UserId == request.UserId.Value);
                if (participant != null)
                {
                    response.UserParticipation = _mapper.Map<ChallengeParticipantResponse>(participant);
                }
            }

            // Get top participants (limited to 5)
            var topParticipants = challenge.Participants
                .OrderByDescending(p => p.Progress)
                .Take(5)
                .Select((p, index) =>
                {
                    var dto = _mapper.Map<ChallengeParticipantResponse>(p);
                    dto.Rank = index + 1;
                    return dto;
                })
                .ToList();

            response.TopParticipants = topParticipants;
            response.ParticipantCount = challenge.Participants.Count;

            return response;
        }
    }
}
