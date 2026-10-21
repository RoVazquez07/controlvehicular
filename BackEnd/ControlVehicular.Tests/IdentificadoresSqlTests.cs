using ControlVehicular.Data;

namespace ControlVehicular.Tests;

public class IdentificadoresSqlTests
{
    [Theory]
    [InlineData("id", "Id")]
    [InlineData("placa", "Placa")]
    [InlineData("FechaRegistro", "FechaRegistroUtc")]
    public void TryObtenerColumnaOrden_acepta_lista_blanca(string solicitado, string esperado)
    {
        var permitido = IdentificadoresSql.TryObtenerColumnaOrden(solicitado, out var columna);

        Assert.True(permitido);
        Assert.Equal(esperado, columna);
    }

    [Theory]
    [InlineData("Id; DROP TABLE Vehiculos")]
    [InlineData("placa DESC; --")]
    [InlineData("(SELECT 1)")]
    [InlineData("")]
    public void TryObtenerColumnaOrden_rechaza_identificadores_externos(string solicitado)
    {
        var permitido = IdentificadoresSql.TryObtenerColumnaOrden(solicitado, out var columna);

        Assert.False(permitido);
        Assert.Equal("Id", columna);
    }

    [Fact]
    public void ObtenerDireccionOrden_solo_permite_asc_o_desc()
    {
        Assert.Equal("ASC", IdentificadoresSql.ObtenerDireccionOrden("asc"));
        Assert.Equal("DESC", IdentificadoresSql.ObtenerDireccionOrden("desc"));
        Assert.Equal("ASC", IdentificadoresSql.ObtenerDireccionOrden("DESC; DROP TABLE"));
    }

    [Fact]
    public void EsEstadoPermitido_usa_lista_blanca()
    {
        Assert.True(IdentificadoresSql.EsEstadoPermitido("Activo"));
        Assert.False(IdentificadoresSql.EsEstadoPermitido("Activo; DROP TABLE Vehiculos"));
    }
}
