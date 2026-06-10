using CommerceOps.API.DTOs;
using CommerceOps.API.Models;

namespace CommerceOps.API.Interfaces;

public interface IOrderRepository
{
    Task<PagedResult<Order>> GetPagedAsync(OrderSearchParams searchParams, CancellationToken ct = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Order?> GetByExternalIdAsync(string externalId, string channel, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Order> orders, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
    Task<int> CountByStatusAsync(string status, CancellationToken ct = default);
    Task<decimal> GetTotalRevenueAsync(CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
