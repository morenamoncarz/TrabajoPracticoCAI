using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Cart.API.Http;

public class ProductsClient : IProductsClient
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;

    public ProductsClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid productoId, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"/api/products/{productoId}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductDto>(JsonOpts, ct);
    }
}
