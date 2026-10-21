namespace ControlVehicular.Configuration;

public class AuthOpciones
{
    public const string Seccion = "Auth";

    public string Usuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
