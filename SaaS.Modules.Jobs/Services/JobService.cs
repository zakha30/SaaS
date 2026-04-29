using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using SaaS.Modules.Jobs.DTOs;
using SaaS.Modules.Jobs.Repositories;
using SaaS.Shared;

namespace SaaS.Modules.Jobs.Services;

public sealed class JobService : IJobService
{
    private readonly IJobRepository repo;
    private readonly IMapper mapper;

    public JobService(IJobRepository repo, IMapper mapper)
    {
        this.repo = repo;
        this.mapper = mapper;
    }

    public async Task<PagedResult<JobResponseDto>> GetFilteredAsync(JobFilterDto filter, CancellationToken ct = default)
    {
        var paged = await repo.GetFilteredAsync(filter, ct);
        var dtos = paged.Items.Select(i => mapper.Map<JobResponseDto>(i)).ToList();
        return new PagedResult<JobResponseDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }

    public async Task<JobResponseDto> CreateAsync(CreateJobDto dto, CancellationToken ct = default)
    {
        var entity = mapper.Map<Entities.JobListing>(dto);
        entity.MembershipTier ??= "Free";
        entity.ExpiresAt = DateTime.UtcNow.AddDays(90);
        await repo.AddAsync(entity, ct);
        await repo.SaveChangesAsync(ct);
        return mapper.Map<JobResponseDto>(entity);
    }
}
