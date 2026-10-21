namespace ControlVehicular.Data;

public sealed class AccesoDatosException : Exception
{
    public AccesoDatosException(string mensaje, Exception? interna = null)
        : base(mensaje, interna)
    {
    }
}
