using System.Security.Claims;
using ControlVehicular.DTOs;

namespace ControlVehicular.Interfaces;

public interface IVehiculoService
{
    Task<VehiculoRespuesta?> ObtenerPorIdAsync(int id, ClaimsPrincipal usuario, CancellationToken cancellationToken = default);
    Task<ResultadoPaginado<VehiculoRespuesta>> ListarAsync(VehiculoConsulta consulta, ClaimsPrincipal usuario, CancellationToken cancellationToken = default);
    Task<VehiculoRespuesta> CrearAsync(VehiculoCrearSolicitud solicitud, ClaimsPrincipal usuario, CancellationToken cancellationToken = default);
    Task CambiarEstadoAsync(int id, string estado, ClaimsPrincipal usuario, CancellationToken cancellationToken = default);
}
