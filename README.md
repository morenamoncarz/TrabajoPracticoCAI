# Trabajo Pr�ctico CAI � Sistema eCommerce

## Descripci�n general

Este proyecto implementa un sistema eCommerce basado en una arquitectura de microservicios utilizando .NET 8 Web API.

El sistema est� dividido en cinco microservicios principales:

- Users.API
- Products.API
- Cart.API
- Orders.API
- Notifications.API

Cada microservicio tiene responsabilidades propias, su propia base de datos SQLite y se comunica con otros servicios mediante HTTP.

---

## Arquitectura

El sistema utiliza una arquitectura basada en microservicios y separaci�n por capas.

Cada API se organiza principalmente en:

- Controllers: reciben las solicitudes HTTP.
- Services: contienen la l�gica de negocio.
- Repositories: acceden a la base de datos.
- Models / DTOs: representan los datos del sistema.
- ExceptionHandlers: manejan errores personalizados.
- Middleware: agrega funcionalidades transversales como Correlation ID.

Cada microservicio posee su propia base de datos SQLite, siguiendo el patr�n database per service.

---

## Microservicios

### Users.API

Responsable de la gesti�n de usuarios.

Endpoints principales:

- Registro de usuarios.
- Login.
- Consulta de usuario por ID.

Base de datos:

- users.db

---

### Products.API

Responsable de la gesti�n de productos.

Endpoints principales:

- Crear producto.
- Listar productos.
- Obtener producto por ID.
- Actualizar producto.
- Eliminar producto.

Base de datos:

- products.db

---

### Cart.API

Responsable de la gesti�n del carrito de compras.

Endpoints principales:

- Agregar productos al carrito.
- Consultar carrito.
- Actualizar cantidades.
- Eliminar productos del carrito.
- Vaciar el carrito completo.

Base de datos:

- cart.db

Comunicaci�n externa:

- Consulta Products.API para validar productos.

---

### Orders.API

Responsable de la gesti�n de �rdenes de compra.

Endpoints principales:

- Crear orden.
- Listar �rdenes.
- Obtener orden por ID.
- Actualizar estado de una orden.

Base de datos:

- orders.db

Comunicaci�n externa:

- Consulta Users.API para validar usuarios.
- Consulta Products.API para validar productos y stock.

---

### Notifications.API

Responsable de la gesti�n y env�o simulado de notificaciones.

Endpoints principales:

- Enviar notificaci�n.
- Consultar notificaciones por usuario.

Base de datos:

- notifications.db

Comunicaci�n externa:

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

## Ejecuci�n del proyecto

Para ejecutar cada microservicio, abrir una terminal en la ra�z del repositorio y correr:

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

Cada API debe quedar ejecut�ndose en una terminal independiente.

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

Tambi�n se utiliza:

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

Cada microservicio crea autom�ticamente su base de datos al iniciar mediante un `DatabaseInitializer`.

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

Los microservicios se comunican entre s� mediante llamadas HTTP utilizando distintos puertos locales.

Header utilizado:
```text
X-Correlation-Id
```
Este identificador permite rastrear una misma solicitud entre distintos microservicios.

---

## Comunicaci�n entre microservicios

| API origen | API destino | Motivo |
|---|---|---|
| Cart.API | Products.API | Validar productos |
| Orders.API | Users.API | Validar usuarios |
| Orders.API | Products.API | Validar precio y stock |
| Notifications.API | Users.API | Validar usuario destinatario |
| Products.API | Orders.API | Validar �rdenes activas antes de eliminar productos |

---

## C�digos de error

El sistema utiliza c�digos de error personalizados para representar errores funcionales y reglas de negocio.

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
- PRD-004: no se puede eliminar un producto con ordenes activas.
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
- NTF-002: tipo de notificacion invalido.
- NTF-003: no se encontraron notificaciones para el usuario.
- NTF-004: error interno.


---

## Documentaci�n entregable

La entrega incluye:

- C�digo fuente completo.
- README.md con instrucciones de ejecuci�n.
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
para quedar logueado y poder usar carrito, ordenes y notificaciones.

---

## Integrantes

- Moncarz Morena - 915098
- Casal Belen - 911065
- Estevez Martin - 898286