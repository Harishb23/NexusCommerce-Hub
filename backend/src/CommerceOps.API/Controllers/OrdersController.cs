using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOps.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetOrders(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? channel,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var searchParams = new OrderSearchParams(search, status, channel, page, pageSize);
        var result = await orderService.GetOrdersAsync(searchParams, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id, CancellationToken ct)
    {
        var order = await orderService.GetOrderByIdAsync(id, ct);
        return order is null ? NotFound(new { message = $"Order {id} not found" }) : Ok(order);
    }

    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<OrderDto>> UpdateStatus(
        int id,
        [FromBody] UpdateOrderStatusDto dto,
        CancellationToken ct)
    {
        var updated = await orderService.UpdateOrderStatusAsync(id, dto.Status, ct);
        return Ok(updated);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<OrderDto>>> SearchOrders(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var searchParams = new OrderSearchParams(q, null, null, page, pageSize);
        var result = await orderService.GetOrdersAsync(searchParams, ct);
        return Ok(result);
    }
}
