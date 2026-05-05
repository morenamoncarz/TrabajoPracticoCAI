using Products.API.Models;

namespace Products.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(string? categoria = null, string? nombre = null)
    {
        return await _repo.GetAllAsync(categoria, nombre);
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<Product> CreateAsync(Product producto)
    {
        producto.Id = Guid.NewGuid();
        producto.FechaCreacion = DateTime.UtcNow;
        await _repo.AddAsync(producto);
        return producto;
    }

    public async Task<Product?> UpdateAsync(Guid id, Product producto)
    {
        var existente = await _repo.GetByIdAsync(id);
        if (existente == null)
        {
            return null;
        }
        producto.Id = id;
        producto.FechaCreacion = existente.FechaCreacion;
        await _repo.UpdateAsync(producto);
        return producto;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repo.DeleteAsync(id);
    }
}