using Products.API.Exceptions;
using Products.API.Models;

namespace Products.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly OrdersApiClient _ordersApiClient;

    public ProductService(IProductRepository repo, OrdersApiClient ordersApiClient)
    {
        _repo = repo;
        _ordersApiClient = ordersApiClient;
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
        var producto = await _repo.GetByIdAsync(id);
        if (producto == null)
        {
            throw new NotFoundException("PRD-001", "Producto no encontrado.");
        }

        // no lo dejo borrar si esta en alguna orden activa
        if (await _ordersApiClient.ProductoTieneOrdenesActivas(id))
        {
            throw new BusinessRuleException(
                "PRD-004",
                "El producto tiene ordenes activas y no puede eliminarse.");
        }

        await _repo.DeleteAsync(id);
    }
}