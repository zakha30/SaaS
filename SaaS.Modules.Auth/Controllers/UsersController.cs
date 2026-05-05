using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SaaS.Modules.Auth.DTOs;
using SaaS.Modules.Auth.Entities;
using SaaS.Modules.Auth.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Auth.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Policy = "AdminOnly")]
public sealed class UsersController(
    IUserRepository userRepository,
    IPasswordHasher<AppUser> passwordHasher,
    ITenantContext tenantContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TenantUserListItemDto>), 200)]
    public async Task<IActionResult> List(CancellationToken ct = default)
    {
        var users = await userRepository.GetAllInTenantAsync(ct);
        var dtos = users.Select(u => new TenantUserListItemDto(
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.Role,
            u.EmailVerified)).ToList();
        return Ok(dtos);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TenantUserListItemDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateTenantUserDto dto, CancellationToken ct = default)
    {
        if (!UserRoles.CanTenantAdminAssign(dto.Role))
            return BadRequest(new { error = "That role cannot be assigned in a tenant." });

        var tenantId = tenantContext.TenantId;

        if (await userRepository.EmailExistsInTenantAsync(dto.Email, tenantId, ct))
            return BadRequest(new { error = "That email is already registered in this organisation." });

        var user = new AppUser
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.ToLowerInvariant().Trim(),
            Role = dto.Role.Trim(),
            TenantId = tenantId,
            EmailVerified = false,
        };

        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);

        await userRepository.AddAsync(user, ct);
        await userRepository.SaveChangesAsync(ct);

        var item = new TenantUserListItemDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role,
            user.EmailVerified);

        return Created(string.Empty, item);
    }
}
