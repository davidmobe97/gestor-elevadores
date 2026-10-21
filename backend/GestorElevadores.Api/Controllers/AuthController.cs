using System.Security.Claims;
using GestorElevadores.Api.Data;
using GestorElevadores.Api.Dtos;
using GestorElevadores.Api.Models;
using GestorElevadores.Api.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorElevadores.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    AppDbContext db,
    IPasswordHasher<Usuario> hasher,
    GeneradorTokens tokens) : ControllerBase
{
    /// <summary>Inicia sesión y devuelve el token JWT.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<SesionDto>> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Activo);

        // Mismo mensaje si no existe el usuario o la contraseña es mala:
        // así no se revela qué emails están registrados.
        if (usuario is null ||
            hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });
        }

        var (token, expira) = tokens.Crear(usuario);
        return new SesionDto(token, expira, usuario.Nombre, usuario.Email, usuario.Rol.ToString());
    }

    /// <summary>Datos del usuario de la sesión actual (sirve para comprobar que el token sigue siendo válido).</summary>
    [HttpGet("yo")]
    [Authorize]
    public ActionResult<UsuarioActualDto> Yo() => new UsuarioActualDto(
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0"),
        User.Identity?.Name ?? "",
        User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email") ?? "",
        User.FindFirstValue(ClaimTypes.Role) ?? "");
}
