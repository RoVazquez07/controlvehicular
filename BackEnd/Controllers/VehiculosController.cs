using ControlVehicular.DTOs;
using ControlVehicular.Interfaces;
using ControlVehicular.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ControlVehicular.Controllers;

[Authorize]
[EnableRateLimiting("api")]
[ApiController]
[Route("vehiculos")]
public sealed class VehiculosController : ControllerBase
{
    private readonly IVehiculoService _vehiculos;

    public VehiculosController(IVehiculoService vehiculos)
    {
        _vehiculos = vehiculos;
    }

    [Authorize(Policy = Politicas.ConsultarVehiculos)]
    [HttpGet]
    public async Task<ActionResult<ResultadoPaginado<VehiculoRespuesta>>> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        [FromQuery] string? placa = null,
        [FromQuery] string? ordenarPor = null,
        [FromQuery] string? direccion = null,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _vehiculos.ListarAsync(new VehiculoConsulta
        {
            Pagina = pagina,
            TamanoPagina = tamanoPagina,
            Placa = placa,
            OrdenarPor = ordenarPor,
            Direccion = direccion
        }, User, cancellationToken);

        return Ok(resultado);
    }

    [Authorize(Policy = Politicas.ConsultarVehiculos)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehiculoRespuesta>> Obtener(int id, CancellationToken cancellationToken)
    {
        var vehiculo = await _vehiculos.ObtenerPorIdAsync(id, User, cancellationToken);
        if (vehiculo is null)
        {
            return NotFound();
        }

        return Ok(vehiculo);
    }

    [Authorize(Policy = Politicas.RegistrarVehiculos)]
    [HttpPost]
    public async Task<ActionResult<VehiculoRespuesta>> Crear(
        [FromBody] VehiculoCrearSolicitud solicitud,
        CancellationToken cancellationToken)
    {
        var creado = await _vehiculos.CrearAsync(solicitud, User, cancellationToken);
        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }

    [Authorize(Policy = Politicas.CambiarEstadoVehiculos)]
    [HttpPut("{id:int}/estado")]
    public async Task<IActionResult> CambiarEstado(
        int id,
        [FromBody] VehiculoCambiarEstadoSolicitud solicitud,
        CancellationToken cancellationToken)
    {
        await _vehiculos.CambiarEstadoAsync(id, solicitud.Estado, User, cancellationToken);
        return NoContent();
    }
}
