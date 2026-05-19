using System.ComponentModel.DataAnnotations;

namespace Cart.API.Models;

/// <summary>Body para PUT /api/cart/{userId}/items/{productId}.</summary>
public class UpdateCartItemRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Cantidad invalida.")]
    public int Cantidad { get; set; }
}
