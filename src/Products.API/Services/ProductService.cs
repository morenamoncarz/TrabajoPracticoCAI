using Products.API.Exceptions;
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

    public async Task<Product> GetByIdAsync(Guid id)
    {
        var producto = await _repo.GetByIdAsync(id);
        if (producto == null)
        {
            throw new NotFoundException("PRD-001", "Producto no encontrado.");
        }
        return producto;
    }

    public async Task<Product> CreateAsync(Product producto)
    {
        if (await _repo.ExisteAsync(producto.Nombre, producto.Categoria))
        {
            throw new BusinessRuleException("PRD-003", $"Ya existe un producto con ese nombre en la categoria '{producto.Categoria}'.");
        }
        producto.Id = Guid.NewGuid();
        producto.FechaCreacion = DateTime.UtcNow;
        await _repo.AddAsync(producto);
        return producto;
    }

    public async Task<Product> UpdateAsync(Guid id, Product producto)
    {
        var existente = await _repo.GetByIdAsync(id);
        if (existente == null)
        {
            throw new NotFoundException("PRD-001", "Producto no encontrado.");
        }
        producto.Id = id;
        producto.FechaCreacion = existente.FechaCreacion;
        await _repo.UpdateAsync(producto);
        return producto;
    }

    public async Task DeleteAsync(Guid id)
    {
        var borrado = await _repo.DeleteAsync(id);
        if (!borrado)
        {
            throw new NotFoundException("PRD-001", "Producto no encontrado.");
        }
    }
}