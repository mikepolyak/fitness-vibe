using AutoMapper;
using FitnessVibe.Application.DTOs.Challenges;
using FitnessVibe.Domain.Entities.Challenges;

namespace FitnessVibe.Application.Mappings;

public class ChallengeMappingProfile : Profile
{
    public ChallengeMappingProfile()
    {
        CreateMap<Challenge, ChallengeResponse>()
            .ForMember(dest => dest.ParticipantCount,
                opt => opt.MapFrom(src => src.Participants.Count));

        CreateMap<ChallengeParticipant, ChallengeParticipantResponse>()
            .ForMember(dest => dest.ChallengeName,
                opt => opt.MapFrom(src => src.Challenge.Name))
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User.UserName));
    }
}
