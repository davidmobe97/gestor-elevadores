using System.ComponentModel.DataAnnotations;

namespace GestorElevadores.Api.Dtos;

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record SesionDto(
    string Token,
    DateTime Expira,
    string Nombre,
    string Email,
    string Rol);

public record UsuarioActualDto(
    int Id,
    string Nombre,
    string Email,
    string Rol);
