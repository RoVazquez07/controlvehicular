namespace ControlVehicular.Errores;

public sealed class RecursoNoAutorizadoException : Exception
{
    public RecursoNoAutorizadoException()
        : base("No se encontro el recurso solicitado.")
    {
    }
}
