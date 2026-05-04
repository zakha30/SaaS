using SaaS.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.Modules.Drivers.Entities
{
    public sealed class Driver : TenantEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string LicenseClass { get; set; } = string.Empty;  // e.g. A, B, C, EC
        public string Status { get; set; } = DriverStatus.Active;
        public string Region { get; set; } = string.Empty;  // SA region key
        public string City { get; set; } = string.Empty;
        public int YearsExperience { get; set; }
        public decimal Rating { get; set; } = 0m;
        public int TripsCompleted { get; set; }
        public string? Notes { get; set; }
        public Guid CreatedByUserId { get; set; }
    }

    public static class DriverStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string OnTrip = "OnTrip";
    }
}
