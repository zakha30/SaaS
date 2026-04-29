using System;
using SaaS.Shared;

namespace SaaS.Modules.Loads.Entities;

public sealed class AvailableLoad : TenantEntity
{
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
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(30);
}
