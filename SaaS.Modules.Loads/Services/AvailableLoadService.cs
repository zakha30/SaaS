using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using SaaS.Modules.Loads.DTOs;
using SaaS.Modules.Loads.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Loads.Services;

public sealed class AvailableLoadService : IAvailableLoadService
{
    private readonly IAvailableLoadRepository repo;
    private readonly IMapper mapper;

    public AvailableLoadService(IAvailableLoadRepository repo, IMapper mapper)
    {
        this.repo = repo;
        this.mapper = mapper;
    }

    public async Task<PagedResult<LoadResponseDto>> GetFilteredAsync(LoadFilterDto filter, CancellationToken ct = default)
    {
        var paged = await repo.GetFilteredAsync(filter, ct);
        var dtos = paged.Items.Select(i => mapper.Map<LoadResponseDto>(i)).ToList();
        return new PagedResult<LoadResponseDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }

    public async Task<LoadResponseDto> CreateAsync(CreateLoadDto dto, Guid postedByUserId, CancellationToken ct = default)
    {
        var entity = mapper.Map<Entities.AvailableLoad>(dto);
        entity.PostedByUserId = postedByUserId;
        entity.MembershipTier ??= "Free";
        entity.Status = "Active";
        entity.ExpiresAt = DateTime.UtcNow.AddDays(30);
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return mapper.Map<LoadResponseDto>(entity);
    }
}
