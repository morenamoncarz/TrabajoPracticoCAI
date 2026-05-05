using Products.API.Models;

namespace Products.API.Services;

public class InMemoryProductRepository : IProductRepository
{
    private readonly Dictionary<Guid, Product> _datos = new();

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        IEnumerable<Product> productos = _datos.Values;
        return Task.FromResult(productos);
    }
}