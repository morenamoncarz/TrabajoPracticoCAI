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

    private static async Task Ver()
    {
        var cart = await ApiClient.Get<CarritoDto>($"{ApiUrls.Cart}/api/cart/{Sesion.UsuarioActualId}");
        if (cart != null)
        {
            if (cart.Items.Count == 0) { Console.WriteLine("el carrito esta vacio"); return; }
            foreach (var i in cart.Items)
                Console.WriteLine($"- producto {i.ProductoId.ToString().Substring(0, 4)}.. x{i.Cantidad}");
        }
    }

    private static async Task Agregar()
    {
        Console.Write("id del producto: "); var productoId = Console.ReadLine();
        Console.Write("cantidad: "); int.TryParse(Console.ReadLine(), out var cantidad);

        var body = new { productoId, cantidad };
        var cart = await ApiClient.Post<CarritoDto>($"{ApiUrls.Cart}/api/cart/{Sesion.UsuarioActualId}/items", body);
        if (cart != null) Console.WriteLine("agregado al carrito");
    }

    private static async Task Cambiar()
    {
        Console.Write("id del producto: "); var productoId = Console.ReadLine();
        Console.Write("nueva cantidad: "); int.TryParse(Console.ReadLine(), out var cantidad);

        var body = new { cantidad };
        var cart = await ApiClient.Put<CarritoDto>($"{ApiUrls.Cart}/api/cart/{Sesion.UsuarioActualId}/items/{productoId}", body);
        if (cart != null) Console.WriteLine("cantidad actualizada");
    }

    private static async Task Sacar()
    {
        Console.Write("id del producto: "); var productoId = Console.ReadLine();
        var ok = await ApiClient.Delete($"{ApiUrls.Cart}/api/cart/{Sesion.UsuarioActualId}/items/{productoId}");
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
