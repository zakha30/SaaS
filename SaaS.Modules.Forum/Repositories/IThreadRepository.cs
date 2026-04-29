using System.Threading;
using System.Threading.Tasks;
using SaaS.Modules.Forum.Entities;
using SaaS.Shared;

namespace SaaS.Modules.Forum.Repositories;

public interface IThreadRepository
{
    Task<PagedResult<ForumThread>> GetFilteredAsync(DTOs.ThreadFilterDto filter, CancellationToken ct = default);
    Task AddAsync(ForumThread thread, CancellationToken ct = default);
    Task<ForumThread?> GetByIdAsync(System.Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
