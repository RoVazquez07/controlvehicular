using ControlVehicular.Models;
using Microsoft.AspNetCore.Authorization;

namespace ControlVehicular.Security;

public sealed class AutorizadorVehiculoHandler : AuthorizationHandler<RequisitoRecursoVehiculo, Vehiculo>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequisitoRecursoVehiculo requirement,
        Vehiculo resource)
    {
        if (context.User.IsInRole(RolesAplicacion.Administrador))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var usuario = context.User.Identity?.Name;
        if (context.User.IsInRole(RolesAplicacion.Operador)
            && !string.IsNullOrWhiteSpace(usuario)
            && string.Equals(resource.RegistradoPor, usuario, StringComparison.Ordinal))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
