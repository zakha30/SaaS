using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Modules.Classifieds.DTOs;

public sealed class CreateClassifiedDto
{
    public Guid? PostedByUserId { get; set; }
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required]
    [MaxLength(16)]
    public string TradeKind { get; set; } = "Sell";
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Province { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? City { get; set; }
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    [Range(typeof(decimal), "0", "10000000")]
    public decimal? Price { get; set; }
    [MaxLength(10)]
    public string Currency { get; set; } = "ZAR";
    [Url]
    public string? ImageUrl { get; set; }
    public string? MembershipTier { get; set; }
}

public sealed class ClassifiedResponseDto
{
    public Guid Id { get; set; }
    public Guid? PostedByUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? City { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = "Active";
    public string MembershipTier { get; set; } = "Free";
    public DateTime ExpiresAt { get; set; }
}

public sealed record ClassifiedFilterDto
{
    public string? Category { get; init; }
    public string? Province { get; init; }
    public string? TradeKind { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
