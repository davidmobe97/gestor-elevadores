using System.ComponentModel.DataAnnotations;

namespace GestorElevadores.Api.Dtos;

// Los DTO (Data Transfer Objects) son lo que entra y sale por la API.
// Separarlos de los modelos evita exponer la base de datos tal cual
// y que alguien pueda modificar campos que no debe (por ejemplo, el Id).

public record ClienteResumenDto(
    int Id,
    string NombreTaller,
    string Poblacion,
    string Provincia,
    string? Telefono,
    int NumeroElevadores);

public record ClienteDetalleDto(
    int Id,
    string NombreTaller,
    string? Cif,
    string? PersonaContacto,
    string? Telefono,
    string? Email,
    string? Direccion,
    string Poblacion,
    string Provincia,
    DateTime FechaAlta,
    List<ElevadorResumenDto> Elevadores);

public record ClienteGuardarDto(
    [Required, MaxLength(150)] string NombreTaller,
    [MaxLength(20)] string? Cif,
    [MaxLength(100)] string? PersonaContacto,
    [MaxLength(20)] string? Telefono,
    [EmailAddress, MaxLength(150)] string? Email,
    [MaxLength(200)] string? Direccion,
    [Required, MaxLength(100)] string Poblacion,
    [Required, MaxLength(100)] string Provincia);
