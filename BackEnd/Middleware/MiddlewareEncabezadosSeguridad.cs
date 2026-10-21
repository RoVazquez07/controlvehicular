namespace ControlVehicular.Middleware;

public sealed class MiddlewareEncabezadosSeguridad
{
    private readonly RequestDelegate _siguiente;

    public MiddlewareEncabezadosSeguridad(RequestDelegate siguiente)
    {
        _siguiente = siguiente;
    }

    public Task InvokeAsync(HttpContext contexto)
    {
        contexto.Response.OnStarting(() =>
        {
            var headers = contexto.Response.Headers;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "no-referrer";
            headers["X-Permitted-Cross-Domain-Policies"] = "none";
            headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'; base-uri 'none'";
            headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
            headers["Cache-Control"] = "no-store";
            return Task.CompletedTask;
        });

        return _siguiente(contexto);
    }
}
