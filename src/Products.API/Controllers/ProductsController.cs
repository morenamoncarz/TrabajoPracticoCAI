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
    /// <remarks>
    /// Acepta filtros opcionales por categoria y nombre (substring case-insensitive).
    ///
    /// Ejemplo: GET /api/products?categoria=Electronica&amp;nombre=Dell
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] string? categoria = null, [FromQuery] string? nombre = null)
    {
        var productos = await _service.GetAllAsync(categoria, nombre);
        return Ok(productos);
    }

    /// <summary>Obtiene un producto por su id.</summary>
    /// <remarks>
    /// Respuesta 200 OK:
    ///
    ///     {
    ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "nombre": "Notebook Dell XPS 15",
    ///       "descripcion": "Laptop 15 pulgadas, 32GB RAM",
    ///       "precio": 1500.00,
    ///       "stock": 10,
    ///       "categoria": "Electronica",
    ///       "fechaCreacion": "2024-01-15T10:30:00Z"
    ///     }
    ///
    /// Respuesta 404 Not Found (PRD-001):
    ///
    ///     {
    ///       "errorCode": "PRD-001",
    ///       "errorMessage": "Producto no encontrado."
    ///     }
    /// </remarks>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var producto = await _service.GetByIdAsync(id);
        return Ok(producto);
    }

    /// <summary>Crea un nuevo producto.</summary>
    /// <remarks>
    /// Ejemplo de request body:
    ///
    ///     {
    ///       "nombre": "Notebook Dell XPS 15",
    ///       "descripcion": "Laptop 15 pulgadas, 32GB RAM",
    ///       "precio": 1500.00,
    ///       "stock": 10,
    ///       "categoria": "Electronica"
    ///     }
    ///
    /// Respuesta 201 Created: el producto con id y fechaCreacion asignados.
    ///
    /// Respuesta 409 Conflict (PRD-003):
    ///
    ///     {
    ///       "errorCode": "PRD-003",
    ///       "errorMessage": "Ya existe un producto con ese nombre en la categoria 'Electronica'."
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] Product producto)
    {
        var creado = await _service.CreateAsync(producto);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    /// <summary>Actualiza un producto existente.</summary>
    /// <remarks>
    /// Ejemplo de request body (mismos campos que Create):
    ///
    ///     {
    ///       "nombre": "Notebook Dell XPS 15",
    ///       "descripcion": "Laptop 15 pulgadas, 64GB RAM",
    ///       "precio": 1750.00,
    ///       "stock": 8,
    ///       "categoria": "Electronica"
    ///     }
    ///
    /// Si cambia el precio, o si el stock queda en 0 o cruza el umbral bajo (por
    /// defecto 5), se notifica via Notifications.API a los usuarios que tienen el
    /// producto en su carrito (consultando Cart.API). Todo fire-and-forget: si Cart o
    /// Notifications estan caidos, la actualizacion del producto se completa igual.
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] Product producto)
    {
        var actualizado = await _service.UpdateAsync(id, producto);
        return Ok(actualizado);
    }

    /// <summary>Elimina un producto.</summary>
    /// <remarks>
    /// Devuelve 204 sin body si el producto existia. Devuelve 404 con PRD-001 si no existe.
    ///
    /// Respuesta 409 Conflict (PRD-004) si el producto tiene ordenes activas:
    ///
    ///     {
    ///       "errorCode": "PRD-004",
    ///       "errorMessage": "El producto tiene ordenes activas y no puede eliminarse."
    ///     }
    /// </remarks>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}