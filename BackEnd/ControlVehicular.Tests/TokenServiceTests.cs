using ControlVehicular.Configuration;
using ControlVehicular.DTOs;
using ControlVehicular.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ControlVehicular.Tests;

public class TokenServiceTests
{
    [Fact]
    public void Autenticar_rechaza_password_incorrecto()
    {
        var hasher = new PasswordHasher<object>();
        var hash = hasher.HashPassword(new object(), "ClaveLocalSegura1!");
        var servicio = new TokenService(
            Options.Create(new JwtOpciones
            {
                Clave = "ClaveDePruebaConAlMenos32Caracteres!!",
                Emisor = "test",
                Audiencia = "test",
                ExpiracionMinutos = 15
            }),
            Options.Create(new AuthOpciones
            {
                Usuario = "admin",
                PasswordHash = hash,
                Rol = RolesAplicacion.Administrador
            }),
            hasher);

        var resultado = servicio.Autenticar(new LoginSolicitud { Usuario = "admin", Password = "otra" });
        Assert.Null(resultado);
    }

    [Fact]
    public void Autenticar_emite_token_con_rol_del_servidor()
    {
        var hasher = new PasswordHasher<object>();
        var hash = hasher.HashPassword(new object(), "ClaveLocalSegura1!");
        var servicio = new TokenService(
            Options.Create(new JwtOpciones
            {
                Clave = "ClaveDePruebaConAlMenos32Caracteres!!",
                Emisor = "test",
                Audiencia = "test",
                ExpiracionMinutos = 15
            }),
            Options.Create(new AuthOpciones
            {
                Usuario = "admin",
                PasswordHash = hash,
                Rol = RolesAplicacion.Administrador
            }),
            hasher);

        var resultado = servicio.Autenticar(new LoginSolicitud { Usuario = "admin", Password = "ClaveLocalSegura1!" });
        Assert.NotNull(resultado);
        Assert.False(string.IsNullOrWhiteSpace(resultado!.Token));
    }
}
