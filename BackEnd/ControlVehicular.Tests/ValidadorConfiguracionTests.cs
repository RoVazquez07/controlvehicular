using ControlVehicular.Configuration;

namespace ControlVehicular.Tests;

public class ValidadorConfiguracionTests
{
    [Fact]
    public void Cors_rechaza_wildcard()
    {
        var cors = new CorsOpciones { OrigenesPermitidos = ["*"] };
        Assert.Throws<InvalidOperationException>(() => ValidadorConfiguracion.ValidarCors(cors, produccion: false));
    }

    [Fact]
    public void Jwt_exige_clave_de_32_caracteres()
    {
        var jwt = new JwtOpciones
        {
            Emisor = "a",
            Audiencia = "b",
            Clave = "corta",
            ExpiracionMinutos = 15
        };

        Assert.Throws<InvalidOperationException>(() => ValidadorConfiguracion.ValidarJwt(jwt));
    }

    [Fact]
    public void Jwt_rechaza_clave_de_ejemplo()
    {
        var jwt = new JwtOpciones
        {
            Emisor = "ControlVehicular",
            Audiencia = "ControlVehicularClientes",
            Clave = "ClaveSuperSecretaDeAlMenos32Caracteres!!",
            ExpiracionMinutos = 15
        };

        Assert.Throws<InvalidOperationException>(() => ValidadorConfiguracion.ValidarJwt(jwt));
    }
}
