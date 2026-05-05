using Products.API.Models;

namespace Products.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _repo.GetByIdAsync(id);
    }
}