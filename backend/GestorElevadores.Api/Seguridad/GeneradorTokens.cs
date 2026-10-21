using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestorElevadores.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GestorElevadores.Api.Seguridad;

/// <summary>
/// Crea el token JWT que el frontend envía en cada petición
/// (cabecera "Authorization: Bearer ...").
/// </summary>
public class GeneradorTokens(IOptions<JwtOpciones> opciones, TimeProvider reloj)
{
    private readonly JwtOpciones _op = opciones.Value;

    public (string Token, DateTime Expira) Crear(Usuario usuario)
    {
        var ahora = reloj.GetUtcNow().UtcDateTime;
        var expira = ahora.AddMinutes(_op.MinutosValidez);

        // Los "claims" son los datos que viajan dentro del token (firmados, no cifrados:
        // por eso nunca se mete información sensible).
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_op.Clave)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _op.Emisor,
            audience: _op.Audiencia,
            claims: claims,
            notBefore: ahora,
            expires: expira,
            signingCredentials: credenciales);

        return (new JwtSecurityTokenHandler().WriteToken(token), expira);
    }
}
