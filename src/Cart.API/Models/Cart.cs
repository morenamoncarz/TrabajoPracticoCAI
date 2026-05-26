namespace Cart.API.Models;

/// <summary>Carrito de un usuario.</summary>
public class Cart
{
    public Guid UsuarioId { get; set; }

    /// <summary>Items en el carrito.</summary>
    public List<CartItem> Items { get; set; } = new();

    public DateTime FechaActualizacion { get; set; }
}
