using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SaaS.Modules.Loads.DTOs;
using SaaS.Modules.Loads.Entities;
using SaaS.Modules.Loads.Repositories;

namespace SaaS.Modules.Loads.Services;

public sealed class QuoteRequestService : IQuoteRequestService
{
    private readonly IQuoteRequestRepository requestRepo;
    private readonly IQuoteSubmissionRepository submissionRepo;
    private readonly IMapper mapper;

    public QuoteRequestService(IQuoteRequestRepository requestRepo, IQuoteSubmissionRepository submissionRepo, IMapper mapper)
    {
        this.requestRepo = requestRepo;
        this.submissionRepo = submissionRepo;
        this.mapper = mapper;
    }

    public async Task<QuoteRequestResponseDto> CreateAsync(CreateQuoteRequestDto dto, CancellationToken ct = default)
    {
        var entity = mapper.Map<QuoteRequest>(dto);
        entity.Status = "Open";
        await requestRepo.AddAsync(entity, ct);
        await requestRepo.SaveChangesAsync(ct);
        return mapper.Map<QuoteRequestResponseDto>(entity);
    }

    public async Task SubmitQuoteAsync(QuoteSubmissionDto dto, Guid submittedByUserId, CancellationToken ct = default)
    {
        var entity = mapper.Map<QuoteSubmission>(dto);
        entity.SubmittedByUserId = submittedByUserId;
        entity.Status = "Pending";
        await submissionRepo.AddAsync(entity, ct);
        await submissionRepo.SaveChangesAsync(ct);
    }
}
