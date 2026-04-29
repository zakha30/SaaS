using AutoMapper;
using SaaS.Modules.Quotes.DTOs;
using SaaS.Modules.Quotes.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SaaS.Modules.Quotes.Mappings;

public sealed class QuoteProfile : Profile
{
    public QuoteProfile()
    {
        // ── Quote → QuoteResponseDto ──────────────────────────────────────────
        CreateMap<Quote, QuoteResponseDto>()
            .ForMember(dest => dest.StatusLabel,
                opt => opt.MapFrom(src => src.Status.ToString()));

        // ── SubmitQuoteDto → Quote ────────────────────────────────────────────
        CreateMap<SubmitQuoteDto, Quote>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TransporterId, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => QuoteStatus.Pending))
            .ForMember(dest => dest.RejectionReason, opt => opt.Ignore())
            .ForMember(dest => dest.AcceptedAt, opt => opt.Ignore())
            .ForMember(dest => dest.RejectedAt, opt => opt.Ignore())
            .ForMember(dest => dest.WithdrawnAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}