# Trabajo Práctico CAI - Sistema eCommerce

## Descripción general

Este proyecto implementa un sistema eCommerce basado en una arquitectura de microservicios utilizando .NET 8 Web API.

El sistema está dividido en cinco microservicios principales:

- Users.API
- Products.API
- Cart.API
- Orders.API
- Notifications.API

Cada microservicio tiene responsabilidades propias, su propia base de datos SQLite y se comunica con otros servicios mediante HTTP.

---

## Arquitectura

El sistema utiliza una arquitectura basada en microservicios y separación por capas.

Cada API se organiza principalmente en:

- Controllers: reciben las solicitudes HTTP.
- Services: contienen la lógica de negocio.
- Repositories: acceden a la base de datos.
- Models / DTOs: representan los datos del sistema.
- ExceptionHandlers: manejan errores personalizados.
- Middleware: agrega funcionalidades transversales como Correlation ID.

Cada microservicio posee su propia base de datos SQLite, siguiendo el patrón database per service.

---

## Microservicios

### Users.API

Responsable de la gestión de usuarios.

Endpoints principales:

- Registro de usuarios.
- Login.
- Consulta de usuario por ID.

Base de datos:

- users.db

Comunicación externa:

- Avisa a Notifications.API cuando se registra un usuario (notificación de bienvenida).

---

### Products.API

Responsable de la gestión de productos.

Endpoints principales:

- Crear producto.
- Listar productos.
- Obtener producto por ID.
- Actualizar producto.
- Eliminar producto.

Base de datos:

- products.db

Comunicación externa:

- Consulta Orders.API para validar órdenes activas antes de eliminar un producto.
- Consulta Cart.API para saber qué usuarios tienen el producto en su carrito.
- Avisa a Notifications.API cuando cambia el precio o el stock de un producto, para
  notificar a quienes lo tienen en el carrito.

---

### Cart.API

Responsable de la gestión del carrito de compras.

Endpoints principales:

- Agregar productos al carrito.
- Consultar carrito.
- Actualizar cantidades.
- Eliminar productos del carrito.
- Vaciar el carrito completo.
- Consultar qué usuarios tienen un producto en su carrito (usado por Products.API).

Base de datos:

- cart.db

Comunicación externa:

- Consulta Products.API para validar productos.
- Avisa a Notifications.API cuando se agrega un producto al carrito.

---

### Orders.API

Responsable de la gestión de órdenes de compra.

Endpoints principales:

- Crear orden.
- Listar órdenes.
- Obtener orden por ID.
- Actualizar estado de una orden.

Base de datos:

- orders.db

Comunicación externa:

- Consulta Users.API para validar usuarios.
- Consulta Products.API para validar productos y stock.

---

### Notifications.API

Responsable de la gestión y envío simulado de notificaciones.

Endpoints principales:

- Enviar notificación.
- Consultar notificaciones por usuario.

Base de datos:

- notifications.db

Comunicación externa:

- Consulta Users.API para validar que el usuario destinatario exista.

---

## Puertos utilizados

| Microservicio | Puerto |
|---|---:|
| Users.API | 5029 |
| Orders.API | 5074 |
| Notifications.API | 5026 |
| Cart.API | 5277 |
| Products.API | 5290 |

---

## Ejecución del proyecto

Para ejecutar cada microservicio, abrir una terminal en la raíz del repositorio y correr:

```bash
dotnet run --project src/Users.API
```

```bash
dotnet run --project src/Products.API
```

```bash
dotnet run --project src/Cart.API
```

```bash
dotnet run --project src/Orders.API
```

```bash
dotnet run --project src/Notifications.API
```

Cada API debe quedar ejecutándose en una terminal independiente.

---

## Swagger

Cada microservicio expone Swagger UI para probar sus endpoints desde el navegador.

URLs:

- Users.API: http://localhost:5029/swagger
- Products.API: http://localhost:5290/swagger
- Cart.API: http://localhost:5277/swagger
- Orders.API: http://localhost:5074/swagger
- Notifications.API: http://localhost:5026/swagger

---

## Health Checks

Cada microservicio cuenta con endpoints de health check para verificar su estado.

Ejemplos:

- http://localhost:5029/health
- http://localhost:5290/health
- http://localhost:5277/health
- http://localhost:5074/health
- http://localhost:5026/health

También se utiliza:

```text
/health
/health/ready
/health/live
```

`/health` da el estado general, `/health/ready` indica si el servicio esta listo
para recibir pedidos y `/health/live` indica si el servicio sigue vivo.

---

## Base de datos

El proyecto utiliza SQLite como motor de base de datos y Dapper para el acceso a datos.

Cada microservicio crea automáticamente su base de datos al iniciar mediante un `DatabaseInitializer`.

Las bases de datos no se suben al repositorio ya que se generan localmente.

Archivos generados:

- users.db
- products.db
- cart.db
- orders.db
- notifications.db

---

## Logging y trazabilidad

El sistema implementa logging mediante Serilog en distintos microservicios del proyecto.

Los microservicios se comunican entre sí mediante llamadas HTTP utilizando distintos puertos locales.

