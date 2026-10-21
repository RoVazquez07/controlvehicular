using ControlVehicular.DTOs;
using ControlVehicular.Interfaces;

namespace ControlVehicular.Services;

public class SaludoService : ISaludoService
{
    public SaludoRespuesta ObtenerSaludo(string? nombre)
    {
        var mensaje = string.IsNullOrWhiteSpace(nombre)
            ? "Hola mundo"
            : $"Hola, {nombre}";

        return new SaludoRespuesta
        {
            Mensaje = mensaje,
            Framework = ".NET 10",
            HoraUtc = DateTime.UtcNow
        };
    }
}
