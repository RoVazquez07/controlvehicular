namespace ControlVehicular.Configuration;

public class JwtOpciones
{
    public const string Seccion = "Jwt";
    public const int LongitudMinimaClave = 32;

    public string Clave { get; set; } = string.Empty;
    public string Emisor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public int ExpiracionMinutos { get; set; } = 15;
}
