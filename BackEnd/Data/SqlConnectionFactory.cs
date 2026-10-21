using ControlVehicular.Configuration;
using ControlVehicular.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace ControlVehicular.Data;

public sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly SqlServerOpciones _opciones;
    private readonly Lazy<string> _cadenaValidada;

    public SqlConnectionFactory(
        IOptions<SqlServerOpciones> opciones,
        IHostEnvironment entorno,
        ILogger<SqlConnectionFactory> logger)
    {
        _opciones = opciones.Value;
        _ = entorno;
        _ = logger;
        _cadenaValidada = new Lazy<string>(ResolverCadena);
    }

    public SqlConnection CreateConnection() => new(_cadenaValidada.Value);

    public async Task<SqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var conexion = CreateConnection();
        try
        {
            await conexion.OpenAsync(cancellationToken);
            return conexion;
        }
        catch
        {
            await conexion.DisposeAsync();
            throw;
        }
    }

    public static string ValidarCadena(string cadenaConexion, bool produccion)
    {
        _ = produccion;
        var builder = new SqlConnectionStringBuilder(cadenaConexion)
        {
            PersistSecurityInfo = false
        };

        if (!EsCifradoObligatorio(builder.Encrypt))
        {
            throw new InvalidOperationException("La cadena de conexion debe usar Encrypt=True (cifrado obligatorio).");
        }

        if (builder.TrustServerCertificate)
        {
            throw new InvalidOperationException(
                "TrustServerCertificate=True no esta permitido. Use un certificado de servidor valido.");
        }

        builder.Encrypt = SqlConnectionEncryptOption.Mandatory;
        builder.TrustServerCertificate = false;
        return builder.ConnectionString;
    }

    private string ResolverCadena()
    {
        if (string.IsNullOrWhiteSpace(_opciones.CadenaConexion))
        {
            throw new InvalidOperationException(
                "No hay cadena de conexion SQL Server. Configure SqlServer:CadenaConexion mediante User Secrets o variables de entorno.");
        }

        return ValidarCadena(_opciones.CadenaConexion, produccion: false);
    }

    private static bool EsCifradoObligatorio(SqlConnectionEncryptOption encrypt) =>
        encrypt == SqlConnectionEncryptOption.Mandatory
        || encrypt == SqlConnectionEncryptOption.Strict;
}
