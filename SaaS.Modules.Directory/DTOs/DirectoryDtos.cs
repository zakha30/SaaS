using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Modules.Directory.DTOs;

public sealed class CreateDirectoryEntryDto
{
    public Guid? PostedByUserId { get; set; }
    [Required]
    [MaxLength(200)]
    public string BusinessName { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Province { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? City { get; set; }
    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;
    [Phone]
    public string? ContactPhone { get; set; }
    [MaxLength(2000)]
    public string? Description { get; set; }
    [Url]
    public string? Website { get; set; }
    public string? MembershipTier { get; set; }
}

public sealed class DirectoryResponseDto
{
    public Guid Id { get; set; }
    public Guid? PostedByUserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? City { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string MembershipTier { get; set; } = "Free";
    public DateTime ExpiresAt { get; set; }
}

public sealed record DirectoryFilterDto
{
    public string? Category { get; init; }
    public string? Province { get; init; }
    public string? Slug { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
