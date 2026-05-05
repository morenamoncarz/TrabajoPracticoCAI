using Products.API.Models;

namespace Products.API.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
}