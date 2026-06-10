using AutoMapper;
using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;
using CommerceOps.API.Models;

namespace CommerceOps.API.Services;

public class OrderService(
    IOrderRepository orderRepo,
    IMapper mapper,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task<PagedResult<OrderDto>> GetOrdersAsync(OrderSearchParams searchParams, CancellationToken ct = default)
    {
        var result = await orderRepo.GetPagedAsync(searchParams, ct);
        var dtos = mapper.Map<IReadOnlyList<OrderDto>>(result.Items);
        return new PagedResult<OrderDto>(dtos, result.Total, result.Page, result.PageSize);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await orderRepo.GetByIdAsync(id, ct);
        return order is null ? null : mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(int id, string status, CancellationToken ct = default)
    {
        if (!OrderStatus.IsValid(status))
            throw new ArgumentException($"Invalid status '{status}'. Valid values: {string.Join(", ", OrderStatus.All)}");

        var order = await orderRepo.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Order {id} not found");

        order.Status = status;
        await orderRepo.UpdateAsync(order, ct);
        await orderRepo.SaveChangesAsync(ct);

        logger.LogInformation("Order {Id} status updated to {Status}", id, status);
        return mapper.Map<OrderDto>(order);
    }
}
