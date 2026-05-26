using Dapper;
using Microsoft.Data.Sqlite;
using Notifications.API.Models;

namespace Notifications.API.Repositories;

// es el encargado de hablar con la base de datos

public class NotificationRepository(IConfiguration config) : INotificationRepository
{
    private readonly string _connectionString =
        config.GetConnectionString("DefaultConnection")
        ?? "Data Source=notifications.db";

    private SqliteConnection CreateConnection()
        => new(_connectionString);

    public async Task AddAsync(Notification notification)
    {
        using var connection = CreateConnection();

        await connection.ExecuteAsync("""
            INSERT INTO notifications
            (id, usuario_id, mensaje, tipo, estado, fecha_envio)
            VALUES
            (@Id, @UsuarioId, @Mensaje, @Tipo, @Estado, @FechaEnvio)
        """, new
        {
            Id = notification.Id.ToString(),
            UsuarioId = notification.UsuarioId.ToString(),
            notification.Mensaje,
            notification.Tipo,
            notification.Estado,
            FechaEnvio = notification.FechaEnvio.ToString("O")
        });
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId)
    {
        using var connection = CreateConnection();

        return await connection.QueryAsync<Notification>("""
            SELECT
                id          AS Id,
                usuario_id  AS UsuarioId,
                mensaje     AS Mensaje,
                tipo        AS Tipo,
                estado      AS Estado,
                fecha_envio AS FechaEnvio
            FROM notifications
            WHERE usuario_id = @UsuarioId
            ORDER BY fecha_envio DESC
        """, new
        {
            UsuarioId = userId.ToString()
        });
    }
}