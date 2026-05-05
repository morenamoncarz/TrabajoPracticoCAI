using Microsoft.AspNetCore.Mvc;
using Products.API.Models;
using Products.API.Services;

namespace Products.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    /// <summary>Lista todos los productos.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? categoria = null, [FromQuery] string? nombre = null)
    {
        var productos = await _service.GetAllAsync(categoria, nombre);
        return Ok(productos);
    }

    /// <summary>Obtiene un producto por su id.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var producto = await _service.GetByIdAsync(id);
        return Ok(producto);
    }

    /// <summary>Crea un nuevo producto.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product producto)
    {
        var creado = await _service.CreateAsync(producto);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    /// <summary>Actualiza un producto existente.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Product producto)
    {
        var actualizado = await _service.UpdateAsync(id, producto);
        return Ok(actualizado);
    }

    /// <summary>Elimina un producto.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}