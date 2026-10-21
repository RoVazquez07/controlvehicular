namespace ControlVehicular.Tests;

public sealed class RequiereSqlServerFactAttribute : FactAttribute
{
    public RequiereSqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CONTROLVEHICULAR_TEST_SQL")))
        {
            Skip = "Defina CONTROLVEHICULAR_TEST_SQL con una cadena Encrypt=True para pruebas de integracion.";
        }
    }
}
