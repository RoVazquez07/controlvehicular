using System.Data;
using ControlVehicular.Configuration;
using ControlVehicular.DTOs;
using ControlVehicular.Errores;
using ControlVehicular.Interfaces;
using ControlVehicular.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace ControlVehicular.Data.Repositorios;

public sealed class VehiculoRepositorio : IVehiculoRepositorio
{
    private const string ColumnasVehiculo =
        "Id, Placa, Marca, Modelo, Anio, Estado, FechaRegistroUtc, RegistradoPor";

    private readonly IDbConnectionFactory _conexiones;
    private readonly SqlServerOpciones _opciones;
    private readonly ILogger<VehiculoRepositorio> _logger;

    public VehiculoRepositorio(
        IDbConnectionFactory conexiones,
        IOptions<SqlServerOpciones> opciones,
        ILogger<VehiculoRepositorio> logger)
    {
        _conexiones = conexiones;
        _opciones = opciones.Value;
        _logger = logger;
    }

    public async Task<Vehiculo?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = $@"
SELECT {ColumnasVehiculo}
FROM dbo.Vehiculos
WHERE Id = @Id;";

        var parametros = ParametrosSql.Crear().AddInt("@Id", id);

        try
        {
            await using var conexion = await _conexiones.OpenConnectionAsync(cancellationToken);
            var comando = new CommandDefinition(
                sql,
                parametros,
                commandTimeout: _opciones.CommandTimeoutSeconds,
                commandType: CommandType.Text,
                cancellationToken: cancellationToken);

            return await conexion.QuerySingleOrDefaultAsync<Vehiculo>(comando);
        }
        catch (SqlException ex)
        {
            throw Envolver(ex, "No se pudo consultar el vehiculo.");
        }
    }

    public async Task<Vehiculo?> ObtenerPorIdSpAsync(int id, CancellationToken cancellationToken = default)
    {
        var parametros = ParametrosSql.Crear().AddInt("@Id", id);

        try
        {
            await using var conexion = await _conexiones.OpenConnectionAsync(cancellationToken);
            var comando = new CommandDefinition(
                "dbo.Vehiculo_ObtenerPorId",
                parametros,
                commandTimeout: _opciones.CommandTimeoutSeconds,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return await conexion.QuerySingleOrDefaultAsync<Vehiculo>(comando);
        }
        catch (SqlException ex)
        {
            throw Envolver(ex, "No se pudo consultar el vehiculo.");
        }
    }

    public async Task<ResultadoPaginado<Vehiculo>> ListarAsync(VehiculoFiltro filtro, CancellationToken cancellationToken = default)
    {
        string columnaOrden;
        if (string.IsNullOrWhiteSpace(filtro.OrdenarPor))
        {
            columnaOrden = "Id";
        }
        else if (!IdentificadoresSql.TryObtenerColumnaOrden(filtro.OrdenarPor, out columnaOrden))
        {
            throw new AccesoDatosException("El campo de orden no esta permitido.");
        }

        var direccion = IdentificadoresSql.ObtenerDireccionOrden(filtro.Direccion);
        var pagina = filtro.Pagina < 1 ? 1 : filtro.Pagina;
        var tamano = Math.Clamp(filtro.TamanoPagina, 1, _opciones.TamanoPaginaMaximo);
        var offset = (pagina - 1) * tamano;

        var placaFiltro = string.IsNullOrWhiteSpace(filtro.Placa)
            ? null
            : filtro.Placa.Trim() + "%";

        var parametros = ParametrosSql.Crear()
            .AddNVarChar("@Placa", placaFiltro, 21)
            .AddNVarChar("@RegistradoPor", string.IsNullOrWhiteSpace(filtro.RegistradoPor) ? null : filtro.RegistradoPor, 64)
            .AddInt("@Offset", offset)
            .AddInt("@PageSize", tamano);

        var sql = $@"
SELECT COUNT(1)
FROM dbo.Vehiculos
WHERE (@Placa IS NULL OR Placa LIKE @Placa)
  AND (@RegistradoPor IS NULL OR RegistradoPor = @RegistradoPor);

SELECT {ColumnasVehiculo}
FROM dbo.Vehiculos
WHERE (@Placa IS NULL OR Placa LIKE @Placa)
  AND (@RegistradoPor IS NULL OR RegistradoPor = @RegistradoPor)
ORDER BY {columnaOrden} {direccion}
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        try
        {
            await using var conexion = await _conexiones.OpenConnectionAsync(cancellationToken);
            var comando = new CommandDefinition(
                sql,
                parametros,
                commandTimeout: _opciones.CommandTimeoutSeconds,
                commandType: CommandType.Text,
                cancellationToken: cancellationToken);

            using var multiple = await conexion.QueryMultipleAsync(comando);
            var total = await multiple.ReadSingleAsync<int>();
            var items = (await multiple.ReadAsync<Vehiculo>()).AsList();

            return new ResultadoPaginado<Vehiculo>
            {
                Items = items,
                Total = total,
                Pagina = pagina,
                TamanoPagina = tamano
            };
        }
        catch (SqlException ex)
        {
            throw Envolver(ex, "No se pudo listar vehiculos.");
        }
    }

    public async Task<Vehiculo> InsertarConBitacoraAsync(Vehiculo vehiculo, CancellationToken cancellationToken = default)
    {
        const string insertarVehiculo = @"
INSERT INTO dbo.Vehiculos (Placa, Marca, Modelo, Anio, Estado, FechaRegistroUtc, RegistradoPor)
OUTPUT INSERTED.Id, INSERTED.Placa, INSERTED.Marca, INSERTED.Modelo, INSERTED.Anio, INSERTED.Estado, INSERTED.FechaRegistroUtc, INSERTED.RegistradoPor
VALUES (@Placa, @Marca, @Modelo, @Anio, @Estado, @FechaRegistroUtc, @RegistradoPor);";

        const string insertarBitacora = @"
INSERT INTO dbo.VehiculoBitacora (VehiculoId, Accion, FechaUtc)
VALUES (@VehiculoId, @Accion, @FechaUtc);";

        var parametrosVehiculo = ParametrosSql.Crear()
            .AddNVarChar("@Placa", vehiculo.Placa, 20)
            .AddNVarChar("@Marca", vehiculo.Marca, 80)
            .AddNVarChar("@Modelo", vehiculo.Modelo, 80)
            .AddInt("@Anio", vehiculo.Anio)
            .AddNVarChar("@Estado", vehiculo.Estado, 20)
            .AddDateTime2("@FechaRegistroUtc", vehiculo.FechaRegistroUtc)
            .AddNVarChar("@RegistradoPor", vehiculo.RegistradoPor, 64);

        await using var conexion = await _conexiones.OpenConnectionAsync(cancellationToken);
        await using var transaccion = (SqlTransaction)await conexion.BeginTransactionAsync(cancellationToken);

        try
        {
            var comandoVehiculo = new CommandDefinition(
                insertarVehiculo,
                parametrosVehiculo,
                transaction: transaccion,
                commandTimeout: _opciones.CommandTimeoutSeconds,
                commandType: CommandType.Text,
                cancellationToken: cancellationToken);

            var creado = await conexion.QuerySingleAsync<Vehiculo>(comandoVehiculo);

            var parametrosBitacora = ParametrosSql.Crear()
                .AddInt("@VehiculoId", creado.Id)
                .AddNVarChar("@Accion", "Registro", 80)
                .AddDateTime2("@FechaUtc", DateTime.UtcNow);

            var comandoBitacora = new CommandDefinition(
                insertarBitacora,
                parametrosBitacora,
                transaction: transaccion,
                commandTimeout: _opciones.CommandTimeoutSeconds,
                commandType: CommandType.Text,
                cancellationToken: cancellationToken);

            await conexion.ExecuteAsync(comandoBitacora);
            await transaccion.CommitAsync(CancellationToken.None);
            return creado;
        }
        catch (Exception ex)
        {
            await transaccion.RollbackAsync(CancellationToken.None);
            if (ex is SqlException sqlEx)
            {
                throw Envolver(sqlEx, "No se pudo registrar el vehiculo.");
            }

            throw;
        }
    }

    public async Task<bool> CambiarEstadoSpAsync(int id, string estado, CancellationToken cancellationToken = default)
    {
        var parametros = ParametrosSql.Crear()
            .AddInt("@Id", id)
            .AddNVarChar("@Estado", estado, 20)
            .AddOutputInt("@FilasAfectadas");

        try
        {
            await using var conexion = await _conexiones.OpenConnectionAsync(cancellationToken);
            var comando = new CommandDefinition(
                "dbo.Vehiculo_CambiarEstado",
                parametros,
                commandTimeout: _opciones.CommandTimeoutSeconds,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            await conexion.ExecuteAsync(comando);
            return parametros.Get<int>("@FilasAfectadas") == 1;
        }
        catch (SqlException ex)
        {
            throw Envolver(ex, "No se pudo actualizar el estado del vehiculo.");
        }
    }

    private Exception Envolver(SqlException ex, string mensajeCliente)
    {
        _logger.LogError(ex, "Error SQL Server Number={Number} Class={Class}", ex.Number, ex.Class);
        if (ex.Number is 2627 or 2601)
        {
            return new RecursoDuplicadoException("Ya existe un vehiculo con esos datos.");
        }

        return new AccesoDatosException(mensajeCliente, ex);
    }
}
