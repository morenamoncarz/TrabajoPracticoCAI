using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Services;

namespace Orders.API.Controllers;

[ApiController]
[Route("api/orders")]
[Tags("Orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;

    public OrdersController(OrderService service)
    {
        _service = service;
    }

    /// <summary>
    /// Devuelve todas las órdenes.
    /// Permite filtrar por usuario.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<List<OrderResponse>> GetAll(
        [FromQuery] Guid? usuarioId)
    {
        var orders = _service.GetAll(usuarioId);

        return Ok(orders);
    }

    /// <summary>
    /// Busca una orden por id.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<OrderResponse> GetById(Guid id)
    {
        var order = _service.GetById(id);

        return Ok(order);
    }

    /// <summary>
    /// Crea una nueva orden.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderResponse>> Create(
        CreateOrderRequest request)
    {
        var order = await _service.Create(request);

        return Created($"/api/orders/{order.Id}", order);
    }

    /// <summary>
    /// Actualiza el estado de una orden.
    /// </summary>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(UpdateOrderStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UpdateOrderStatusResponse>> UpdateStatus(
        Guid id,
        UpdateOrderStatusRequest request)
    {
        var result = await _service.UpdateStatus(id, request);

        return Ok(result);
    }
}