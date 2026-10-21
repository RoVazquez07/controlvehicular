namespace ControlVehicular.DTOs;

public sealed class VehiculoRespuesta
{
    public int Id { get; init; }
    public string Placa { get; init; } = string.Empty;
    public string Marca { get; init; } = string.Empty;
    public string Modelo { get; init; } = string.Empty;
    public int Anio { get; init; }
    public string Estado { get; init; } = string.Empty;
    public DateTime FechaRegistroUtc { get; init; }
}
