namespace ControlVehicular.Models;

public sealed class VehiculoFiltro
{
    public int Pagina { get; init; } = 1;
    public int TamanoPagina { get; init; } = 20;
    public string? Placa { get; init; }
    public string? OrdenarPor { get; init; }
    public string? Direccion { get; init; }
    public string? RegistradoPor { get; init; }
}
