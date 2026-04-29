using System;
using SaaS.Shared;

namespace SaaS.Infrastructure.Modules.Fleet.Entities;

public sealed class Vehicle : TenantEntity
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Status { get; set; } = VehicleStatus.Available;

    // New fields
    public VehicleCategory Category { get; set; } = VehicleCategory.Truck;
    public string? TruckType { get; set; } // e.g. Flatdeck, Tautliner, Tipper, Bakkie, Van, LDV
    public string? TrailerType { get; set; } // Optional trailer info

    public decimal PayloadTons { get; set; } // Load capacity in tons

    public string Province { get; set; } = string.Empty; // Location / Province
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
}

public enum VehicleCategory
{
    Truck,
    Trailer,
    LCV,
    Bus,
    Other
}

public static class MembershipTierConstants
{
    public const string Free = "Free";
    public const string Member = "Member";
    public const string Premium = "Premium";
}

public static class VehicleStatus
{
    public const string Available = "Available";
    public const string InService = "InService";
    public const string Unavailable = "Unavailable";
}
