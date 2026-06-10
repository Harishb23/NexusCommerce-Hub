using CommerceOps.API.Models;

namespace CommerceOps.API.Interfaces;

public interface INoonChannelService
{
    Task<(IReadOnlyList<Product> Products, string Error)> FetchProductsAsync(CancellationToken ct = default);
}
