using CommerceOps.API.Models;

namespace CommerceOps.API.Interfaces;

public interface IIntegrationHealthRepository
{
    Task<IReadOnlyList<IntegrationHealth>> GetAllAsync(CancellationToken ct = default);
    Task<IntegrationHealth?> GetByChannelAsync(string channel, CancellationToken ct = default);
    Task UpsertAsync(IntegrationHealth health, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
