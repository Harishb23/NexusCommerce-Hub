using CommerceOps.API.Models;

namespace CommerceOps.API.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetLowStockAsync(int threshold, CancellationToken ct = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Product?> GetByExternalIdAsync(string externalId, string channel, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Product> products, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
    Task<int> CountLowStockAsync(int threshold, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
