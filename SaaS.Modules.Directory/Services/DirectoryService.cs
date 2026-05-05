using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using SaaS.Modules.Directory.DTOs;
using SaaS.Modules.Directory.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Directory.Services;

public sealed class DirectoryService : IDirectoryService
{
    private readonly IDirectoryRepository repo;
    private readonly IMapper mapper;

    public DirectoryService(IDirectoryRepository repo, IMapper mapper)
    {
        this.repo = repo;
        this.mapper = mapper;
    }

    public async Task<PagedResult<DirectoryResponseDto>> GetFilteredAsync(DirectoryFilterDto filter, CancellationToken ct = default)
    {
        var paged = await repo.GetFilteredAsync(filter, ct);
        var dtos = paged.Items.Select(mapper.Map<DirectoryResponseDto>).ToList();
        return new PagedResult<DirectoryResponseDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }

    public async Task<DirectoryResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        return entity is null ? null : mapper.Map<DirectoryResponseDto>(entity);
    }

    public async Task<DirectoryResponseDto> CreateAsync(CreateDirectoryEntryDto dto, CancellationToken ct = default)
    {
        var entity = mapper.Map<Entities.BusinessDirectoryEntry>(dto);
        entity.MembershipTier ??= "Free";
        entity.ExpiresAt = DateTime.UtcNow.AddDays(365);
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return mapper.Map<DirectoryResponseDto>(entity);
    }
}
