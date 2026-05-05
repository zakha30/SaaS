using System.ComponentModel.DataAnnotations;
using SaaS.Modules.Auth.Entities;

namespace SaaS.Modules.Auth.DTOs;

public sealed record TenantUserListItemDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool EmailVerified);

public sealed class CreateTenantUserDto
{
    [Required, MinLength(2), MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MinLength(2), MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(64)]
    public string Password { get; set; } = string.Empty;

    /// <summary>Assignable tenant role (see <see cref="UserRoles.AssignableByTenantAdmin"/>).</summary>
    [Required, MinLength(2), MaxLength(64)]
    public string Role { get; set; } = UserRoles.User;
}
