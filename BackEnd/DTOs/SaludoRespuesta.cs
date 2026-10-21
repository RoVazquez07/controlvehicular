namespace ControlVehicular.DTOs;

public class SaludoRespuesta
{
    public string Mensaje { get; set; } = string.Empty;
    public string Framework { get; set; } = string.Empty;
    public DateTime HoraUtc { get; set; }
}