Header utilizado:
```text
X-Correlation-Id
```
Este identificador permite rastrear una misma solicitud entre distintos microservicios.

---

## Comunicación entre microservicios

| API origen | API destino | Motivo |
|---|---|---|
| Cart.API | Products.API | Validar productos |
| Cart.API | Notifications.API | Avisar cuando se agrega un producto al carrito |
| Orders.API | Users.API | Validar usuarios |
| Orders.API | Products.API | Validar precio y stock |
| Orders.API | Notifications.API | Avisar creación y cambio de estado de una orden |
| Notifications.API | Users.API | Validar usuario destinatario |
| Products.API | Orders.API | Validar órdenes activas antes de eliminar productos |
| Products.API | Cart.API | Saber qué usuarios tienen el producto en el carrito |
| Products.API | Notifications.API | Avisar cambios de precio o stock a quienes tienen el producto en el carrito |
| Users.API | Notifications.API | Avisar el registro de un usuario nuevo |

---

## Notificaciones

Cada microservicio genera notificaciones ante eventos relevantes. Todas se envían a
Notifications.API (`POST /api/notifications/send`, `tipo = "Push"`) y quedan
disponibles en el menú de notificaciones del cliente, asociadas al usuario destinatario.

El envío es **fire-and-forget**: si Notifications.API está caído, la operación principal
(registro, agregar al carrito, actualizar producto, crear orden) se completa igual y solo
se registra un warning en el log.

| Servicio | Evento | Destinatario | Mensaje |
|---|---|---|---|
| Users.API | Registro de usuario | el usuario nuevo | "¡Bienvenido {Nombre}! Tu cuenta fue creada con éxito." |
| Cart.API | Agregar producto al carrito | dueño del carrito | "Agregaste {producto} a tu carrito." |
| Products.API | Cambio de precio en un producto | usuarios que lo tienen en el carrito | "El producto {nombre} de tu carrito cambió de precio: ahora ${precio}." |
| Products.API | El producto queda sin stock | usuarios que lo tienen en el carrito | "El producto {nombre} de tu carrito se quedó sin stock." |
| Products.API | El stock cruza el umbral bajo (≤ 5, configurable) | usuarios que lo tienen en el carrito | "El producto {nombre} de tu carrito está por agotarse (quedan {stock})." |
| Orders.API | Crear orden | dueño de la orden | "Su orden #XXXXXXXX fue creada por un total de $XXX." |
| Orders.API | Cambio de estado de la orden | dueño de la orden | "Su orden #XXXXXXXX fue {estado}." |

El umbral de stock bajo de Products.API se configura en `appsettings.json`
(`Notifications:StockBajoUmbral`, default 5).

---

## Códigos de error

El sistema utiliza códigos de error personalizados para representar errores funcionales y reglas de negocio.

Cada error viaja en la respuesta con los campos `errorCode` y `errorMessage`.

**Users (USR)**

- USR-001: el email ya esta registrado.
- USR-002: datos del usuario invalidos.
- USR-003: credenciales incorrectas.
- USR-004: usuario bloqueado por demasiados intentos fallidos.
- USR-006: error interno al procesar el usuario.
- USR-007: usuario no encontrado.

**Products (PRD)**

- PRD-001: producto no encontrado.
- PRD-002: datos del producto invalidos.
- PRD-003: ya existe un producto con ese nombre en la categoria.
- PRD-004: no se puede eliminar un producto con órdenes activas.
- PRD-005: error interno.

**Cart (CRT)**

- CRT-001: carrito o item no encontrado.
- CRT-002: producto no encontrado.
- CRT-003: stock insuficiente.
- CRT-004: cantidad invalida.
- CRT-005: error interno.

**Orders (ORD)**

- ORD-001: orden no encontrada.
- ORD-002: datos de la orden invalidos.
- ORD-003: usuario no encontrado.
- ORD-004: producto no encontrado.
- ORD-005: stock insuficiente.
- ORD-006: transicion de estado no permitida.
- ORD-007: error interno.

**Notifications (NTF)**

- NTF-001: usuario destinatario no encontrado.
- NTF-002: tipo de notificación invalido.
- NTF-003: no se encontraron notificaciones para el usuario.
- NTF-004: error interno.


---

## Documentación entregable

La entrega incluye:

- Código fuente completo.
- README.md con instrucciones de ejecución.
- Diagrama de arquitectura.
- Capturas de Swagger UI mostrando respuestas exitosas y errores con `errorCode` y `errorMessage`.

---

## Cliente de consola

Hay un cliente de consola (`src/Cliente.Consola`) para probar las APIs sin Swagger.

Primero levantar las 5 APIs (cada una en su terminal) y despues correr:

```
dotnet run --project src/Cliente.Consola/Cliente.Consola.csproj
```

Se navega con numeros. Conviene arrancar registrandote en el menu de usuarios
para quedar logueado y poder usar carrito, órdenes y notificaciones.

---

## Integrantes

- Moncarz Morena - 915098
- Casal Belen - 911065
- Estevez Martin - 898286