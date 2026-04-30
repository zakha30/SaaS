using SaaS.Infrastructure.Modules.Fleet.DTOs;
using SaaS.Infrastructure.Modules.Fleet.Entities;
using SaaS.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Repositories.Fleet;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Vehicle>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(Vehicle vehicle, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<PagedResult<Vehicle>> GetFilteredAsync(Modules.Fleet.DTOs.VehicleFilterDto filter, CancellationToken ct = default);
}

public interface IFleetImageRepository
{
    Task<FleetImage?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<List<FleetImage>> GetByFleetIdAsync(Guid fleetId, CancellationToken ct = default);

    Task AddAsync(FleetImage image, CancellationToken ct = default);

    Task DeleteAsync(FleetImage image, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}