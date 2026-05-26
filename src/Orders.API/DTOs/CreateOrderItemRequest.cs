using System.ComponentModel.DataAnnotations;

namespace Orders.API.DTOs;

public class CreateOrderItemRequest
{
    // Producto que se quiere comprar
    [Required]
    public Guid ProductoId { get; set; }

    // Cantidad solicitada del producto
    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }
}