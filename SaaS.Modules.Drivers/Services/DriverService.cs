using AutoMapper;
using SaaS.Modules.Drivers.DTOs;
using SaaS.Modules.Drivers.Entities;
using SaaS.Modules.Drivers.Repositories;
using SaaS.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.Modules.Drivers.Services
{
    public sealed class DriverService(
    IDriverRepository repository,
    IMapper mapper) : IDriverService
    {
        public async Task<Result<DriverResponseDto>> CreateAsync(
            CreateDriverDto dto, Guid userId, CancellationToken ct = default)
        {
            // Prevent duplicate email within tenant (global query filter handles tenant scoping)
            var duplicate = await repository.ExistsByEmailAsync(dto.Email, excludeId: null, ct);
            if (duplicate)
                return Result<DriverResponseDto>.Failure("A driver with this email already exists.");

            var driver = mapper.Map<Driver>(dto);
            driver.CreatedByUserId = userId;

            await repository.AddAsync(driver, ct);
            await repository.SaveChangesAsync(ct);

            return Result<DriverResponseDto>.Success(mapper.Map<DriverResponseDto>(driver));
        }

        public async Task<Result<DriverResponseDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var driver = await repository.GetByIdAsync(id, ct);
            return driver is null
                ? Result<DriverResponseDto>.Failure("Driver not found.")
                : Result<DriverResponseDto>.Success(mapper.Map<DriverResponseDto>(driver));
        }

        public async Task<Result<PagedResult<DriverResponseDto>>> GetPagedAsync(
            int page, int pageSize, CancellationToken ct = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var paged = await repository.GetPagedAsync(page, pageSize, ct);
            return Result<PagedResult<DriverResponseDto>>.Success(new PagedResult<DriverResponseDto>(
                paged.Items.Select(mapper.Map<DriverResponseDto>).ToList(),
                paged.TotalCount, paged.Page, paged.PageSize));
        }

        public async Task<Result<PagedResult<DriverResponseDto>>> GetFilteredAsync(
            DriverFilterDto filter, CancellationToken ct = default)
        {
            var paged = await repository.GetFilteredAsync(filter, ct);
            return Result<PagedResult<DriverResponseDto>>.Success(new PagedResult<DriverResponseDto>(
                paged.Items.Select(mapper.Map<DriverResponseDto>).ToList(),
                paged.TotalCount, paged.Page, paged.PageSize));
        }

        public async Task<Result<DriverResponseDto>> UpdateAsync(
            Guid id, UpdateDriverDto dto, Guid userId, CancellationToken ct = default)
        {
            var driver = await repository.GetByIdAsync(id, ct);
            if (driver is null)
                return Result<DriverResponseDto>.Failure("Driver not found.");

            // Check email uniqueness if email is being changed
            if (dto.Email is not null && dto.Email != driver.Email)
            {
                var duplicate = await repository.ExistsByEmailAsync(dto.Email, excludeId: id, ct);
                if (duplicate)
                    return Result<DriverResponseDto>.Failure("A driver with this email already exists.");
            }

            mapper.Map(dto, driver);
            await repository.SaveChangesAsync(ct);

            return Result<DriverResponseDto>.Success(mapper.Map<DriverResponseDto>(driver));
        }

        public async Task<Result<string>> DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
        {
            var driver = await repository.GetByIdAsync(id, ct);
            if (driver is null)
                return Result<string>.Failure("Driver not found.");

            driver.IsDeleted = true;  // soft delete — consistent with other entities
            await repository.SaveChangesAsync(ct);

            return Result<string>.Success("Driver deleted successfully.");
        }
    }
}
