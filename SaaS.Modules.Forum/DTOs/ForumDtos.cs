using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Modules.Forum.DTOs;

public sealed class CreateThreadDto
{
    public Guid? PostedByUserId { get; set; }
    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(5000)]
    public string? Content { get; set; }
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    public string? MembershipTier { get; set; }
}

public sealed class ThreadResponseDto
{
    public Guid Id { get; set; }
    public Guid? PostedByUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string MembershipTier { get; set; } = "Free";
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public sealed class CreatePostDto
{
    public Guid ThreadId { get; set; }
    public Guid? PostedByUserId { get; set; }
    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;
}

public sealed class PostResponseDto
{
    public Guid Id { get; set; }
    public Guid ThreadId { get; set; }
    public Guid? PostedByUserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
}

public sealed record ThreadFilterDto
{
    public string? Category { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
