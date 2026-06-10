using CommerceOps.API.Data;
using CommerceOps.API.Interfaces;
using CommerceOps.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CommerceOps.API.Repositories;

public class IntegrationHealthRepository(AppDbContext db) : IIntegrationHealthRepository
{
    public async Task<IReadOnlyList<IntegrationHealth>> GetAllAsync(CancellationToken ct = default) =>
        await db.IntegrationHealths.OrderBy(h => h.Channel).ToListAsync(ct);

    public Task<IntegrationHealth?> GetByChannelAsync(string channel, CancellationToken ct = default) =>
        db.IntegrationHealths.FirstOrDefaultAsync(h => h.Channel == channel, ct);

    public async Task UpsertAsync(IntegrationHealth health, CancellationToken ct = default)
    {
        var existing = await GetByChannelAsync(health.Channel, ct);
        if (existing is null)
        {
            await db.IntegrationHealths.AddAsync(health, ct);
        }
        else
        {
            existing.Status = health.Status;
            existing.LastChecked = health.LastChecked;
            existing.FailureCount = health.FailureCount;
            existing.ResponseTime = health.ResponseTime;
            db.IntegrationHealths.Update(existing);
        }
    }

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
