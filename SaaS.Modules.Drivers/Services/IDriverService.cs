using SaaS.Modules.Drivers.DTOs;
using SaaS.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.Modules.Drivers.Services
{
    public interface IDriverService
    {
        Task<Result<DriverResponseDto>> CreateAsync(CreateDriverDto dto, Guid userId, CancellationToken ct = default);
        Task<Result<DriverResponseDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Result<PagedResult<DriverResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<Result<PagedResult<DriverResponseDto>>> GetFilteredAsync(DriverFilterDto filter, CancellationToken ct = default);
        Task<Result<DriverResponseDto>> UpdateAsync(Guid id, UpdateDriverDto dto, Guid userId, CancellationToken ct = default);
        Task<Result<string>> DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
    }
}
