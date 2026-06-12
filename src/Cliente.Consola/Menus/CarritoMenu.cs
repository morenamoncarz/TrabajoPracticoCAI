namespace Cliente.Consola.Menus;

public static class CarritoMenu
{
    public static async Task Mostrar()
    {
        if (Sesion.UsuarioActualId == null)
        {
            Console.WriteLine("primero logueate en el menu de usuarios");
            return;
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("--- CARRITO ---");
            Console.WriteLine("1) ver");
            Console.WriteLine("2) agregar producto");
            Console.WriteLine("3) cambiar cantidad");
            Console.WriteLine("4) sacar producto");
            Console.WriteLine("5) vaciar");
            Console.WriteLine("6) volver");
            Console.Write("opcion: ");

            switch (Console.ReadLine())
            {
                case "1": await Ver(); break;
                case "2": await Agregar(); break;
                case "3": await Cambiar(); break;
                case "4": await Sacar(); break;
                case "5": await Vaciar(); break;
                case "6": return;
                default: Console.WriteLine("opcion invalida"); break;
            }
        }
    }

    // traigo los nombres del catalogo para no mostrar uuids en el carrito
    private static async Task<Dictionary<Guid, string>> NombresDeProductos()
    {
        var productos = await ApiClient.Get<List<ProductoDto>>($"{ApiUrls.Products}/api/products");
        return productos?.ToDictionary(p => p.Id, p => p.Nombre) ?? new();
    }

    private static async Task Ver()
    {
        var cart = await ApiClient.Get<CarritoDto>($"{ApiUrls.Cart}/api/cart/{Sesion.UsuarioActualId}");
        if (cart == null) return;
        if (cart.Items.Count == 0) { Console.WriteLine("el carrito esta vacio"); return; }

        var nombres = await NombresDeProductos();
        foreach (var i in cart.Items)
            Console.WriteLine($"- {nombres.GetValueOrDefault(i.ProductoId, "producto desconocido")} x{i.Cantidad}");
    }

    private static async Task<ProductoDto?> ElegirProducto()
    {
        var productos = await ApiClient.Get<List<ProductoDto>>($"{ApiUrls.Products}/api/products");
        if (productos == null) return null;
        if (productos.Count == 0) { Console.WriteLine("no hay productos"); return null; }

        for (var i = 0; i < productos.Count; i++)
            Console.WriteLine($"{i + 1}) {productos[i].Nombre} ${productos[i].Precio} stock:{productos[i].Stock}");

        Console.Write("numero del producto: ");
        if (!int.TryParse(Console.ReadLine(), out var n) || n < 1 || n > productos.Count)
        {
            Console.WriteLine("numero invalido");
            return null;
        }
        return productos[n - 1];
    }

    private static async Task<CarritoItemDto?> ElegirItem()
    {
        var cart = await ApiClient.Get<CarritoDto>($"{ApiUrls.Cart}/api/cart/{Sesion.UsuarioActualId}");
        if (cart == null) return null;
        if (cart.Items.Count == 0) { Console.WriteLine("el carrito esta vacio"); return null; }

        var nombres = await NombresDeProductos();
        for (var i = 0; i < cart.Items.Count; i++)
            Console.WriteLine($"{i + 1}) {nombres.GetValueOrDefault(cart.Items[i].ProductoId, "producto desconocido")} x{cart.Items[i].Cantidad}");

        Console.Write("numero del item: ");
        if (!int.TryParse(Console.ReadLine(), out var n) || n < 1 || n > cart.Items.Count)
        {
            Console.WriteLine("numero invalido");
            return null;
        }
        return cart.Items[n - 1];
    }

    private static async Task Agregar()
    {
        var producto = await ElegirProducto();
        if (producto == null) return;

        Console.Write("cantidad: "); int.TryParse(Console.ReadLine(), out var cantidad);

        var body = new { productoId = producto.Id, cantidad };
        var cart = await ApiClient.Post<CarritoDto>($"{ApiUrls.Cart}/api/cart/{Sesion.UsuarioActualId}/items", body);
        if (cart != null) Console.WriteLine("agregado al carrito");
    }

    private static async Task Cambiar()
    {
        var item = await ElegirItem();
        if (item == null) return;

        Console.Write("nueva cantidad: "); int.TryParse(Console.ReadLine(), out var cantidad);

        var body = new { cantidad };
        var cart = await ApiClient.Put<CarritoDto>($"{ApiUrls.Cart}/api/cart/{Sesion.UsuarioActualId}/items/{item.ProductoId}", body);
        if (cart != null) Console.WriteLine("cantidad actualizada");
    }

    private static async Task Sacar()
    {
        var item = await ElegirItem();
        if (item == null) return;

        var ok = await ApiClient.Delete($"{ApiUrls.Cart}/api/cart/{Sesion.UsuarioActualId}/items/{item.ProductoId}");
        if (ok) Console.WriteLine("producto sacado del carrito");
    }

    private static async Task Vaciar()
    {
        var ok = await ApiClient.Delete($"{ApiUrls.Cart}/api/cart/{Sesion.UsuarioActualId}");
        if (ok) Console.WriteLine("carrito vaciado");
    }
}

public class CarritoDto
{
    public Guid UsuarioId { get; set; }
    public List<CarritoItemDto> Items { get; set; } = new();
}

public class CarritoItemDto
{
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
}
