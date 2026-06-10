namespace CommerceOps.API.DTOs;

public record DashboardStatsDto(
    int TotalOrders,
    int PendingOrders,
    int CompletedOrders,
    decimal TotalRevenue,
    int LowInventoryAlerts,
    int FailedSyncs,
    IReadOnlyList<IntegrationHealthDto> IntegrationStatuses
);
