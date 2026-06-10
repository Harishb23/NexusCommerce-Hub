using CommerceOps.API.DTOs;

namespace CommerceOps.API.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken ct = default);
}
