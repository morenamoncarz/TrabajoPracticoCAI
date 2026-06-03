using Orders.API.DTOs;
using Orders.API.Exceptions;
using Orders.API.Models;

namespace Orders.API.Services;

public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly UsersApiClient _usersApiClient;
    private readonly ProductsApiClient _productsApiClient;

    // Transiciones de estado permitidas
    // clave = estado actual, valor = estados a los que puede pasar
    private static readonly Dictionary<string, string[]> TransicionesValidas = new()
    {
        ["Pendiente"] = new[] { "Confirmada", "Cancelada" },
        ["Confirmada"] = new[] { "Enviada", "Cancelada" },
        ["Enviada"] = new[] { "Entregada" },
        ["Entregada"] = Array.Empty<string>(),
        ["Cancelada"] = Array.Empty<string>()
    };

    public OrderService(
        IOrderRepository repository,
        UsersApiClient usersApiClient,
        ProductsApiClient productsApiClient)
    {
        _repository = repository;
        _usersApiClient = usersApiClient;
        _productsApiClient = productsApiClient;
    }

    // Devuelve todas las órdenes
    public List<OrderResponse> GetAll(Guid? usuarioId)
    {
        var orders = _repository.GetAll(usuarioId);

        return orders.Select(MapToResponse).ToList();
    }

    // Busca una orden por id
    public OrderResponse GetById(Guid id)
    {
        var order = _repository.GetById(id);

        if (order == null)
        {
            throw new NotFoundException(
                "ORD-001",
                $"No existe la orden '{id}'.");
        }

        return MapToResponse(order);
    }

    // Crea una nueva orden
    public async Task<OrderResponse> Create(
        CreateOrderRequest request)
    {
        // Validamos que exista el usuario en Users.API
        var userExists =
            await _usersApiClient.UserExists(request.UsuarioId);

        if (!userExists)
        {
            throw new NotFoundException(
                "ORD-003",
                $"El usuario '{request.UsuarioId}' no fue encontrado.");
        }

        var items = new List<OrderItem>();

        decimal total = 0;

        // Recorremos todos los productos de la orden
        foreach (var item in request.Items)
        {
            // Buscamos el producto real en Products.API
            var producto =
                await _productsApiClient.GetProductAsync(item.ProductoId);

            if (producto == null)
            {
                throw new NotFoundException(
                    "ORD-004",
                    $"El producto '{item.ProductoId}' no fue encontrado.");
            }

            // Validamos stock real
            if (producto.Stock < item.Cantidad)
            {
                throw new BusinessRuleException(
                    "ORD-005",
                    $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, solicitado: {item.Cantidad}.");
            }

            var orderItem = new OrderItem
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,

                // Usamos el precio real devuelto por Products.API
                PrecioUnitario = producto.Precio
            };

            items.Add(orderItem);

            total += producto.Precio * item.Cantidad;
        }

        // Creamos la orden
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UsuarioId = request.UsuarioId,
            Items = items,
            Total = total,
            Estado = "Pendiente",
            FechaCreacion = DateTime.UtcNow
        };

        _repository.Add(order);

        return MapToResponse(order);
    }

    // Actualiza el estado de una orden
    public UpdateOrderStatusResponse UpdateStatus(
        Guid id,
        UpdateOrderStatusRequest request)
    {
        var order = _repository.GetById(id);

        if (order == null)
        {
            throw new NotFoundException(
                "ORD-001",
                $"No existe la orden '{id}'.");
        }

        // Buscamos qué estados están permitidos desde el estado actual
        var estadosPermitidos =
            TransicionesValidas.GetValueOrDefault(
                order.Estado,
                Array.Empty<string>());

        // Si el nuevo estado no está permitido, devolvemos ORD-006
        if (!estadosPermitidos.Contains(request.Estado))
        {
            throw new BusinessRuleException(
                "ORD-006",
                $"Una orden en estado '{order.Estado}' no puede pasar a '{request.Estado}'.");
        }

        _repository.UpdateStatus(id, request.Estado);

        return new UpdateOrderStatusResponse
        {
            Id = id,
            Estado = request.Estado,
            FechaActualizacion = DateTime.UtcNow
        };
    }

    // Convierte Order a OrderResponse
    private OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            UsuarioId = order.UsuarioId,
            Total = order.Total,
            Estado = order.Estado,
            FechaCreacion = order.FechaCreacion,

            Items = order.Items.Select(i => new OrderItemResponse
            {
                ProductoId = i.ProductoId,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario
            }).ToList()
        };
    }
}