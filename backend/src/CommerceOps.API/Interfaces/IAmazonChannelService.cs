using CommerceOps.API.Models;

namespace CommerceOps.API.Interfaces;

public interface IAmazonChannelService
{
    Task<(IReadOnlyList<Order> Orders, string Error)> FetchOrdersAsync(CancellationToken ct = default);
}
