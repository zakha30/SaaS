using SaaS.Modules.Quotes.DTOs;
using SaaS.Modules.Quotes.Entities;
using SaaS.Shared;

namespace SaaS.Modules.Quotes.Services;

public interface IQuoteService
{
    Task<Result<QuoteResponseDto>> SubmitAsync(SubmitQuoteDto dto, Guid transporterId, CancellationToken ct = default);
    Task<Result<QuoteResponseDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PagedResult<QuoteResponseDto>>> GetByFilterAsync(QuoteFilterDto filter, CancellationToken ct = default);
    Task<Result<QuoteResponseDto>> AcceptAsync(Guid id, Guid requestingUserId, AcceptQuoteDto dto, CancellationToken ct = default);
    Task<Result<QuoteResponseDto>> RejectAsync(Guid id, Guid requestingUserId, RejectQuoteDto dto, CancellationToken ct = default);
    Task<Result<QuoteResponseDto>> WithdrawAsync(Guid id, Guid transporterId, WithdrawQuoteDto dto, CancellationToken ct = default);
}