using ControlVehicular.DTOs;
using ControlVehicular.Interfaces;
using ControlVehicular.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ControlVehicular.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IProteccionLogin _proteccion;

    public AuthController(ITokenService tokenService, IProteccionLogin proteccion)
    {
        _tokenService = tokenService;
        _proteccion = proteccion;
    }

    [AllowAnonymous]
    [EnableRateLimiting("login")]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginSolicitud solicitud)
    {
        var clave = HttpContext.Connection.RemoteIpAddress?.ToString() + ":" + solicitud.Usuario;
        if (_proteccion.EstaBloqueado(clave))
        {
            return StatusCode(StatusCodes.Status429TooManyRequests, new ProblemDetails
            {
                Title = "Demasiados intentos",
                Status = StatusCodes.Status429TooManyRequests,
                Detail = "Intente mas tarde."
            });
        }

        var respuesta = _tokenService.Autenticar(solicitud);
        if (respuesta is null)
        {
            _proteccion.RegistrarFallo(clave);
            return Unauthorized(new ProblemDetails
            {
                Title = "No autorizado",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Credenciales invalidas."
            });
        }

        _proteccion.RegistrarExito(clave);
        return Ok(respuesta);
    }
}
