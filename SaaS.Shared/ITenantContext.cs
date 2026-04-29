namespace SaaS.Shared;

public interface ITenantContext
{
    Guid TenantId { get; }
    bool IsResolved { get; }
}