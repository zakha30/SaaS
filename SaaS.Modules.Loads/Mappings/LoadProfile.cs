using AutoMapper;
using SaaS.Modules.Loads.DTOs;
using SaaS.Modules.Loads.Entities;

namespace SaaS.Modules.Loads.Mappings;

public sealed class LoadProfile : Profile
{
    public LoadProfile()
    {
        CreateMap<CreateLoadDto, AvailableLoad>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
            .ForMember(dest => dest.MembershipTier, opt => opt.MapFrom(src => src.MembershipTier ?? "Free"))
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => System.DateTime.UtcNow.AddDays(30)));

        CreateMap<AvailableLoad, LoadResponseDto>();

        CreateMap<CreateQuoteRequestDto, QuoteRequest>();
        CreateMap<QuoteRequest, QuoteRequestResponseDto>();

        CreateMap<QuoteSubmissionDto, QuoteSubmission>();
    }
}
