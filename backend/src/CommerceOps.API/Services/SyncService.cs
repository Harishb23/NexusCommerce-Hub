using AutoMapper;
using CommerceOps.API.DTOs;
using CommerceOps.API.Interfaces;
using CommerceOps.API.Models;

namespace CommerceOps.API.Services;

public class SyncService(
    IAmazonChannelService amazonService,
    INoonChannelService noonService,
    IOrderRepository orderRepo,
    IProductRepository productRepo,
    ISyncLogRepository syncLogRepo,
    IIntegrationHealthRepository healthRepo,
    IMapper mapper,
    ILogger<SyncService> logger) : ISyncService
{
    public async Task<SyncResultDto> SyncAllChannelsAsync(CancellationToken ct = default)
    {
        var errors = new List<string>();
        int ordersSynced = 0, productsSynced = 0;

        var amazonResult = await SyncAmazonAsync(ct);
        ordersSynced += amazonResult.OrdersSynced;
        if (!string.IsNullOrEmpty(amazonResult.Errors.FirstOrDefault()))
            errors.AddRange(amazonResult.Errors);

        var noonResult = await SyncNoonAsync(ct);
        productsSynced += noonResult.ProductsSynced;
        if (!string.IsNullOrEmpty(noonResult.Errors.FirstOrDefault()))
            errors.AddRange(noonResult.Errors);

        return new SyncResultDto(
            errors.Count == 0,
            errors.Count == 0 ? "All channels synced successfully" : "Sync completed with errors",
            ordersSynced,
            productsSynced,
            errors
        );
    }

    public async Task<SyncResultDto> SyncChannelAsync(string channel, CancellationToken ct = default) =>
        channel.ToLower() switch
        {
            "amazon" => await SyncAmazonAsync(ct),
            "noon" => await SyncNoonAsync(ct),
            _ => new SyncResultDto(false, $"Unknown channel: {channel}", 0, 0, [$"Unknown channel: {channel}"])
        };

    public async Task<IReadOnlyList<SyncLogDto>> GetSyncLogsAsync(CancellationToken ct = default)
    {
        var logs = await syncLogRepo.GetAllAsync(100, ct);
        return mapper.Map<IReadOnlyList<SyncLogDto>>(logs);
    }

    public async Task<IReadOnlyList<SyncLogDto>> GetFailedSyncsAsync(CancellationToken ct = default)
    {
        var logs = await syncLogRepo.GetFailedAsync(ct);
        return mapper.Map<IReadOnlyList<SyncLogDto>>(logs);
    }

    public async Task<SyncResultDto> RetrySyncAsync(int syncLogId, CancellationToken ct = default)
    {
        var log = await syncLogRepo.GetByIdAsync(syncLogId, ct);
        if (log is null)
            return new SyncResultDto(false, $"Sync log {syncLogId} not found", 0, 0, [$"Sync log {syncLogId} not found"]);

        return await SyncChannelAsync(log.Channel, ct);
    }

    private async Task<SyncResultDto> SyncAmazonAsync(CancellationToken ct)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var (orders, error) = await amazonService.FetchOrdersAsync(ct);
        sw.Stop();

        if (!string.IsNullOrEmpty(error))
        {
            await RecordSyncLog("Amazon", SyncStatus.Failed, error, ct);
            await UpdateHealth("Amazon", HealthStatus.Failed, sw.Elapsed.TotalMilliseconds, ct);
            return new SyncResultDto(false, error, 0, 0, [error]);
        }

        int synced = 0;
        foreach (var order in orders)
        {
            var existing = await orderRepo.GetByExternalIdAsync(order.ExternalOrderId, order.Channel, ct);
            if (existing is null)
            {
                await orderRepo.AddAsync(order, ct);
                synced++;
            }
        }
        await orderRepo.SaveChangesAsync(ct);

        await RecordSyncLog("Amazon", SyncStatus.Success, $"Synced {synced} new orders", ct);
        await UpdateHealth("Amazon", HealthStatus.Healthy, sw.Elapsed.TotalMilliseconds, ct);

        logger.LogInformation("Amazon sync complete: {Synced} new orders", synced);
        return new SyncResultDto(true, $"Amazon: {synced} new orders synced", synced, 0, []);
    }

    private async Task<SyncResultDto> SyncNoonAsync(CancellationToken ct)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var (products, error) = await noonService.FetchProductsAsync(ct);
        sw.Stop();

        if (!string.IsNullOrEmpty(error))
        {
            await RecordSyncLog("Noon", SyncStatus.Failed, error, ct);
            await UpdateHealth("Noon", HealthStatus.Failed, sw.Elapsed.TotalMilliseconds, ct);
            return new SyncResultDto(false, error, 0, 0, [error]);
        }

        int synced = 0;
        foreach (var product in products)
        {
            var existing = await productRepo.GetByExternalIdAsync(product.ExternalProductId, product.Channel, ct);
            if (existing is null)
            {
                await productRepo.AddAsync(product, ct);
                synced++;
            }
            else
            {
                existing.Price = product.Price;
                existing.Name = product.Name;
                await productRepo.UpdateAsync(existing, ct);
            }
        }
        await productRepo.SaveChangesAsync(ct);

        await RecordSyncLog("Noon", SyncStatus.Success, $"Synced {synced} new products", ct);
        await UpdateHealth("Noon", HealthStatus.Healthy, sw.Elapsed.TotalMilliseconds, ct);

        logger.LogInformation("Noon sync complete: {Synced} new products", synced);
        return new SyncResultDto(true, $"Noon: {synced} new products synced", 0, synced, []);
    }

    private async Task RecordSyncLog(string channel, string status, string message, CancellationToken ct)
    {
        await syncLogRepo.AddAsync(new SyncLog
        {
            Channel = channel,
            Status = status,
            Message = message,
            CreatedAt = DateTime.UtcNow
        }, ct);
        await syncLogRepo.SaveChangesAsync(ct);
    }

    private async Task UpdateHealth(string channel, string status, double responseTime, CancellationToken ct)
    {
        var existing = await healthRepo.GetByChannelAsync(channel, ct);
        var failureCount = existing?.FailureCount ?? 0;
        if (status == HealthStatus.Failed) failureCount++;
        else failureCount = 0;

        var healthStatus = failureCount >= 5 ? HealthStatus.Failed
            : failureCount >= 2 ? HealthStatus.Warning
            : HealthStatus.Healthy;

        await healthRepo.UpsertAsync(new IntegrationHealth
        {
            Channel = channel,
            Status = healthStatus,
            LastChecked = DateTime.UtcNow,
            FailureCount = failureCount,
            ResponseTime = responseTime
        }, ct);
        await healthRepo.SaveChangesAsync(ct);
    }
}
