namespace ControlVehicular.Errores;

public sealed class ErrorValidacionNegocioException : Exception
{
    public ErrorValidacionNegocioException(string mensaje) : base(mensaje)
    {
    }
}
