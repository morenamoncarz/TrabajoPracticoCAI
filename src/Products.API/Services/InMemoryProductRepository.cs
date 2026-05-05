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

    public Task<Product?> GetByIdAsync(Guid id)
    {
        _datos.TryGetValue(id, out var producto);
        return Task.FromResult(producto);
    }

    public Task AddAsync(Product producto)
    {
        _datos[producto.Id] = producto;
        return Task.CompletedTask;
    }

    public Task<bool> UpdateAsync(Product producto)
    {
        if (!_datos.ContainsKey(producto.Id))
        {
            return Task.FromResult(false);
        }
        _datos[producto.Id] = producto;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(_datos.Remove(id));
    }
}