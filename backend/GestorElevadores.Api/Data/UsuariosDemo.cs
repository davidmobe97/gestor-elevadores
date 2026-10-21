using GestorElevadores.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestorElevadores.Api.Data;

/// <summary>
/// Crea los usuarios de demostración si la tabla está vacía.
/// Los datos salen de la configuración (sección "UsuariosDemo"), no del código,
/// y solo existen en desarrollo.
/// </summary>
public static class UsuariosDemo
{
    public record Semilla(string Email, string Nombre, string Password, RolUsuario Rol);

    public static async Task CrearSiNoHayAsync(IServiceProvider servicios, IConfiguration config)
    {
        var semillas = config.GetSection("UsuariosDemo").Get<Semilla[]>() ?? [];
        if (semillas.Length == 0) return;

        var db = servicios.GetRequiredService<AppDbContext>();
        if (await db.Usuarios.AnyAsync()) return;

        var hasher = servicios.GetRequiredService<IPasswordHasher<Usuario>>();
        foreach (var s in semillas)
        {
            var u = new Usuario { Email = s.Email.ToLowerInvariant(), Nombre = s.Nombre, Rol = s.Rol };
            u.PasswordHash = hasher.HashPassword(u, s.Password);
            db.Usuarios.Add(u);
        }
        await db.SaveChangesAsync();
    }
}
