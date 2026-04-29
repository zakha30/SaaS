using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using SaaS.Modules.Classifieds.DTOs;
using SaaS.Modules.Classifieds.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Classifieds.Services;

public sealed class ClassifiedService : IClassifiedService
{
    private readonly IClassifiedRepository repo;
    private readonly IMapper mapper;

    public ClassifiedService(IClassifiedRepository repo, IMapper mapper)
    {
        this.repo = repo;
        this.mapper = mapper;
    }

    public async Task<PagedResult<ClassifiedResponseDto>> GetFilteredAsync(ClassifiedFilterDto filter, CancellationToken ct = default)
    {
        var paged = await repo.GetFilteredAsync(filter, ct);
        var dtos = paged.Items.Select(i => mapper.Map<ClassifiedResponseDto>(i)).ToList();
        return new PagedResult<ClassifiedResponseDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }

    public async Task<ClassifiedResponseDto> CreateAsync(CreateClassifiedDto dto, CancellationToken ct = default)
    {
        var entity = mapper.Map<Entities.ClassifiedItem>(dto);
        entity.MembershipTier ??= "Free";
        entity.ExpiresAt = DateTime.UtcNow.AddDays(90);
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return mapper.Map<ClassifiedResponseDto>(entity);
    }
}
