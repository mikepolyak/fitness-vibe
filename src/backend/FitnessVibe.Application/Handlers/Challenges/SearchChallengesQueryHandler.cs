using MediatR;
using AutoMapper;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Queries.Challenges;
using FitnessVibe.Application.DTOs.Challenges;

namespace FitnessVibe.Application.Handlers.Challenges
{
    /// <summary>
    /// Handler for searching challenges
    /// </summary>
    public class SearchChallengesQueryHandler : IRequestHandler<SearchChallengesQuery, IEnumerable<ChallengeResponse>>
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IMapper _mapper;

        public SearchChallengesQueryHandler(IChallengeRepository challengeRepository, IMapper mapper)
        {
            _challengeRepository = challengeRepository ?? throw new ArgumentNullException(nameof(challengeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<ChallengeResponse>> Handle(SearchChallengesQuery request, CancellationToken cancellationToken)
        {
            var challenges = await _challengeRepository.SearchChallengesAsync(
                request.IsActive,
                request.IsPrivate,
                request.StartDateFrom,
                request.StartDateTo,
                request.Type,
                request.ActivityType);

            // Apply additional filtering (search term)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                challenges = challenges.Where(c =>
                    c.Title.ToLower().Contains(searchTerm) ||
                    c.Description.ToLower().Contains(searchTerm));
            }

            // Apply sorting
            challenges = request.SortDirection.ToUpper() == "ASC" 
                ? ApplyAscendingSort(challenges, request.SortBy)
                : ApplyDescendingSort(challenges, request.SortBy);

            // Apply pagination
            var skip = (request.Page - 1) * request.PageSize;
            challenges = challenges.Skip(skip).Take(request.PageSize);

            var responses = challenges.Select(c =>
            {
                var response = _mapper.Map<ChallengeResponse>(c);

                if (request.UserId.HasValue)
                {
                    var participant = c.Participants.FirstOrDefault(p => p.UserId == request.UserId.Value);
                    if (participant != null)
                    {
                        response.UserParticipation = _mapper.Map<ChallengeParticipantResponse>(participant);
                    }
                }

                response.ParticipantCount = c.Participants.Count;

                return response;
            });

            return responses;
        }

        private static IEnumerable<Domain.Entities.Challenges.Challenge> ApplyAscendingSort(
            IEnumerable<Domain.Entities.Challenges.Challenge> challenges,
            string sortBy)
        {
            return sortBy.ToLower() switch
            {
                "startdate" => challenges.OrderBy(c => c.StartDate),
                "enddate" => challenges.OrderBy(c => c.EndDate ?? DateTime.MaxValue),
                "title" => challenges.OrderBy(c => c.Title),
                "participantcount" => challenges.OrderBy(c => c.Participants.Count),
                "type" => challenges.OrderBy(c => c.Type),
                _ => challenges.OrderBy(c => c.StartDate)
            };
        }

        private static IEnumerable<Domain.Entities.Challenges.Challenge> ApplyDescendingSort(
            IEnumerable<Domain.Entities.Challenges.Challenge> challenges,
            string sortBy)
        {
            return sortBy.ToLower() switch
            {
                "startdate" => challenges.OrderByDescending(c => c.StartDate),
                "enddate" => challenges.OrderByDescending(c => c.EndDate ?? DateTime.MaxValue),
                "title" => challenges.OrderByDescending(c => c.Title),
                "participantcount" => challenges.OrderByDescending(c => c.Participants.Count),
                "type" => challenges.OrderByDescending(c => c.Type),
                _ => challenges.OrderByDescending(c => c.StartDate)
            };
        }
    }
}
