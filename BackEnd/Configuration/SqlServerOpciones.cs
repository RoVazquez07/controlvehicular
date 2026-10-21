namespace ControlVehicular.Configuration;

public class SqlServerOpciones
{
    public const string Seccion = "SqlServer";

    public string CadenaConexion { get; set; } = string.Empty;
    public int CommandTimeoutSeconds { get; set; } = 30;
    public int TamanoPaginaPredeterminado { get; set; } = 20;
    public int TamanoPaginaMaximo { get; set; } = 100;
}
