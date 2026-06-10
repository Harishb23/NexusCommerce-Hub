using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;

namespace CommerceOps.API.Services;

public class AnalyticsService(
    IOrderRepository orderRepo,
    IProductRepository productRepo,
    IConfiguration config) : IAnalyticsService
{
    private int LowStockThreshold => config.GetValue<int>("Inventory:LowStockThreshold", 10);

    public async Task<OrderAnalyticsDto> GetOrderAnalyticsAsync(CancellationToken ct = default)
    {
        var orders = await orderRepo.GetAllAsync(ct);

        var byChannel = orders
            .GroupBy(o => o.Channel)
            .Select(g => new ChannelMetricDto(g.Key, g.Count()))
            .ToList();

        var byStatus = orders
            .GroupBy(o => o.Status)
            .Select(g => new StatusMetricDto(g.Key, g.Count()))
            .ToList();

        return new OrderAnalyticsDto(byChannel, byStatus);
    }

    public async Task<RevenueAnalyticsDto> GetRevenueAnalyticsAsync(CancellationToken ct = default)
    {
        var orders = await orderRepo.GetAllAsync(ct);

        var byChannel = orders
            .GroupBy(o => o.Channel)
            .Select(g => new ChannelMetricDto(g.Key, g.Sum(o => o.TotalAmount)))
            .ToList();

        return new RevenueAnalyticsDto(byChannel, orders.Sum(o => o.TotalAmount));
    }

    public async Task<InventoryHealthDto> GetInventoryHealthAsync(CancellationToken ct = default)
    {
        var products = await productRepo.GetAllAsync(ct);
        var total = products.Count;
        var outOfStock = products.Count(p => p.Stock == 0);
        var lowStock = products.Count(p => p.Stock > 0 && p.Stock <= LowStockThreshold);
        var healthy = total - outOfStock - lowStock;

        return new InventoryHealthDto(total, Math.Max(0, healthy), lowStock, outOfStock);
    }
}
