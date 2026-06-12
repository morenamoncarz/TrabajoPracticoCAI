using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Services;

namespace Users.API.Controllers;

[ApiController]
[Route("api/users")]
[Tags("Users")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;

    public UsersController(UserService service)
    {
        _service = service;
    }

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    /// <remarks>
    /// Al registrar el usuario se le envia una notificacion de bienvenida a traves de
    /// Notifications.API. El envio es fire-and-forget: si Notifications esta caido el
    /// registro se completa igual.
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserResponse>> Register(RegisterUserRequest request)
    {
        // Este endpoint registra un nuevo usuario
        var user = await _service.Register(request);

        return Created("/api/users/register", user);
    }

    /// <summary>
    /// Autentica un usuario usando email y contraseña.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<UserResponse> Login(LoginRequest request)
    {
        // Este endpoint valida email y password
        var user = _service.Login(request);

        return Ok(user);
    }

    /// <summary>
    /// Busca un usuario por id.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<UserResponse> GetById(Guid id)
    {
        // Este endpoint permite que otros microservicios validen usuarios existentes, se agregó debido a que en Orders hay que validar el usuario
        var user = _service.GetById(id);

        return Ok(user);
    }
}