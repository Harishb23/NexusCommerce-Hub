using AutoMapper;
using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;

namespace CommerceOps.API.Services;

public class InventoryService(
    IProductRepository productRepo,
    IMapper mapper,
    IConfiguration config) : IInventoryService
{
    private int LowStockThreshold => config.GetValue<int>("Inventory:LowStockThreshold", 10);

    public async Task<IReadOnlyList<ProductDto>> GetAllProductsAsync(CancellationToken ct = default)
    {
        var products = await productRepo.GetAllAsync(ct);
        return products.Select(p => mapper.Map<ProductDto>(p) with
        {
            IsLowStock = p.Stock <= LowStockThreshold
        }).ToList();
    }

    public async Task<IReadOnlyList<ProductDto>> GetLowStockProductsAsync(CancellationToken ct = default)
    {
        var products = await productRepo.GetLowStockAsync(LowStockThreshold, ct);
        return products.Select(p => mapper.Map<ProductDto>(p) with { IsLowStock = true }).ToList();
    }

    public async Task<InventoryStatsDto> GetInventoryStatsAsync(CancellationToken ct = default)
    {
        var all = await productRepo.GetAllAsync(ct);
        var total = all.Count;
        var lowStock = all.Count(p => p.Stock > 0 && p.Stock <= LowStockThreshold);
        var outOfStock = all.Count(p => p.Stock == 0);
        var totalValue = all.Sum(p => p.Price * p.Stock);

        return new InventoryStatsDto(total, lowStock, outOfStock, totalValue);
    }
}
