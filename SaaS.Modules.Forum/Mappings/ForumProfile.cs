using AutoMapper;
using SaaS.Modules.Forum.DTOs;
using SaaS.Modules.Forum.Entities;

namespace SaaS.Modules.Forum.Mappings;

public sealed class ForumProfile : Profile
{
    public ForumProfile()
    {
        CreateMap<CreateThreadDto, ForumThread>()
            .ForMember(dest => dest.MembershipTier, opt => opt.MapFrom(src => src.MembershipTier ?? "Free"))
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => System.DateTime.UtcNow.AddDays(365)));

        CreateMap<ForumThread, ThreadResponseDto>();

        CreateMap<CreatePostDto, ForumPost>();
        CreateMap<ForumPost, PostResponseDto>();
    }
}
