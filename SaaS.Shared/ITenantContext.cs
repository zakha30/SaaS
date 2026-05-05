namespace SaaS.Shared;

public interface ITenantContext
{
    /// <summary>Empty Guid when the request has not been scoped to a tenant yet.</summary>
    Guid CurrentTenantId { get; }

    /// <summary>Strict: throws if no tenant is resolved (use after slug or JWT resolution).</summary>
    Guid TenantId { get; }

    bool IsResolved { get; }

    void SetTenant(Guid tenantId);
}