using Cliente.Consola;

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
        case "1": Console.WriteLine("en construccion"); break;
        case "2": Console.WriteLine("en construccion"); break;
        case "3": Console.WriteLine("en construccion"); break;
        case "4": Console.WriteLine("en construccion"); break;
        case "5": Console.WriteLine("en construccion"); break;
        case "0": return;
        default: Console.WriteLine("opcion invalida"); break;
    }
}
