using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Loads.DTOs;

namespace SaaS.Modules.Loads.Services;

public interface IQuoteRequestService
{
    Task<QuoteRequestResponseDto> CreateAsync(CreateQuoteRequestDto dto, CancellationToken ct = default);
    Task SubmitQuoteAsync(QuoteSubmissionDto dto, CancellationToken ct = default);
}
