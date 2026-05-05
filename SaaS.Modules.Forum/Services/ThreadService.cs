using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using SaaS.Modules.Forum.DTOs;
using SaaS.Modules.Forum.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Forum.Services;

public sealed class ThreadService : IThreadService
{
    private readonly IThreadRepository repo;
    private readonly IMapper mapper;

    public ThreadService(IThreadRepository repo, IMapper mapper)
    {
        this.repo = repo;
        this.mapper = mapper;
    }

    public async Task<PagedResult<ThreadResponseDto>> GetFilteredAsync(ThreadFilterDto filter, CancellationToken ct = default)
    {
        var paged = await repo.GetFilteredAsync(filter, ct);
        var dtos = paged.Items.Select(i => mapper.Map<ThreadResponseDto>(i)).ToList();
        return new PagedResult<ThreadResponseDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }

    public async Task<ThreadResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repo.GetByIdAsync(id, ct);
        return entity is null ? null : mapper.Map<ThreadResponseDto>(entity);
    }

    public async Task<ThreadResponseDto> CreateAsync(CreateThreadDto dto, CancellationToken ct = default)
    {
        var entity = mapper.Map<Entities.ForumThread>(dto);
        entity.MembershipTier ??= "Free";
        entity.ExpiresAt = DateTime.UtcNow.AddDays(365);
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return mapper.Map<ThreadResponseDto>(entity);
    }
}
