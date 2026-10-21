namespace ControlVehicular.Middleware;

public sealed class MiddlewareCorrelacion
{
    public const string Encabezado = "X-Correlation-ID";
    private readonly RequestDelegate _siguiente;

    public MiddlewareCorrelacion(RequestDelegate siguiente)
    {
        _siguiente = siguiente;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        var correlacion = contexto.Request.Headers.TryGetValue(Encabezado, out var valor)
            && Guid.TryParse(valor.ToString(), out var guid)
            ? guid.ToString("D")
            : contexto.TraceIdentifier;

        contexto.TraceIdentifier = correlacion;
        contexto.Response.OnStarting(() =>
        {
            contexto.Response.Headers[Encabezado] = correlacion;
            return Task.CompletedTask;
        });

        await _siguiente(contexto);
    }
}
