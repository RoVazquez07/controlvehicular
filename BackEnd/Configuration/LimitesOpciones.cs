namespace ControlVehicular.Configuration;

public class LimitesOpciones
{
    public const string Seccion = "Limites";

    public int TamanoMaximoSolicitudBytes { get; set; } = 65_536;
    public int LoginIntentosPorMinuto { get; set; } = 5;
    public int ApiSolicitudesPorMinuto { get; set; } = 60;
}
