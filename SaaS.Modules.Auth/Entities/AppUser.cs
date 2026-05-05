using SaaS.Shared;

namespace SaaS.Modules.Auth.Entities;

public sealed class AppUser : TenantEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = UserRoles.User;
    public bool EmailVerified { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}

public sealed class RefreshToken : TenantEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }

    public AppUser User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}

public static class UserRoles
{
    /// <summary>Platform operator — not assignable by tenant admins.</summary>
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>Tenant administrator — full access within the tenant.</summary>
    public const string Admin = "Admin";

    /// <summary>Legacy / generic member.</summary>
    public const string User = "User";

    public const string Shipper = "Shipper";
    public const string Transporter = "Transporter";
    public const string FleetOwner = "FleetOwner";
    public const string Driver = "Driver";

    public static readonly IReadOnlyList<string> All =
        [SuperAdmin, Admin, User, Shipper, Transporter, FleetOwner, Driver];

    /// <summary>Roles a tenant Admin may assign when inviting users (excludes SuperAdmin).</summary>
    public static readonly IReadOnlyList<string> AssignableByTenantAdmin =
        [Admin, User, Shipper, Transporter, FleetOwner, Driver];

    public static bool IsValid(string role) =>
        All.Contains(role, StringComparer.OrdinalIgnoreCase);

    public static bool CanTenantAdminAssign(string role) =>
        AssignableByTenantAdmin.Contains(role, StringComparer.OrdinalIgnoreCase);

    public static bool IsTenantAdmin(string role) =>
        string.Equals(role, Admin, StringComparison.OrdinalIgnoreCase)
        || string.Equals(role, SuperAdmin, StringComparison.OrdinalIgnoreCase);
}