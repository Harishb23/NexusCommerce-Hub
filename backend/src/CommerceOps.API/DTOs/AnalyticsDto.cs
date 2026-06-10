namespace CommerceOps.API.DTOs;

public record ChannelMetricDto(string Channel, decimal Value);

public record OrderAnalyticsDto(
    IReadOnlyList<ChannelMetricDto> OrdersByChannel,
    IReadOnlyList<StatusMetricDto> OrdersByStatus
);

public record StatusMetricDto(string Status, int Count);

public record RevenueAnalyticsDto(
    IReadOnlyList<ChannelMetricDto> RevenueByChannel,
    decimal TotalRevenue
);

public record InventoryHealthDto(
    int TotalProducts,
    int HealthyStock,
    int LowStock,
    int OutOfStock
);
