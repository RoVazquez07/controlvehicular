using ControlVehicular.DTOs;
using ControlVehicular.Models;

namespace ControlVehicular.Interfaces;

public interface IVehiculoRepositorio
{
    Task<Vehiculo?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Vehiculo?> ObtenerPorIdSpAsync(int id, CancellationToken cancellationToken = default);
    Task<ResultadoPaginado<Vehiculo>> ListarAsync(VehiculoFiltro filtro, CancellationToken cancellationToken = default);
    Task<Vehiculo> InsertarConBitacoraAsync(Vehiculo vehiculo, CancellationToken cancellationToken = default);
    Task<bool> CambiarEstadoSpAsync(int id, string estado, CancellationToken cancellationToken = default);
}
