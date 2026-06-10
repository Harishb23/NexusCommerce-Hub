using CommerceOps.API.Data;
using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;
using CommerceOps.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CommerceOps.API.Repositories;

public class OrderRepository(AppDbContext db) : IOrderRepository
{
    public async Task<PagedResult<Order>> GetPagedAsync(OrderSearchParams p, CancellationToken ct = default)
    {
        var query = db.Orders.AsQueryable();

        if (!string.IsNullOrWhiteSpace(p.Search))
            query = query.Where(o =>
                o.CustomerName.ToLower().Contains(p.Search.ToLower()) ||
                o.ExternalOrderId.ToLower().Contains(p.Search.ToLower()));

        if (!string.IsNullOrWhiteSpace(p.Status))
            query = query.Where(o => o.Status == p.Status);

        if (!string.IsNullOrWhiteSpace(p.Channel))
            query = query.Where(o => o.Channel == p.Channel);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((p.Page - 1) * p.PageSize)
            .Take(p.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Order>(items, total, p.Page, p.PageSize);
    }

    public Task<Order?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);

    public Task<Order?> GetByExternalIdAsync(string externalId, string channel, CancellationToken ct = default) =>
        db.Orders.FirstOrDefaultAsync(o => o.ExternalOrderId == externalId && o.Channel == channel, ct);

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default) =>
        await db.Orders.OrderByDescending(o => o.CreatedAt).ToListAsync(ct);

    public async Task AddAsync(Order order, CancellationToken ct = default) =>
        await db.Orders.AddAsync(order, ct);

    public async Task AddRangeAsync(IEnumerable<Order> orders, CancellationToken ct = default) =>
        await db.Orders.AddRangeAsync(orders, ct);

    public Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        db.Orders.Update(order);
        return Task.CompletedTask;
    }

    public Task<int> CountByStatusAsync(string status, CancellationToken ct = default) =>
        db.Orders.CountAsync(o => o.Status == status, ct);

    public Task<decimal> GetTotalRevenueAsync(CancellationToken ct = default) =>
        db.Orders.SumAsync(o => o.TotalAmount, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
