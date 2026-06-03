using System.ComponentModel.DataAnnotations;

namespace Cart.API.Models;

/// <summary>Body para POST /api/cart/{userId}/items.</summary>
public class AddCartItemRequest
{
    [Required]
    public Guid ProductoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Cantidad invalida.")]
    public int Cantidad { get; set; }
}
