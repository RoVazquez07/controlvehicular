using ControlVehicular.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ControlVehicular.Controllers;

[Authorize]
[EnableRateLimiting("api")]
[ApiController]
[Route("hola")]
public class HolaController : ControllerBase
{
    private readonly ISaludoService _saludoService;

    public HolaController(ISaludoService saludoService)
    {
        _saludoService = saludoService;
    }

    [HttpGet]
    public IActionResult Hola()
    {
        var usuario = User.Identity?.Name ?? "usuario";
        return Ok(_saludoService.ObtenerSaludo(usuario));
    }
}
