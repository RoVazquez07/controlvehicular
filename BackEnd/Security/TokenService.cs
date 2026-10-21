using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ControlVehicular.Configuration;
using ControlVehicular.DTOs;
using ControlVehicular.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ControlVehicular.Security;

public sealed class TokenService : ITokenService
{
    private readonly JwtOpciones _jwt;
    private readonly AuthOpciones _auth;
    private readonly PasswordHasher<object> _hasher;

    public TokenService(IOptions<JwtOpciones> jwt, IOptions<AuthOpciones> auth, PasswordHasher<object> hasher)
    {
        _jwt = jwt.Value;
        _auth = auth.Value;
        _hasher = hasher;
    }

    public LoginRespuesta? Autenticar(LoginSolicitud solicitud)
    {
        if (!string.Equals(solicitud.Usuario, _auth.Usuario, StringComparison.Ordinal))
        {
            VerificarDummy();
            return null;
        }

        var resultado = _hasher.VerifyHashedPassword(new object(), _auth.PasswordHash, solicitud.Password);
        if (resultado == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var expiracion = DateTime.UtcNow.AddMinutes(_jwt.ExpiracionMinutos);
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Clave));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, _auth.Usuario),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, _auth.Usuario),
            new(ClaimTypes.Role, _auth.Rol)
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Emisor,
            audience: _jwt.Audiencia,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiracion,
            signingCredentials: credenciales);

        return new LoginRespuesta
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiracionUtc = expiracion
        };
    }

    private void VerificarDummy()
    {
        _hasher.VerifyHashedPassword(
            new object(),
            _auth.PasswordHash,
            "intento-no-valido");
    }
}
