namespace ControlVehicular.Security;

/// <summary>
/// Nombres de rol usados por las politicas de la API.
/// No representan un catalogo de negocio validado por la organizacion;
/// deben sustituirse cuando exista el modelo real de identidad.
/// </summary>
public static class RolesAplicacion
{
    public const string Administrador = "Administrador";
    public const string Operador = "Operador";
}
