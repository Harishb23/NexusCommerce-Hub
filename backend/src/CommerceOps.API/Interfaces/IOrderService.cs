using CommerceOps.API.DTOs;

namespace CommerceOps.API.Interfaces;

public interface IOrderService
{
    Task<PagedResult<OrderDto>> GetOrdersAsync(OrderSearchParams searchParams, CancellationToken ct = default);
    Task<OrderDto?> GetOrderByIdAsync(int id, CancellationToken ct = default);
    Task<OrderDto> UpdateOrderStatusAsync(int id, string status, CancellationToken ct = default);
}
