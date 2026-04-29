using System;
using System.ComponentModel.DataAnnotations;

namespace SaaS.Modules.Loads.DTOs;

public sealed class CreateQuoteRequestDto
{
    [Required]
    public Guid LoadId { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    [Range(typeof(decimal), "0", "10000000")]
    public decimal? Budget { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "ZAR";

    public DateTime? NeededBy { get; set; }
}

public sealed class QuoteSubmissionDto
{
    [Required]
    public Guid QuoteRequestId { get; set; }

    [Required]
    public decimal Price { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "ZAR";

    [MaxLength(2000)]
    public string? Notes { get; set; }
}

public sealed class QuoteRequestResponseDto
{
    public Guid Id { get; set; }
    public Guid LoadId { get; set; }
    public string Notes { get; set; } = string.Empty;
    public decimal? Budget { get; set; }
    public string Currency { get; set; } = "ZAR";
    public DateTime? NeededBy { get; set; }
    public string Status { get; set; } = "Open";
}
