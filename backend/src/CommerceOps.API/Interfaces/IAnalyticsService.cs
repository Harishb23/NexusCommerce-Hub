using CommerceOps.API.DTOs;

namespace CommerceOps.API.Interfaces;

public interface IAnalyticsService
{
    Task<OrderAnalyticsDto> GetOrderAnalyticsAsync(CancellationToken ct = default);
    Task<RevenueAnalyticsDto> GetRevenueAnalyticsAsync(CancellationToken ct = default);
    Task<InventoryHealthDto> GetInventoryHealthAsync(CancellationToken ct = default);
}
