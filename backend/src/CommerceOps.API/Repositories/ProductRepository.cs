using CommerceOps.API.Data;
using CommerceOps.API.Interfaces;
using CommerceOps.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CommerceOps.API.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default) =>
        await db.Products.OrderBy(p => p.Name).ToListAsync(ct);

    public async Task<IReadOnlyList<Product>> GetLowStockAsync(int threshold, CancellationToken ct = default) =>
        await db.Products.Where(p => p.Stock <= threshold).OrderBy(p => p.Stock).ToListAsync(ct);

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Product?> GetByExternalIdAsync(string externalId, string channel, CancellationToken ct = default) =>
        db.Products.FirstOrDefaultAsync(p => p.ExternalProductId == externalId && p.Channel == channel, ct);

    public async Task AddAsync(Product product, CancellationToken ct = default) =>
        await db.Products.AddAsync(product, ct);

    public async Task AddRangeAsync(IEnumerable<Product> products, CancellationToken ct = default) =>
        await db.Products.AddRangeAsync(products, ct);

    public Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        db.Products.Update(product);
        return Task.CompletedTask;
    }

    public Task<int> CountLowStockAsync(int threshold, CancellationToken ct = default) =>
        db.Products.CountAsync(p => p.Stock <= threshold, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
