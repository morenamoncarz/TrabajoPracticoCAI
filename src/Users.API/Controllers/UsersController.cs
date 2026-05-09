using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Services;

namespace Users.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;

    public UsersController(UserService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public ActionResult<UserResponse> Register(RegisterUserRequest request)
    {
        // Este endpoint registra un nuevo usuario
        var user = _service.Register(request);

        return Created("/api/users/register", user);
    }

    [HttpPost("login")]
    public ActionResult<UserResponse> Login(LoginRequest request)
    {
        // Este endpoint valida email y password
        var user = _service.Login(request);

        return Ok(user);
    }
}