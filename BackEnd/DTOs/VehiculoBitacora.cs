namespace ControlVehicular.DTOs;

public sealed class VehiculoBitacora
{
    public int Id { get; set; }
    public int VehiculoId { get; set; }
    public string Accion { get; set; } = string.Empty;
    public DateTime FechaUtc { get; set; }
}
