using System.Security.Claims;
using ControlVehicular.Models;
using ControlVehicular.Security;
using Microsoft.AspNetCore.Authorization;

namespace ControlVehicular.Tests;

public class AutorizadorVehiculoHandlerTests
{
    [Fact]
    public async Task Administrador_accede_a_cualquier_recurso()
    {
        var handler = new AutorizadorVehiculoHandler();
        var usuario = Principal("admin", RolesAplicacion.Administrador);
        var recurso = new Vehiculo { RegistradoPor = "otro" };
        var contexto = new AuthorizationHandlerContext([new RequisitoRecursoVehiculo()], usuario, recurso);

        await handler.HandleAsync(contexto);

        Assert.True(contexto.HasSucceeded);
    }

    [Fact]
    public async Task Operador_no_accede_a_recurso_de_otro()
    {
        var handler = new AutorizadorVehiculoHandler();
        var usuario = Principal("operador1", RolesAplicacion.Operador);
        var recurso = new Vehiculo { RegistradoPor = "operador2" };
        var contexto = new AuthorizationHandlerContext([new RequisitoRecursoVehiculo()], usuario, recurso);

        await handler.HandleAsync(contexto);

        Assert.False(contexto.HasSucceeded);
    }

    private static ClaimsPrincipal Principal(string nombre, string rol) =>
        new(new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, nombre), new Claim(ClaimTypes.Role, rol)],
            "Test"));
}
