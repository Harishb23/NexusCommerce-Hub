using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOps.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardStatsDto>> GetStats(CancellationToken ct) =>
        Ok(await dashboardService.GetDashboardStatsAsync(ct));
}
