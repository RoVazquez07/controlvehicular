using ControlVehicular.Security;

namespace ControlVehicular.Tests;

public class ValidadorUrlExternaTests
{
    [Theory]
    [InlineData("http://ejemplo.com/x")]
    [InlineData("https://127.0.0.1/x")]
    [InlineData("https://localhost/x")]
    [InlineData("https://192.168.1.10/x")]
    [InlineData("file:///etc/passwd")]
    [InlineData("https://[::ffff:192.168.1.1]/x")]
    [InlineData("https://[fc00::1]/x")]
    [InlineData("https://[fe80::1]/x")]
    public void Rechaza_urls_inseguras(string url)
    {
        Assert.False(ValidadorUrlExterna.EsUrlRemotaPermitida(url));
    }

    [Fact]
    public void Acepta_https_publico_literal()
    {
        Assert.True(ValidadorUrlExterna.EsUrlRemotaPermitida("https://example.com/recurso"));
    }
}
