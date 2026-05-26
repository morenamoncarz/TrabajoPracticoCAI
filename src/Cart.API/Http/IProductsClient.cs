namespace Cart.API.Http;

public interface IProductsClient
{
    Task<ProductDto?> GetByIdAsync(Guid productoId, CancellationToken ct = default);
}
