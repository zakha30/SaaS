using AutoMapper;
using SaaS.Modules.Classifieds.DTOs;
using SaaS.Modules.Classifieds.Entities;

namespace SaaS.Modules.Classifieds.Mappings;

public sealed class ClassifiedProfile : Profile
{
    public ClassifiedProfile()
    {
        CreateMap<CreateClassifiedDto, ClassifiedItem>()
            .ForMember(dest => dest.MembershipTier, opt => opt.MapFrom(src => src.MembershipTier ?? "Free"))
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => System.DateTime.UtcNow.AddDays(90)));

        CreateMap<ClassifiedItem, ClassifiedResponseDto>();
    }
}
