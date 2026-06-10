using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOps.API.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetInventory(CancellationToken ct) =>
        Ok(await inventoryService.GetAllProductsAsync(ct));

    [HttpGet("low-stock")]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetLowStock(CancellationToken ct) =>
        Ok(await inventoryService.GetLowStockProductsAsync(ct));

    [HttpGet("stats")]
    public async Task<ActionResult<InventoryStatsDto>> GetStats(CancellationToken ct) =>
        Ok(await inventoryService.GetInventoryStatsAsync(ct));
}
