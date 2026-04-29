using AutoMapper;
using SaaS.Modules.Listings.DTOs;
using SaaS.Modules.Listings.Entities;

namespace SaaS.Modules.Listings.Mappings;

public sealed class ListingProfile : Profile
{
    public ListingProfile()
    {
        // ── Listing → ListingResponseDto ──────────────────────────────────────
        CreateMap<Listing, ListingResponseDto>()
            .ForMember(dest => dest.TypeLabel,
                opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.StatusLabel,
                opt => opt.MapFrom(src => src.Status.ToString()));

        // ── CreateListingDto → Listing ────────────────────────────────────────
        // UserId and TenantId are set by the service after mapping
        CreateMap<CreateListingDto, Listing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => ListingStatus.Draft))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        // ── UpdateListingDto → Listing (patch — nulls are skipped) ────────────
        CreateMap<UpdateListingDto, Listing>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition(
                (_, _, srcMember) => srcMember is not null));
    }
}