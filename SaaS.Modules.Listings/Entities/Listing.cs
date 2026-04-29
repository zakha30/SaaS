using SaaS.Shared;

namespace SaaS.Modules.Listings.Entities;

public sealed class Listing : TenantEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ListingType Type { get; set; } = ListingType.Shipment;
    public string LocationFrom { get; set; } = string.Empty;
    public string LocationTo { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public ListingStatus Status { get; set; } = ListingStatus.Draft;
    public Guid UserId { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? VolumeM3 { get; set; }
    public string? CargoType { get; set; }
    public DateTime? AvailableFrom { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public enum ListingType
{
    Shipment,    // cargo that needs to be transported
    Truck,       // available truck looking for cargo
    Warehouse    // available storage space
}

public enum ListingStatus
{
    Draft,
    Active,
    Paused,
    Archived
}