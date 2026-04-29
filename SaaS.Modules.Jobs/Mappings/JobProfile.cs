using AutoMapper;
using SaaS.Modules.Jobs.DTOs;
using SaaS.Modules.Jobs.Entities;

namespace SaaS.Modules.Jobs.Mappings;

public sealed class JobProfile : Profile
{
    public JobProfile()
    {
        CreateMap<CreateJobDto, JobListing>()
            .ForMember(dest => dest.MembershipTier, opt => opt.MapFrom(src => src.MembershipTier ?? "Free"))
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => System.DateTime.UtcNow.AddDays(90)));

        CreateMap<JobListing, JobResponseDto>();
    }
}
