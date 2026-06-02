# TP Microservicios E-Commerce

Trabajo practico de Construccion de Aplicaciones Informaticas - .NET 8

## Microservicios

- `src/Products.API` - catalogo de productos (SQLite + Dapper)
- `src/Cart.API` - carrito de compras, consume Products.API
- `src/Users.API` - pendiente
- `src/Orders.API` - pendiente
- `src/Notifications.API` - pendiente

## Como ejecutar

Hace falta tener instalado el SDK de .NET 8.

Levantar los dos servicios en terminales separadas:

```
dotnet run --project src/Products.API
dotnet run --project src/Cart.API
```

Las bases sqlite (`products.db`, `cart.db`) se crean automaticamente al arrancar.

## URLs por servicio

| Servicio | Swagger | Health JSON | Health UI |
|---|---|---|---|
| Products.API | http://localhost:5290/swagger | http://localhost:5290/health | http://localhost:5290/health-ui |
| Cart.API | http://localhost:5277/swagger | http://localhost:5277/health | http://localhost:5277/health-ui |

## Variables de entorno

- `ASPNETCORE_ENVIRONMENT` - `Development` muestra stack trace en errores 500. `Production` lo oculta.
- `ProductsApi__BaseUrl` (Cart.API) - url base de Products. Default en development: `http://localhost:5290`.

## Logs

Cada servicio escribe en `logs/<servicio>-<fecha>.log` en formato JSON compact (Serilog). Cada request lleva un `X-Correlation-Id` que se propaga en las llamadas HTTP entre servicios.
