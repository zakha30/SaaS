using Microsoft.AspNetCore.Http;
using SaaS.Infrastructure.Modules.Fleet.DTOs;
using SaaS.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Modules.Fleet.Services;

public interface IVehicleService
{
    Task<Result<VehicleResponseDto>> CreateAsync(CreateVehicleDto dto, Guid userId, CancellationToken ct = default);
    Task<Result<VehicleResponseDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PagedResult<VehicleResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Result<VehicleResponseDto>> UpdateAsync(Guid id, UpdateVehicleDto dto, Guid requestingUserId, CancellationToken ct = default);
    Task<Result<PagedResult<VehicleResponseDto>>> GetFilteredAsync(VehicleFilterDto filter, CancellationToken ct = default);

    Task<Result<string>> UploadImageAsync(IFormFile file, CancellationToken ct);
}

public interface IFleetImageService
{
    Task<Result<FleetImageResponseDto>> UploadImageAsync(
        Guid fleetId, IFormFile file, string userId, CancellationToken ct = default);

    Task<Result<List<FleetImageResponseDto>>> GetFleetImagesAsync(
        Guid fleetId, CancellationToken ct = default);

    Task<Result<string>> DeleteImageAsync(
        int imageId, Guid userId, CancellationToken ct = default);
}
