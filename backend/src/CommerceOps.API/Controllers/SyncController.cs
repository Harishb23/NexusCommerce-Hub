using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceOps.API.Controllers;

[ApiController]
[Route("api/sync")]
public class SyncController(ISyncService syncService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SyncResultDto>> Sync(
        [FromBody] SyncRequestDto? request,
        CancellationToken ct)
    {
        var result = string.IsNullOrWhiteSpace(request?.Channel)
            ? await syncService.SyncAllChannelsAsync(ct)
            : await syncService.SyncChannelAsync(request.Channel, ct);

        return result.Success ? Ok(result) : StatusCode(207, result);
    }

    [HttpGet("logs")]
    public async Task<ActionResult<IReadOnlyList<SyncLogDto>>> GetLogs(CancellationToken ct) =>
        Ok(await syncService.GetSyncLogsAsync(ct));

    [HttpGet("failed")]
    public async Task<ActionResult<IReadOnlyList<SyncLogDto>>> GetFailed(CancellationToken ct) =>
        Ok(await syncService.GetFailedSyncsAsync(ct));

    [HttpPost("retry/{id:int}")]
    public async Task<ActionResult<SyncResultDto>> Retry(int id, CancellationToken ct)
    {
        var result = await syncService.RetrySyncAsync(id, ct);
        return result.Success ? Ok(result) : StatusCode(207, result);
    }
}
