using System.Text.Json;
using System.Text.Json.Serialization;
using CommerceOps.API.Interfaces;
using CommerceOps.API.Models;

namespace CommerceOps.API.Services;

public class AmazonChannelService(HttpClient httpClient, ILogger<AmazonChannelService> logger) : IAmazonChannelService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<(IReadOnlyList<Order> Orders, string Error)> FetchOrdersAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await httpClient.GetAsync("carts?limit=30", ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<DummyJsonCartsResponse>(json, JsonOptions);

            if (result?.Carts is null)
                return ([], "Received empty response from DummyJSON");

            var orders = result.Carts.Select(cart => new Order
            {
                ExternalOrderId = $"AMZ-{cart.Id}",
                Channel = "Amazon",
                CustomerName = $"Customer {cart.UserId}",
                TotalAmount = (decimal)cart.Total,
                Status = AssignStatus(cart.Id),
                CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 30))
            }).ToList();

            logger.LogInformation("Fetched {Count} orders from Amazon (DummyJSON)", orders.Count);
            return (orders, string.Empty);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            var msg = "Amazon channel timed out";
            logger.LogError(ex, msg);
            return ([], msg);
        }
        catch (HttpRequestException ex)
        {
            var msg = $"Amazon channel HTTP error: {ex.Message}";
            logger.LogError(ex, msg);
            return ([], msg);
        }
        catch (Exception ex)
        {
            var msg = $"Amazon channel unexpected error: {ex.Message}";
            logger.LogError(ex, msg);
            return ([], msg);
        }
    }

    private static string AssignStatus(int id) => (id % 4) switch
    {
        0 => OrderStatus.Pending,
        1 => OrderStatus.Processing,
        2 => OrderStatus.Delivered,
        _ => OrderStatus.Cancelled
    };

    private record DummyJsonCartsResponse(
        [property: JsonPropertyName("carts")] List<DummyJsonCart>? Carts,
        [property: JsonPropertyName("total")] int Total
    );

    private record DummyJsonCart(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("userId")] int UserId,
        [property: JsonPropertyName("total")] double Total,
        [property: JsonPropertyName("totalProducts")] int TotalProducts
    );
}
