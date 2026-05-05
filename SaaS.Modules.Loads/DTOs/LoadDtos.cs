using System;
using System.ComponentModel.DataAnnotations;
using SaaS.Modules.Loads.Entities;

namespace SaaS.Modules.Loads.DTOs;

public sealed class CreateLoadDto
{
    [Required]
    [MaxLength(100)]
    public string DepartureProvince { get; set; } = string.Empty;

    [MaxLength(100)]
    public string DepartureCountry { get; set; } = "South Africa";

    [Required]
    [MaxLength(100)]
    public string DepartureCity { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string DestinationProvince { get; set; } = string.Empty;

    [MaxLength(100)]
    public string DestinationCountry { get; set; } = "South Africa";

    [Required]
    [MaxLength(100)]
    public string DestinationCity { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Commodity { get; set; } = string.Empty;

    [MaxLength(100)]
    public string WeightBracket { get; set; } = string.Empty;

    public decimal? WeightKg { get; set; }

    [MaxLength(100)]
    public string? TruckTypeRequired { get; set; }

    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;

    [Phone]
    public string? ContactPhone { get; set; }

    [MaxLength(2000)]
    public string? AdditionalNotes { get; set; }

    public bool IsCrossBorder { get; set; }
    public DateTime? LoadDate { get; set; }

    public string? MembershipTier { get; set; }
}

public sealed class LoadResponseDto
{
    public Guid Id { get; set; }
    public Guid? PostedByUserId { get; set; }
    public string DepartureProvince { get; set; } = string.Empty;
    public string DepartureCountry { get; set; } = "South Africa";
    public string DepartureCity { get; set; } = string.Empty;
    public string DestinationProvince { get; set; } = string.Empty;
    public string DestinationCountry { get; set; } = "South Africa";
    public string DestinationCity { get; set; } = string.Empty;
    public string Commodity { get; set; } = string.Empty;
    public string WeightBracket { get; set; } = string.Empty;
    public decimal? WeightKg { get; set; }
    public string? TruckTypeRequired { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? AdditionalNotes { get; set; }
    public bool IsCrossBorder { get; set; }
    public DateTime? LoadDate { get; set; }
    public string Status { get; set; } = "Active";
    public string MembershipTier { get; set; } = "Free";
    public DateTime ExpiresAt { get; set; }
}

public sealed record LoadFilterDto
{
    public string? DepartureProvince { get; init; }
    public string? DestinationProvince { get; init; }
    public string? TruckTypeRequired { get; init; }
    public bool? IsCrossBorder { get; init; }
    public decimal? MinWeightKg { get; init; }
    public string? Status { get; init; }
    /// <summary>Case-insensitive partial match on commodity text.</summary>
    public string? Commodity { get; init; }
    /// <summary>Partial match on weight bracket (e.g. t, ton).</summary>
    public string? WeightBracket { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
