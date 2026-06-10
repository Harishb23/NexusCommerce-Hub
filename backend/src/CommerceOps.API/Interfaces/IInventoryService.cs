using CommerceOps.API.DTOs;

namespace CommerceOps.API.Interfaces;

public interface IInventoryService
{
    Task<IReadOnlyList<ProductDto>> GetAllProductsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ProductDto>> GetLowStockProductsAsync(CancellationToken ct = default);
    Task<InventoryStatsDto> GetInventoryStatsAsync(CancellationToken ct = default);
}
