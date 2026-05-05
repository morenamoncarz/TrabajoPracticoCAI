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
        if (producto == null)
        {
            return NotFound(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                title = "Not Found",
                status = 404,
                detail = "El recurso solicitado no fue encontrado.",
                instance = HttpContext.Request.Path.Value,
                errorCode = "PRD-001",
                errorMessage = "Producto no encontrado."
            });
        }
        return Ok(producto);
    }

    /// <summary>Crea un nuevo producto.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product producto)
    {
        if (await _service.ExisteAsync(producto.Nombre, producto.Categoria))
        {
            return Conflict(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.9",
                title = "Conflict",
                status = 409,
                detail = "Ya existe un recurso con esos datos.",
                instance = HttpContext.Request.Path.Value,
                errorCode = "PRD-003",
                errorMessage = $"Ya existe un producto con ese nombre en la categoria '{producto.Categoria}'."
            });
        }
        var creado = await _service.CreateAsync(producto);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    /// <summary>Actualiza un producto existente.</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Product producto)
    {
        var actualizado = await _service.UpdateAsync(id, producto);
        if (actualizado == null)
        {
            return NotFound(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                title = "Not Found",
                status = 404,
                detail = "El recurso solicitado no fue encontrado.",
                instance = HttpContext.Request.Path.Value,
                errorCode = "PRD-001",
                errorMessage = "Producto no encontrado."
            });
        }
        return Ok(actualizado);
    }

    /// <summary>Elimina un producto.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var borrado = await _service.DeleteAsync(id);
        if (!borrado)
        {
            return NotFound(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                title = "Not Found",
                status = 404,
                detail = "El recurso solicitado no fue encontrado.",
                instance = HttpContext.Request.Path.Value,
                errorCode = "PRD-001",
                errorMessage = "Producto no encontrado."
            });
        }
        return NoContent();
    }
}