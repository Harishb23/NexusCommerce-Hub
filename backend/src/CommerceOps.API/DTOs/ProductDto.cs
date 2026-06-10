namespace CommerceOps.API.DTOs;

public record ProductDto
{
    public int Id { get; init; }
    public string ExternalProductId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public string Channel { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsLowStock { get; init; }
}

public record InventoryStatsDto(
    int TotalProducts,
    int LowStockCount,
    int OutOfStockCount,
    decimal TotalInventoryValue
);
