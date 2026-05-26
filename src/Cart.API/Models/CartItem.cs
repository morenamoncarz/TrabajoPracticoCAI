namespace Cart.API.Models;

/// <summary>Producto dentro del carrito.</summary>
public class CartItem
{
    public Guid ProductoId { get; set; }

    public int Cantidad { get; set; }
}
