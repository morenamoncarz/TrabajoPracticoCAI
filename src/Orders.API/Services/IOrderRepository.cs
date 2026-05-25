using Orders.API.Models;

namespace Orders.API.Services;

public interface IOrderRepository
{
    // Lista órdenes, opcionalmente filtradas por usuario
    List<Order> GetAll(Guid? usuarioId);

    // Busca una orden por id
    Order? GetById(Guid id);

    // Guarda una nueva orden con sus items
    void Add(Order order);

    // Actualiza el estado de una orden existente
    void UpdateStatus(Guid id, string estado);
}