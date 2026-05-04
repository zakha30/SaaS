using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SaaS.Modules.Drivers.DTOs
{
    public sealed class CreateDriverDto
    {
        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, Phone, MaxLength(50)]
        public string Phone { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string LicenseClass { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Active";

        [Required, MaxLength(100)]
        public string Region { get; set; } = string.Empty;

        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Range(0, 60)]
        public int YearsExperience { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }
    }

    public sealed class UpdateDriverDto
    {
        [MaxLength(200)] public string? FullName { get; set; }
        [EmailAddress, MaxLength(256)] public string? Email { get; set; }
        [Phone, MaxLength(50)] public string? Phone { get; set; }
        [MaxLength(100)] public string? LicenseNumber { get; set; }
        [MaxLength(20)] public string? LicenseClass { get; set; }
        [MaxLength(50)] public string? Status { get; set; }
        [MaxLength(100)] public string? Region { get; set; }
        [MaxLength(100)] public string? City { get; set; }
        [Range(0, 60)] public int? YearsExperience { get; set; }
        [MaxLength(2000)] public string? Notes { get; set; }
    }

    public sealed class DriverResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string LicenseClass { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int YearsExperience { get; set; }
        public decimal Rating { get; set; }
        public int TripsCompleted { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public sealed record DriverFilterDto
    {
        public string? Status { get; init; }
        public string? Region { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 12;
    }
}
