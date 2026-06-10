using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOps.API.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    [HttpGet("orders")]
    public async Task<ActionResult<OrderAnalyticsDto>> GetOrderAnalytics(CancellationToken ct) =>
        Ok(await analyticsService.GetOrderAnalyticsAsync(ct));

    [HttpGet("revenue")]
    public async Task<ActionResult<RevenueAnalyticsDto>> GetRevenueAnalytics(CancellationToken ct) =>
        Ok(await analyticsService.GetRevenueAnalyticsAsync(ct));

    [HttpGet("inventory-health")]
    public async Task<ActionResult<InventoryHealthDto>> GetInventoryHealth(CancellationToken ct) =>
        Ok(await analyticsService.GetInventoryHealthAsync(ct));
}
