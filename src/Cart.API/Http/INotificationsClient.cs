namespace Cart.API.Http;

public interface INotificationsClient
{
    Task Notificar(Guid usuarioId, string mensaje, CancellationToken ct = default);
}
