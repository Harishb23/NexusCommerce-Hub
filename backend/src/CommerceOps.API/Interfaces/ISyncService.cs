using CommerceOps.API.DTOs;

namespace CommerceOps.API.Interfaces;

public interface ISyncService
{
    Task<SyncResultDto> SyncAllChannelsAsync(CancellationToken ct = default);
    Task<SyncResultDto> SyncChannelAsync(string channel, CancellationToken ct = default);
    Task<IReadOnlyList<SyncLogDto>> GetSyncLogsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SyncLogDto>> GetFailedSyncsAsync(CancellationToken ct = default);
    Task<SyncResultDto> RetrySyncAsync(int syncLogId, CancellationToken ct = default);
}
