using ControlVehicular.DTOs;

namespace ControlVehicular.Interfaces;

public interface ISaludoService
{
    SaludoRespuesta ObtenerSaludo(string? nombre);
}
