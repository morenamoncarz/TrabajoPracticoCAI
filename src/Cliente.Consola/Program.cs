using Cliente.Consola;
using Cliente.Consola.Menus;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== ECOMMERCE - MENU PRINCIPAL ===");

    var quien = Sesion.UsuarioActualId == null
        ? "(ninguno)"
        : $"{Sesion.NombreUsuario} ({Sesion.UsuarioActualId.ToString()!.Substring(0, 4)}..)";
    Console.WriteLine($"usuario actual: {quien}");

    Console.WriteLine();
    Console.WriteLine("1) usuarios");
    Console.WriteLine("2) productos");
    Console.WriteLine("3) carrito");
    Console.WriteLine("4) ordenes");
    Console.WriteLine("5) notificaciones");
    Console.WriteLine("0) salir");
    Console.Write("elegi una opcion: ");

    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1": await UsuariosMenu.Mostrar(); break;
        case "2": await ProductosMenu.Mostrar(); break;
        case "3": await CarritoMenu.Mostrar(); break;
        case "4": await OrdenesMenu.Mostrar(); break;
        case "5": Console.WriteLine("en construccion"); break;
        case "0": return;
        default: Console.WriteLine("opcion invalida"); break;
    }
}
