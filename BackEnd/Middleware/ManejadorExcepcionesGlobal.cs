using ControlVehicular.Data;
using ControlVehicular.Errores;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ControlVehicular.Middleware;

public sealed class ManejadorExcepcionesGlobal : IExceptionHandler
{
    private readonly ILogger<ManejadorExcepcionesGlobal> _logger;

    public ManejadorExcepcionesGlobal(ILogger<ManejadorExcepcionesGlobal> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, titulo) = exception switch
        {
            ErrorValidacionNegocioException => (StatusCodes.Status422UnprocessableEntity, "Datos no procesables"),
            RecursoDuplicadoException => (StatusCodes.Status409Conflict, "Conflicto"),
            RecursoNoAutorizadoException => (StatusCodes.Status404NotFound, "No encontrado"),
            AccesoDatosException => (StatusCodes.Status500InternalServerError, "Error interno"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Solicitud invalida"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno")
        };

        if (status >= 500)
        {
            _logger.LogError(exception, "Error no controlado. TraceId={TraceId}", httpContext.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(exception, "Error de cliente. TraceId={TraceId}", httpContext.TraceIdentifier);
        }

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Type = $"https://httpstatuses.com/{status}",
            Detail = status is StatusCodes.Status400BadRequest or StatusCodes.Status409Conflict or StatusCodes.Status422UnprocessableEntity
                ? exception.Message
                : "No se pudo completar la operacion.",
            Instance = httpContext.Request.Path
        };
        problema.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problema, cancellationToken);
        return true;
    }
}
