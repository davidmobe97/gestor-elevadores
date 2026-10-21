using System.ComponentModel.DataAnnotations;

namespace GestorElevadores.Api.Models;

public enum RolUsuario
{
    Administrador, // gestiona clientes y elevadores
    Tecnico        // registra revisiones e incidencias
}

/// <summary>
/// Persona que usa la aplicación. La contraseña NUNCA se guarda en claro:
/// solo su hash (PasswordHasher de ASP.NET Core Identity, PBKDF2 con sal).
/// </summary>
public class Usuario
{
    public int Id { get; set; }

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public RolUsuario Rol { get; set; } = RolUsuario.Tecnico;

    public bool Activo { get; set; } = true;
}
