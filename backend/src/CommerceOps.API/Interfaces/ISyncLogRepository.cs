using CommerceOps.API.Models;

namespace CommerceOps.API.Interfaces;

public interface ISyncLogRepository
{
    Task<IReadOnlyList<SyncLog>> GetAllAsync(int limit = 100, CancellationToken ct = default);
    Task<IReadOnlyList<SyncLog>> GetFailedAsync(CancellationToken ct = default);
    Task<SyncLog?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(SyncLog log, CancellationToken ct = default);
    Task<int> CountFailedAsync(CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
