using Cart.API.Models;
using Cart.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _service;

    public CartController(ICartService service)
    {
        _service = service;
    }

    /// <summary>Obtiene el carrito de un usuario.</summary>
    /// <remarks>
    /// Respuesta 200 OK:
    ///
    ///     {
    ///       "usuarioId": "a1b2c3d4-0000-0000-0000-111122223333",
    ///       "items": [
    ///         { "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "cantidad": 1 }
    ///       ],
    ///       "fechaActualizacion": "2024-03-10T10:45:00Z"
    ///     }
    ///
    /// Respuesta 404 (CRT-001) si el usuario no tiene carrito activo.
    /// </remarks>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(Models.Cart), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCart(Guid userId)
    {
        var cart = await _service.GetCartAsync(userId);
        return Ok(cart);
    }

    /// <summary>Lista los usuarios que tienen un producto en su carrito.</summary>
    /// <remarks>
    /// Lo usa Products.API para avisar cambios de precio o stock a quienes
    /// tienen el producto agregado. Devuelve 200 OK con una lista de usuarioId
    /// (vacia si nadie lo tiene).
    /// </remarks>
    [HttpGet("with-product/{productId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsuariosConProducto(Guid productId)
    {
        var usuarios = await _service.GetUsuariosConProductoAsync(productId);
        return Ok(usuarios);
    }

    /// <summary>Agrega un producto al carrito.</summary>
    /// <remarks>
    /// Ejemplo de request body:
    ///
    ///     {
    ///       "productoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "cantidad": 2
    ///     }
    ///
    /// Respuesta 200 OK con el carrito actualizado.
    /// 400 (CRT-004) cantidad invalida.
    /// 404 (CRT-002) producto no existe en Products API.
    /// 422 (CRT-003) stock insuficiente.
    ///
    /// Al agregar el item se le envia una notificacion al usuario a traves de
    /// Notifications.API (fire-and-forget: si Notifications esta caido el carrito
    /// se actualiza igual).
    /// </remarks>
    [HttpPost("{userId:guid}/items")]
    [ProducesResponseType(typeof(Models.Cart), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddItem(Guid userId, [FromBody] AddCartItemRequest request)
    {
        var cart = await _service.AddItemAsync(userId, request);
        return Ok(cart);
    }

    /// <summary>Actualiza la cantidad de un item del carrito.</summary>
    /// <remarks>
    /// Ejemplo de request body:
    ///
    ///     {
    ///       "cantidad": 4
    ///     }
    ///
    /// Respuesta 200 OK con el carrito actualizado.
    /// 400 (CRT-004) cantidad invalida.
    /// 404 (CRT-001) carrito o item no encontrado.
    /// 422 (CRT-003) stock insuficiente.
    /// </remarks>
    [HttpPut("{userId:guid}/items/{productId:guid}")]
    [ProducesResponseType(typeof(Models.Cart), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateItem(Guid userId, Guid productId, [FromBody] UpdateCartItemRequest request)
    {
        var cart = await _service.UpdateItemAsync(userId, productId, request);
        return Ok(cart);
    }

    /// <summary>Quita un producto del carrito.</summary>
    /// <remarks>Devuelve 204 sin body. 404 (CRT-001) si el carrito no existe.</remarks>
    [HttpDelete("{userId:guid}/items/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveItem(Guid userId, Guid productId)
    {
        await _service.RemoveItemAsync(userId, productId);
        return NoContent();
    }

    /// <summary>Vacia el carrito completo del usuario.</summary>
    /// <remarks>Devuelve 204 sin body. 404 (CRT-001) si el carrito no existe.</remarks>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ClearCart(Guid userId)
    {
        await _service.ClearCartAsync(userId);
        return NoContent();
    }
}
