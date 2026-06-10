using CommerceOps.API.Data;
using CommerceOps.API.Interfaces;
using CommerceOps.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CommerceOps.API.Repositories;

public class SyncLogRepository(AppDbContext db) : ISyncLogRepository
{
    public async Task<IReadOnlyList<SyncLog>> GetAllAsync(int limit = 100, CancellationToken ct = default) =>
        await db.SyncLogs
            .OrderByDescending(s => s.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<SyncLog>> GetFailedAsync(CancellationToken ct = default) =>
        await db.SyncLogs
            .Where(s => s.Status == SyncStatus.Failed)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

    public Task<SyncLog?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.SyncLogs.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task AddAsync(SyncLog log, CancellationToken ct = default) =>
        await db.SyncLogs.AddAsync(log, ct);

    public Task<int> CountFailedAsync(CancellationToken ct = default) =>
        db.SyncLogs.CountAsync(s => s.Status == SyncStatus.Failed, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
