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

    public sealed class FleetImageService(
    IFleetImageRepository imageRepository,
    IVehicleRepository vehicleRepository,
    IMapper mapper) : IFleetImageService
    {
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
        private static readonly string[] AllowedContentTypes =
        {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

        public async Task<Result<FleetImageResponseDto>> UploadImageAsync(
            Guid vehicleId, IFormFile file, Guid userId, CancellationToken ct = default)
        {
            // Step 1: Validate vehicle exists
            var vehicle = await vehicleRepository.GetByIdAsync(vehicleId, ct);
            if (vehicle is null)
                return Result<FleetImageResponseDto>.Failure($"Vehicle with ID {vehicleId} not found.");

            // Step 2: Validate file
            if (file is null || file.Length == 0)
                return Result<FleetImageResponseDto>.Failure("No file provided.");

            if (file.Length > MaxFileSizeBytes)
                return Result<FleetImageResponseDto>.Failure($"File size exceeds {MaxFileSizeBytes / (1024 * 1024)}MB limit.");

            if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
                return Result<FleetImageResponseDto>.Failure(
                    $"File type '{file.ContentType}' is not allowed. Allowed types: JPG, PNG, WEBP, GIF.");

            try
            {
                // Step 3: Save file to disk
                var uploadDir = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "uploads", "vehicles", vehicleId.ToString());

                Directory.CreateDirectory(uploadDir);

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadDir, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(stream, ct);
                }

                // Step 4: Create database record
                var imagePath = $"/uploads/vehicles/{vehicleId}/{fileName}";  // ← ImagePath

                var image = new FleetImage
                {
                    VehicleId = vehicleId,  // ← not FleetId
                    ImagePath = imagePath,   // ← not ImageUrl
                    FileName = file.FileName,
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                await imageRepository.AddAsync(image, ct);
                await imageRepository.SaveChangesAsync(ct);

                return Result<FleetImageResponseDto>.Success(mapper.Map<FleetImageResponseDto>(image));
            }
            catch (Exception ex)
            {
                return Result<FleetImageResponseDto>.Failure(
                    $"Error uploading file: {ex.Message}");
            }
        }

        public async Task<Result<List<FleetImageResponseDto>>> GetFleetImagesAsync(
            Guid vehicleId, CancellationToken ct = default)
        {
            try
            {
                var images = await imageRepository.GetByVehicleIdAsync(vehicleId, ct);  // ← renamed
                var dtos = images.Select(mapper.Map<FleetImageResponseDto>).ToList();
                return Result<List<FleetImageResponseDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<List<FleetImageResponseDto>>.Failure(
                    $"Error retrieving images: {ex.Message}");
            }
        }

        public async Task<Result<string>> DeleteImageAsync(
            int imageId, Guid userId, CancellationToken ct = default)  // ← int imageId
        {
            try
            {
                var image = await imageRepository.GetByIdAsync(imageId, ct);
                if (image is null)
                    return Result<string>.Failure("Image not found.");

                // Delete from disk
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    image.ImagePath.TrimStart('/'));

                if (File.Exists(filePath))
                    File.Delete(filePath);

                // Delete from database
                await imageRepository.DeleteAsync(image, ct);
                await imageRepository.SaveChangesAsync(ct);

                return Result<string>.Success("Image deleted successfully.");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Error deleting image: {ex.Message}");
            }
        }
    }
}
