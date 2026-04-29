using SaaS.Modules.Dashboard.DTOs;
using SaaS.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Modules.Dashboard.Services;

public interface IDashboardService
{
    Task<Result<DashboardSummaryDto>> GetSummaryAsync(Guid userId, CancellationToken ct = default);
    Task<Result<MyListingsResponseDto>> GetMyListingsAsync(Guid userId, MyListingsFilterDto filter, CancellationToken ct = default);
    Task<Result<MyQuotesResponseDto>> GetMyQuotesAsync(Guid userId, MyQuotesFilterDto filter, CancellationToken ct = default);
    Task<Result<ReceivedQuotesResponseDto>> GetReceivedQuotesAsync(Guid userId, ReceivedQuotesFilterDto filter, CancellationToken ct = default);
}