namespace ControlVehicular.Configuration;

public class CorsOpciones
{
    public const string Seccion = "Cors";
    public const string NombrePolitica = "PoliticaCors";

    public string[] OrigenesPermitidos { get; set; } = [];
}
