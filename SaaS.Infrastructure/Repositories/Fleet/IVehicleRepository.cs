using System;
using System.Threading;
using System.Threading.Tasks;
using SaaS.Infrastructure.Modules.Fleet.Entities;
using SaaS.Shared;

namespace SaaS.Infrastructure.Repositories.Fleet;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Vehicle>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(Vehicle vehicle, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<PagedResult<Vehicle>> GetFilteredAsync(Modules.Fleet.DTOs.VehicleFilterDto filter, CancellationToken ct = default);
}