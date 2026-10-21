using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ControlVehicular.Configuration;

public static class ValidadorConfiguracion
{
    private static readonly HashSet<string> ClavesJwtDeEjemplo = new(StringComparer.Ordinal)
    {
        "ClaveSuperSecretaDeAlMenos32Caracteres!!"
    };

    public static void ValidarJwt(JwtOpciones jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt.Emisor) || string.IsNullOrWhiteSpace(jwt.Audiencia))
        {
            throw new InvalidOperationException("Jwt:Emisor y Jwt:Audiencia son obligatorios.");
        }

        if (string.IsNullOrWhiteSpace(jwt.Clave) || jwt.Clave.Length < JwtOpciones.LongitudMinimaClave)
        {
            throw new InvalidOperationException("Jwt:Clave debe configurarse fuera del repositorio con al menos 32 caracteres.");
        }

        if (ClavesJwtDeEjemplo.Contains(jwt.Clave))
        {
            throw new InvalidOperationException("Jwt:Clave no puede ser una clave de ejemplo. Genere un secreto aleatorio.");
        }

        var material = Encoding.UTF8.GetBytes(jwt.Clave);
        var claveSimetrica = new SymmetricSecurityKey(material);
        if (claveSimetrica.KeySize < 256)
        {
            throw new InvalidOperationException("Jwt:Clave no tiene entropia suficiente para HMAC-SHA256 (minimo 256 bits).");
        }

        if (jwt.Clave.Distinct().Count() < 8)
        {
            throw new InvalidOperationException("Jwt:Clave tiene entropia insuficiente. Use un valor aleatorio.");
        }

        if (jwt.ExpiracionMinutos is < 1 or > 1440)
        {
            throw new InvalidOperationException("Jwt:ExpiracionMinutos esta fuera de rango.");
        }
    }

    public static void ValidarCors(CorsOpciones cors, bool produccion)
    {
        if (cors.OrigenesPermitidos.Any(origen => origen == "*"))
        {
            throw new InvalidOperationException("CORS no admite origenes con *.");
        }

        if (produccion && cors.OrigenesPermitidos.Length == 0)
        {
            throw new InvalidOperationException("En produccion debe configurar Cors:OrigenesPermitidos.");
        }
    }

    public static void ValidarAuth(AuthOpciones auth)
    {
        if (string.IsNullOrWhiteSpace(auth.Usuario) || string.IsNullOrWhiteSpace(auth.PasswordHash))
        {
            throw new InvalidOperationException("Auth:Usuario y Auth:PasswordHash deben configurarse como secretos (User Secrets o variables de entorno).");
        }
    }
}
