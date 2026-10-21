using ControlVehicular.Configuration;
using ControlVehicular.Data;
using ControlVehicular.Data.Repositorios;
using ControlVehicular.Interfaces;
using ControlVehicular.Middleware;
using ControlVehicular.Security;
using ControlVehicular.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
// Secretos: User Secrets (Development, UserSecretsId) y variables de entorno
// (Jwt__Clave, Auth__PasswordHash, SqlServer__CadenaConexion). Azure Key Vault
// se puede registrar en el host de despliegue sobre este mismo IConfiguration.
var limites = builder.Configuration.GetSection(LimitesOpciones.Seccion).Get<LimitesOpciones>() ?? new LimitesOpciones();

builder.WebHost.ConfigureKestrel(opciones =>
{
    opciones.Limits.MaxRequestBodySize = limites.TamanoMaximoSolicitudBytes;
});

builder.Services.Configure<JwtOpciones>(builder.Configuration.GetSection(JwtOpciones.Seccion));
builder.Services.Configure<AuthOpciones>(builder.Configuration.GetSection(AuthOpciones.Seccion));
builder.Services.Configure<SqlServerOpciones>(builder.Configuration.GetSection(SqlServerOpciones.Seccion));
builder.Services.Configure<CorsOpciones>(builder.Configuration.GetSection(CorsOpciones.Seccion));
builder.Services.Configure<LimitesOpciones>(builder.Configuration.GetSection(LimitesOpciones.Seccion));
builder.Services.Configure<ForwardedHeadersOptions>(opciones =>
{
    opciones.ForwardedHeaders = ForwardedHeaders.None;
});

builder.Services.AddProblemDetails(opciones =>
{
    opciones.CustomizeProblemDetails = contexto =>
    {
        contexto.ProblemDetails.Extensions["traceId"] = contexto.HttpContext.TraceIdentifier;
    };
});
builder.Services.AddExceptionHandler<ManejadorExcepcionesGlobal>();
builder.Services.AddMemoryCache();
builder.Services.AddHashContrasenas();
builder.Services.AddSingleton<IProteccionLogin, ProteccionLogin>();
builder.Services.AddHealthChecks();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opciones =>
    {
        opciones.InvalidModelStateResponseFactory = contexto =>
        {
            var problema = new ValidationProblemDetails(contexto.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Solicitud invalida",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            };
            problema.Extensions["traceId"] = contexto.HttpContext.TraceIdentifier;
            return new BadRequestObjectResult(problema)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });

builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IVehiculoRepositorio, VehiculoRepositorio>();
builder.Services.AddScoped<ISaludoService, SaludoService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IVehiculoService, VehiculoService>();

builder.Services.AddPoliticaCors(builder.Configuration, builder.Environment);
builder.Services.AddAutenticacionJwt(builder.Configuration, builder.Environment);
builder.Services.AddAutorizacionAplicacion();
builder.Services.AddLimitacionSolicitudes(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(opciones =>
    {
        opciones.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ControlVehicular",
            Version = "v1"
        });
        opciones.AddDefinicionJwtSwagger();
    });
}

var app = builder.Build();

app.UseMiddleware<MiddlewareCorrelacion>();
app.UseMiddleware<MiddlewareEncabezadosSeguridad>();
app.UseExceptionHandler();

if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "ControlVehicular v1");
        opciones.RoutePrefix = "swagger";
    });
}

app.UseRouting();
app.UseCors(CorsOpciones.NombrePolitica);
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();
