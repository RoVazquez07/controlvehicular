using ControlVehicular.Configuration;
using ControlVehicular.Data;
using ControlVehicular.Data.Repositorios;
using ControlVehicular.Errores;
using ControlVehicular.Interfaces;
using ControlVehicular.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ControlVehicular.Tests;

public class VehiculoRepositorioIntegrationTests
{
    private static VehiculoRepositorio CrearRepositorio()
    {
        var cadena = Environment.GetEnvironmentVariable("CONTROLVEHICULAR_TEST_SQL")
            ?? throw new InvalidOperationException("CONTROLVEHICULAR_TEST_SQL no esta definida.");

        var opciones = Options.Create(new SqlServerOpciones
        {
            CadenaConexion = cadena,
            CommandTimeoutSeconds = 30,
            TamanoPaginaMaximo = 100,
            TamanoPaginaPredeterminado = 20
        });

        var entorno = new EntornoPrueba { EnvironmentName = Environments.Development };
        IDbConnectionFactory factory = new SqlConnectionFactory(opciones, entorno, NullLogger<SqlConnectionFactory>.Instance);
        return new VehiculoRepositorio(factory, opciones, NullLogger<VehiculoRepositorio>.Instance);
    }

    [RequiereSqlServerFact]
    public async Task Esquema_y_procedimientos_quedan_disponibles()
    {
        var cadena = SqlConnectionFactory.ValidarCadena(
            Environment.GetEnvironmentVariable("CONTROLVEHICULAR_TEST_SQL")!,
            produccion: false);

        await using var conexion = new SqlConnection(cadena);
        await conexion.OpenAsync();
        var script = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Scripts", "001_Esquema_SqlServer2012.sql"));
        await SqlScriptRunner.EjecutarConGoAsync(conexion, script);
    }

    [RequiereSqlServerFact]
    public async Task Insertar_consultar_sp_paginar_y_cambiar_estado()
    {
        await Esquema_y_procedimientos_quedan_disponibles();
        var repo = CrearRepositorio();
        var placa = "T" + Guid.NewGuid().ToString("N")[..8];

        var creado = await repo.InsertarConBitacoraAsync(new Vehiculo
        {
            Placa = placa,
            Marca = "Toyota",
            Modelo = "Corolla",
            Anio = 2020,
            Estado = "Activo",
            FechaRegistroUtc = DateTime.UtcNow,
            RegistradoPor = "admin"
        });

        Assert.True(creado.Id > 0);

        var porConsulta = await repo.ObtenerPorIdAsync(creado.Id);
        var porSp = await repo.ObtenerPorIdSpAsync(creado.Id);
        Assert.NotNull(porConsulta);
        Assert.NotNull(porSp);
        Assert.Equal(placa, porSp!.Placa);

        var pagina = await repo.ListarAsync(new VehiculoFiltro
        {
            Pagina = 1,
            TamanoPagina = 10,
            Placa = placa,
            OrdenarPor = "placa",
            Direccion = "asc"
        });

        Assert.Equal(1, pagina.Total);
        Assert.Single(pagina.Items);

        var inyeccionLike = await repo.ListarAsync(new VehiculoFiltro
        {
            Pagina = 1,
            TamanoPagina = 10,
            Placa = "' OR 1=1 --"
        });
        Assert.Equal(0, inyeccionLike.Total);

        var actualizado = await repo.CambiarEstadoSpAsync(creado.Id, "Mantenimiento");
        Assert.True(actualizado);
        var despues = await repo.ObtenerPorIdSpAsync(creado.Id);
        Assert.Equal("Mantenimiento", despues!.Estado);
    }

    [RequiereSqlServerFact]
    public async Task Transaccion_hace_rollback_si_falla_la_segunda_operacion()
    {
        await Esquema_y_procedimientos_quedan_disponibles();
        var repo = CrearRepositorio();
        var placa = "R" + Guid.NewGuid().ToString("N")[..8];

        await repo.InsertarConBitacoraAsync(new Vehiculo
        {
            Placa = placa,
            Marca = "Honda",
            Modelo = "Civic",
            Anio = 2019,
            Estado = "Activo",
            FechaRegistroUtc = DateTime.UtcNow,
            RegistradoPor = "admin"
        });

        await Assert.ThrowsAsync<RecursoDuplicadoException>(() => repo.InsertarConBitacoraAsync(new Vehiculo
        {
            Placa = placa,
            Marca = "Honda",
            Modelo = "Civic",
            Anio = 2019,
            Estado = "Activo",
            FechaRegistroUtc = DateTime.UtcNow,
            RegistradoPor = "admin"
        }));

        var pagina = await repo.ListarAsync(new VehiculoFiltro
        {
            Pagina = 1,
            TamanoPagina = 20,
            Placa = placa
        });

        Assert.Equal(1, pagina.Total);
    }

    [RequiereSqlServerFact]
    public async Task Listar_rechaza_order_by_fuera_de_lista_blanca()
    {
        await Esquema_y_procedimientos_quedan_disponibles();
        var repo = CrearRepositorio();

        await Assert.ThrowsAsync<AccesoDatosException>(() => repo.ListarAsync(new VehiculoFiltro
        {
            Pagina = 1,
            TamanoPagina = 10,
            OrdenarPor = "Id; DROP TABLE dbo.Vehiculos"
        }));
    }
}
