namespace ControlVehicular.Models;

public sealed class Vehiculo
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaRegistroUtc { get; set; }
    public string RegistradoPor { get; set; } = string.Empty;
}
