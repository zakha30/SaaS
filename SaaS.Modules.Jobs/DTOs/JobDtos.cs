using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Modules.Jobs.DTOs;

public sealed class CreateJobDto
{
    public Guid? PostedByUserId { get; set; }
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(200)]
    public string Company { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Province { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? City { get; set; }
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;
    [Required]
    [MaxLength(32)]
    public string PostKind { get; set; } = "OfferingJob";
    [MaxLength(100)]
    public string EmploymentType { get; set; } = string.Empty;
    [Range(typeof(decimal), "0", "100000000")]
    public decimal? Salary { get; set; }
    [MaxLength(10)]
    public string Currency { get; set; } = "ZAR";
    public string? MembershipTier { get; set; }
}

public sealed class JobResponseDto
{
    public Guid Id { get; set; }
    public Guid? PostedByUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? City { get; set; }
    public string Description { get; set; } = string.Empty;
    public string PostKind { get; set; } = "OfferingJob";
    public string EmploymentType { get; set; } = string.Empty;
    public decimal? Salary { get; set; }
    public string Currency { get; set; } = "ZAR";
    public string Status { get; set; } = "Active";
    public string MembershipTier { get; set; } = "Free";
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public sealed record JobFilterDto
{
    public string? Company { get; init; }
    public string? Province { get; init; }
    public string? PostKind { get; init; }
    public string? EmploymentType { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
