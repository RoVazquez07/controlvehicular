using System.Text;
using System.Threading.RateLimiting;
using ControlVehicular.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace ControlVehicular.Security;

public static class ExtensionesSeguridad
{
    public static IServiceCollection AddPoliticaCors(this IServiceCollection services, IConfiguration configuration, IHostEnvironment entorno)
    {
        var cors = configuration.GetSection(CorsOpciones.Seccion).Get<CorsOpciones>() ?? new CorsOpciones();
        ValidadorConfiguracion.ValidarCors(cors, entorno.IsProduction());

        services.AddCors(opciones =>
        {
            opciones.AddPolicy(CorsOpciones.NombrePolitica, politica =>
            {
                if (cors.OrigenesPermitidos.Length == 0)
                {
                    politica.SetIsOriginAllowed(_ => false);
                    return;
                }

                politica
                    .WithOrigins(cors.OrigenesPermitidos)
                    .WithHeaders("Authorization", "Content-Type", "X-Correlation-ID")
                    .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .DisallowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddAutenticacionJwt(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment entorno)
    {
        var jwt = configuration.GetSection(JwtOpciones.Seccion).Get<JwtOpciones>()
            ?? throw new InvalidOperationException("Falta la seccion Jwt en appsettings.");
        ValidadorConfiguracion.ValidarJwt(jwt);

        var auth = configuration.GetSection(AuthOpciones.Seccion).Get<AuthOpciones>() ?? new AuthOpciones();
        ValidadorConfiguracion.ValidarAuth(auth);
        if (auth.Rol is not (RolesAplicacion.Administrador or RolesAplicacion.Operador))
        {
            throw new InvalidOperationException("Auth:Rol debe ser Administrador u Operador.");
        }

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave));

        services
            .AddAuthentication(opciones =>
            {
                opciones.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opciones.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opciones =>
            {
                opciones.RequireHttpsMetadata = !entorno.IsDevelopment();
                opciones.MapInboundClaims = true;
                opciones.IncludeErrorDetails = entorno.IsDevelopment();
                opciones.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = _ => Task.CompletedTask,
                    OnChallenge = _ => Task.CompletedTask
                };
                opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    ValidIssuer = jwt.Emisor,
                    ValidAudience = jwt.Audiencia,
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    IssuerSigningKey = clave,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }

    public static IServiceCollection AddAutorizacionAplicacion(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, AutorizadorVehiculoHandler>();
        services.AddAuthorization(opciones =>
        {
            opciones.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            opciones.AddPolicy(Politicas.ConsultarVehiculos, politica =>
                politica.RequireRole(RolesAplicacion.Administrador, RolesAplicacion.Operador));
            opciones.AddPolicy(Politicas.RegistrarVehiculos, politica =>
                politica.RequireRole(RolesAplicacion.Administrador, RolesAplicacion.Operador));
            opciones.AddPolicy(Politicas.CambiarEstadoVehiculos, politica =>
                politica.RequireRole(RolesAplicacion.Administrador, RolesAplicacion.Operador));
            opciones.AddPolicy(Politicas.RecursoVehiculo, politica =>
                politica.AddRequirements(new RequisitoRecursoVehiculo()));
        });

        return services;
    }

    public static IServiceCollection AddLimitacionSolicitudes(this IServiceCollection services, IConfiguration configuration)
    {
        var limites = configuration.GetSection(LimitesOpciones.Seccion).Get<LimitesOpciones>() ?? new LimitesOpciones();

        services.AddRateLimiter(opciones =>
        {
            opciones.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            opciones.OnRejected = async (contexto, token) =>
            {
                contexto.HttpContext.Response.ContentType = "application/problem+json";
                await contexto.HttpContext.Response.WriteAsJsonAsync(new
                {
                    type = "https://httpstatuses.com/429",
                    title = "Demasiadas solicitudes",
                    status = 429,
                    traceId = contexto.HttpContext.TraceIdentifier
                }, token);
            };

            opciones.AddPolicy("login", contexto =>
                RateLimitPartition.GetFixedWindowLimiter(
                    contexto.Connection.RemoteIpAddress?.ToString() ?? "anonimo",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = limites.LoginIntentosPorMinuto,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            opciones.AddPolicy("api", contexto =>
                RateLimitPartition.GetFixedWindowLimiter(
                    contexto.User.Identity?.Name ?? contexto.Connection.RemoteIpAddress?.ToString() ?? "anonimo",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = limites.ApiSolicitudesPorMinuto,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
        });

        return services;
    }

    public static IServiceCollection AddHashContrasenas(this IServiceCollection services)
    {
        services.AddSingleton(_ => new PasswordHasher<object>(
            Microsoft.Extensions.Options.Options.Create(new PasswordHasherOptions
            {
                IterationCount = 210_000
            })));
        return services;
    }

    public static void AddDefinicionJwtSwagger(this Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions opciones)
    {
        opciones.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Token JWT emitido por /auth/login."
        });

        opciones.AddSecurityRequirement(documento => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("bearer", documento)] = []
        });
    }
}
