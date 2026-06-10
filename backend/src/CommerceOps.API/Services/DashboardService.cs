using AutoMapper;
using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;
using CommerceOps.API.Models;

namespace CommerceOps.API.Services;

public class DashboardService(
    IOrderRepository orderRepo,
    IProductRepository productRepo,
    ISyncLogRepository syncLogRepo,
    IIntegrationHealthRepository healthRepo,
    IMapper mapper,
    IConfiguration config) : IDashboardService
{
    private int LowStockThreshold => config.GetValue<int>("Inventory:LowStockThreshold", 10);

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken ct = default)
    {
        var totalOrders = (await orderRepo.GetAllAsync(ct)).Count;
        var pendingOrders = await orderRepo.CountByStatusAsync(OrderStatus.Pending, ct);
        var completedOrders = await orderRepo.CountByStatusAsync(OrderStatus.Delivered, ct);
        var totalRevenue = await orderRepo.GetTotalRevenueAsync(ct);
        var lowInventoryAlerts = await productRepo.CountLowStockAsync(LowStockThreshold, ct);
        var failedSyncs = await syncLogRepo.CountFailedAsync(ct);
        var integrations = await healthRepo.GetAllAsync(ct);

        return new DashboardStatsDto(
            totalOrders,
            pendingOrders,
            completedOrders,
            totalRevenue,
            lowInventoryAlerts,
            failedSyncs,
            mapper.Map<IReadOnlyList<IntegrationHealthDto>>(integrations)
        );
    }
}
