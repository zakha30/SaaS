using System;
using System.ComponentModel.DataAnnotations;
using SaaS.Infrastructure.Modules.Fleet.Entities;

namespace SaaS.Infrastructure.Modules.Fleet.DTOs;

public sealed class CreateVehicleDto
{
    [Required]
    [MaxLength(50)]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Make { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    [Required]
    [Range(1900, 2100)]
    public int Year { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = VehicleStatus.Available;

    // New fields
    [Required]
    public VehicleCategory Category { get; set; } = VehicleCategory.Truck;

    [Required]
    [MaxLength(100)]
    public string TruckType { get; set; } = string.Empty; // e.g. Flatdeck, Tautliner

    [MaxLength(100)]
    public string? TrailerType { get; set; }

    [Required]
    [Range(typeof(decimal), "0", "1000000")]
    public decimal PayloadTons { get; set; }

    [Required]
    [MaxLength(100)]
    public string Province { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? City { get; set; }

    [Required]
    public bool IsCrossBorderCapable { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string ContactEmail { get; set; } = string.Empty;

    [Phone]
    [MaxLength(50)]
    public string? ContactPhone { get; set; }

    [Range(typeof(decimal), "0", "10000000")]
    public decimal? DailyRate { get; set; }

    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = "ZAR";

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Url]
    [MaxLength(1000)]
    public string? ImageUrl { get; set; }

    [Required]
    [MaxLength(50)]
    public string MembershipTier { get; set; } = MembershipTierConstants.Free;

    [Required]
    public Guid PostedByUserId { get; set; }
}

public sealed class VehicleResponseDto
{
    public Guid Id { get; set; }

    public string RegistrationNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Status { get; set; } = string.Empty;

    // New fields
    public VehicleCategory Category { get; set; }
    public string? TruckType { get; set; }
    public string? TrailerType { get; set; }
    public decimal PayloadTons { get; set; }
    public string Province { get; set; } = string.Empty;
    public string? City { get; set; }
    public bool IsCrossBorderCapable { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public decimal? DailyRate { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string MembershipTier { get; set; } = MembershipTierConstants.Free;
    public Guid PostedByUserId { get; set; }

    // TenantEntity fields if any (CreatedAt etc.) are handled elsewhere
}

public sealed class UpdateVehicleDto
{
    [MaxLength(50)]
    public string? RegistrationNumber { get; set; }

    [MaxLength(100)]
    public string? Make { get; set; }

    [MaxLength(100)]
    public string? Model { get; set; }

    [Range(1900, 2100)]
    public int? Year { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    // New fields (nullable)
    public VehicleCategory? Category { get; set; }
    [MaxLength(100)]
    public string? TruckType { get; set; }
    [MaxLength(100)]
    public string? TrailerType { get; set; }
    [Range(typeof(decimal), "0", "1000000")]
    public decimal? PayloadTons { get; set; }
    [MaxLength(100)]
    public string? Province { get; set; }
    [MaxLength(100)]
    public string? City { get; set; }
    public bool? IsCrossBorderCapable { get; set; }
    [EmailAddress]
    [MaxLength(256)]
    public string? ContactEmail { get; set; }
    [Phone]
    [MaxLength(50)]
    public string? ContactPhone { get; set; }
    [Range(typeof(decimal), "0", "10000000")]
    public decimal? DailyRate { get; set; }
    [MaxLength(10)]
    public string? Currency { get; set; }
    [MaxLength(2000)]
    public string? Description { get; set; }
    [Url]
    [MaxLength(1000)]
    public string? ImageUrl { get; set; }
    [MaxLength(50)]
    public string? MembershipTier { get; set; }
    public Guid? PostedByUserId { get; set; }
}

public sealed record VehicleFilterDto
{
    public VehicleCategory? Category { get; init; }
    public string? Province { get; init; }
    public string? TruckType { get; init; }
    public bool? IsCrossBorderCapable { get; init; }
    public decimal? MinPayloadTons { get; init; }
    public string? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
