using System.Security.Claims;
using ControlVehicular.Configuration;
using ControlVehicular.DTOs;
using ControlVehicular.Errores;
using ControlVehicular.Interfaces;
using ControlVehicular.Models;
using ControlVehicular.Security;
using ControlVehicular.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace ControlVehicular.Tests;

public class VehiculoServiceTests
{
    [Fact]
    public async Task ListarAsync_rechaza_order_by_inyectado()
    {
        var servicio = CrearServicio();

        await Assert.ThrowsAsync<ErrorValidacionNegocioException>(() => servicio.ListarAsync(new VehiculoConsulta
        {
            OrdenarPor = "Id; DROP TABLE dbo.Vehiculos"
        }, Usuario("admin", RolesAplicacion.Administrador)));
    }

    [Fact]
    public async Task CrearAsync_rechaza_estado_fuera_de_lista_blanca()
    {
        var servicio = CrearServicio();

        await Assert.ThrowsAsync<ErrorValidacionNegocioException>(() => servicio.CrearAsync(new VehiculoCrearSolicitud
        {
            Placa = "ABC-123",
            Marca = "Toyota",
            Modelo = "Corolla",
            Anio = 2020,
            Estado = "Activo'; DROP TABLE dbo.Vehiculos;--"
        }, Usuario("admin", RolesAplicacion.Administrador)));
    }

    private static VehiculoService CrearServicio() =>
        new(new RepositorioInerte(), new AutorizacionPermisiva(), Options.Create(new SqlServerOpciones()));

    private static ClaimsPrincipal Usuario(string nombre, string rol)
    {
        var identidad = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, nombre), new Claim(ClaimTypes.Role, rol)],
            authenticationType: "Test");
        return new ClaimsPrincipal(identidad);
    }

    private sealed class AutorizacionPermisiva : IAuthorizationService
    {
        public Task<AuthorizationResult> AuthorizeAsync(
            ClaimsPrincipal user,
            object? resource,
            IEnumerable<IAuthorizationRequirement> requirements) =>
            Task.FromResult(AuthorizationResult.Success());

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName) =>
            Task.FromResult(AuthorizationResult.Success());
    }

    private sealed class RepositorioInerte : IVehiculoRepositorio
    {
        public Task<bool> CambiarEstadoSpAsync(int id, string estado, CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<Vehiculo> InsertarConBitacoraAsync(Vehiculo vehiculo, CancellationToken cancellationToken = default) =>
            Task.FromResult(vehiculo);

        public Task<ResultadoPaginado<Vehiculo>> ListarAsync(VehiculoFiltro filtro, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ResultadoPaginado<Vehiculo>());

        public Task<Vehiculo?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Vehiculo?>(null);

        public Task<Vehiculo?> ObtenerPorIdSpAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Vehiculo?>(null);
    }
}
