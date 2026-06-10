using System.Text.Json;
using System.Text.Json.Serialization;
using CommerceOps.API.Interfaces;
using CommerceOps.API.Models;

namespace CommerceOps.API.Services;

public class NoonChannelService(HttpClient httpClient, ILogger<NoonChannelService> logger) : INoonChannelService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<(IReadOnlyList<Product> Products, string Error)> FetchProductsAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await httpClient.GetAsync("products", ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var fakeStoreProducts = JsonSerializer.Deserialize<List<FakeStoreProduct>>(json, JsonOptions);

            if (fakeStoreProducts is null)
                return ([], "Received empty response from FakeStore");

            var products = fakeStoreProducts.Select(p => new Product
            {
                ExternalProductId = $"NOON-{p.Id}",
                Name = p.Title,
                Price = (decimal)p.Price,
                Stock = Random.Shared.Next(0, 50),
                Channel = "Noon",
                CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 60))
            }).ToList();

            logger.LogInformation("Fetched {Count} products from Noon (FakeStore)", products.Count);
            return (products, string.Empty);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            var msg = "Noon channel timed out";
            logger.LogError(ex, msg);
            return ([], msg);
        }
        catch (HttpRequestException ex)
        {
            var msg = $"Noon channel HTTP error: {ex.Message}";
            logger.LogError(ex, msg);
            return ([], msg);
        }
        catch (Exception ex)
        {
            var msg = $"Noon channel unexpected error: {ex.Message}";
            logger.LogError(ex, msg);
            return ([], msg);
        }
    }

    private record FakeStoreProduct(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("price")] double Price,
        [property: JsonPropertyName("category")] string Category
    );
}
