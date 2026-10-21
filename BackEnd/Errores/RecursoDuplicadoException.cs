namespace ControlVehicular.Errores;

public sealed class RecursoDuplicadoException : Exception
{
    public RecursoDuplicadoException(string mensaje) : base(mensaje)
    {
    }
}
