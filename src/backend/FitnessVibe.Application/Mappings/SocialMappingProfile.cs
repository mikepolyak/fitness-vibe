using AutoMapper;
using FitnessVibe.Application.DTOs.Social;
using FitnessVibe.Domain.Entities.Social;

namespace FitnessVibe.Application.Mappings;

/// <summary>
/// AutoMapper profile for social feature mappings
/// </summary>
public class SocialMappingProfile : Profile
{
    public SocialMappingProfile()
    {
        CreateMap<UserConnection, UserConnectionDto>()
            .ForMember(dest => dest.FollowerUsername, opt => opt.MapFrom(src => src.Follower.Username))
            .ForMember(dest => dest.FollowerProfilePicture, opt => opt.MapFrom(src => src.Follower.ProfilePicture))
            .ForMember(dest => dest.FollowedUsername, opt => opt.MapFrom(src => src.Followed.Username))
            .ForMember(dest => dest.FollowedProfilePicture, opt => opt.MapFrom(src => src.Followed.ProfilePicture));

        CreateMap<ActivityShare, ActivityShareDto>()
            .ForMember(dest => dest.UserUsername, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.UserProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture))
            .ForMember(dest => dest.ActivityTitle, opt => opt.MapFrom(src => src.Activity.Title))
            .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.Activity.Type))
            .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.RecentComments, opt => opt.MapFrom(src => src.Comments.OrderByDescending(c => c.CreatedAt).Take(3)));

        CreateMap<ActivityLike, ActivityLikeDto>()
            .ForMember(dest => dest.UserUsername, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.UserProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture));

        CreateMap<ActivityComment, ActivityCommentDto>()
            .ForMember(dest => dest.UserUsername, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.UserProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture));
    }
}
