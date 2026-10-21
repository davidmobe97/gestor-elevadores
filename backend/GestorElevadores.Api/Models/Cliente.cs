using System.ComponentModel.DataAnnotations;

namespace GestorElevadores.Api.Models;

/// <summary>
/// Taller o empresa donde hay elevadores instalados.
/// </summary>
public class Cliente
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string NombreTaller { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Cif { get; set; }

    [MaxLength(100)]
    public string? PersonaContacto { get; set; }

    [MaxLength(20)]
    public string? Telefono { get; set; }

    [EmailAddress, MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(200)]
    public string? Direccion { get; set; }

    [Required, MaxLength(100)]
    public string Poblacion { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Provincia { get; set; } = string.Empty;

    public DateTime FechaAlta { get; set; } = DateTime.UtcNow;

    // Relación 1 a N: un cliente tiene varios elevadores.
    public List<Elevador> Elevadores { get; set; } = [];
}
