using AutoMapper;
using Microsoft.AspNetCore.Http;
using SaaS.Infrastructure.Modules.Fleet.DTOs;
using SaaS.Infrastructure.Modules.Fleet.Entities;
using SaaS.Infrastructure.Repositories.Fleet;
using SaaS.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace SaaS.Infrastructure.Modules.Fleet.Services;

public sealed class VehicleService(
    IVehicleRepository repository,
    IMapper mapper) : IVehicleService
{
    public async Task<Result<VehicleResponseDto>> CreateAsync(CreateVehicleDto dto, Guid userId, CancellationToken ct = default)
    {
        var vehicle = mapper.Map<Vehicle>(dto);
        await repository.AddAsync(vehicle, ct);
        await repository.SaveChangesAsync(ct);
        return Result<VehicleResponseDto>.Success(mapper.Map<VehicleResponseDto>(vehicle));
    }

    public async Task<Result<VehicleResponseDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var v = await repository.GetByIdAsync(id, ct);
        return v is null ? Result<VehicleResponseDto>.Failure("Vehicle not found.") : Result<VehicleResponseDto>.Success(mapper.Map<VehicleResponseDto>(v));
    }

    public async Task<Result<PagedResult<VehicleResponseDto>>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : pageSize > 100 ? 100 : pageSize;
        var paged = await repository.GetPagedAsync(page, pageSize, ct);
        return Result<PagedResult<VehicleResponseDto>>.Success(new PagedResult<VehicleResponseDto>(paged.Items.Select(mapper.Map<VehicleResponseDto>).ToList(), paged.TotalCount, paged.Page, paged.PageSize));
    }

    public async Task<Result<VehicleResponseDto>> UpdateAsync(Guid id, UpdateVehicleDto dto, Guid requestingUserId, CancellationToken ct = default)
    {
        var v = await repository.GetByIdAsync(id, ct);
        if (v is null) return Result<VehicleResponseDto>.Failure("Vehicle not found.");

        mapper.Map(dto, v);
        await repository.SaveChangesAsync(ct);
        return Result<VehicleResponseDto>.Success(mapper.Map<VehicleResponseDto>(v));
    }

    public async Task<Result<PagedResult<VehicleResponseDto>>> GetFilteredAsync(
    VehicleFilterDto filter, CancellationToken ct = default)
    {
        var paged = await repository.GetFilteredAsync(filter, ct);
        return Result<PagedResult<VehicleResponseDto>>.Success(
            new PagedResult<VehicleResponseDto>(
                paged.Items.Select(mapper.Map<VehicleResponseDto>).ToList(),
                paged.TotalCount, paged.Page, paged.PageSize));
    }

    public async Task<Result<string>> UploadImageAsync(
    IFormFile file, CancellationToken ct)
    {
        try
        {
            // Create upload directory if it doesn't exist
            var uploadDir = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot", "uploads", "vehicles");

            Directory.CreateDirectory(uploadDir);

            // Generate unique filename
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadDir, fileName);

            // Save file
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream, ct);

            // Return public URL
            var publicUrl = $"/uploads/vehicles/{fileName}";
            return Result<string>.Success(publicUrl);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Image upload failed: {ex.Message}");
        }
    }
}
