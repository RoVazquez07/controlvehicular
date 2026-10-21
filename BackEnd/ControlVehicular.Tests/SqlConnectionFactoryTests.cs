using ControlVehicular.Data;

namespace ControlVehicular.Tests;

public class SqlConnectionFactoryTests
{
    [Fact]
    public void ValidarCadena_exige_cifrado()
    {
        var cadena = "Server=localhost;Database=ControlVehicular;User Id=cv_app;Password=x;Encrypt=False;TrustServerCertificate=False";

        Assert.Throws<InvalidOperationException>(() => SqlConnectionFactory.ValidarCadena(cadena, produccion: false));
    }

    [Fact]
    public void ValidarCadena_rechaza_TrustServerCertificate()
    {
        var cadena = "Server=localhost;Database=ControlVehicular;User Id=cv_app;Password=x;Encrypt=True;TrustServerCertificate=True";

        Assert.Throws<InvalidOperationException>(() => SqlConnectionFactory.ValidarCadena(cadena, produccion: false));
        Assert.Throws<InvalidOperationException>(() => SqlConnectionFactory.ValidarCadena(cadena, produccion: true));
    }

    [Fact]
    public void ValidarCadena_acepta_cifrado_obligatorio()
    {
        var cadena = "Server=localhost;Database=ControlVehicular;User Id=cv_app;Password=x;Encrypt=True;TrustServerCertificate=False";

        var validada = SqlConnectionFactory.ValidarCadena(cadena, produccion: true);
        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(validada);

        Assert.Equal(Microsoft.Data.SqlClient.SqlConnectionEncryptOption.Mandatory, builder.Encrypt);
        Assert.False(builder.TrustServerCertificate);
    }
}
