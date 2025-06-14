using AutoMapper;
using FitnessVibe.Application.DTOs.Activities;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.ValueObjects;

namespace FitnessVibe.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping activity-related entities and DTOs
/// </summary>
public class ActivityMappingProfile : Profile
{
    public ActivityMappingProfile()
    {
        // Activity route mapping
        CreateMap<GpsPoint, GpsPointDto>()
            .ReverseMap();

        CreateMap<ActivityRoute, ActivityRouteDto>()
            .ForMember(dest => dest.Points, opt => opt.MapFrom(src => src.Points))
            .ReverseMap();

        // Activity mapping
        CreateMap<Activity, LiveActivityResponse>()
            .ForMember(dest => dest.Route, opt => opt.MapFrom(src => src.Route))
            .ReverseMap();

        CreateMap<Activity, UserActivityResponse>()
            .ForMember(dest => dest.Route, opt => opt.MapFrom(src => src.Route))
            .ReverseMap();

        CreateMap<ActivityTemplate, ActivityTemplateResponse>()
            .ReverseMap();
    }
}
