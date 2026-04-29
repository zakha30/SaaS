using AutoMapper;
using SaaS.Modules.Directory.DTOs;
using SaaS.Modules.Directory.Entities;

namespace SaaS.Modules.Directory.Mappings;

public sealed class DirectoryProfile : Profile
{
    public DirectoryProfile()
    {
        CreateMap<CreateDirectoryEntryDto, BusinessDirectoryEntry>()
            .ForMember(dest => dest.MembershipTier, opt => opt.MapFrom(src => src.MembershipTier ?? "Free"))
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => System.DateTime.UtcNow.AddDays(365)));

        CreateMap<BusinessDirectoryEntry, DirectoryResponseDto>();
    }
}
