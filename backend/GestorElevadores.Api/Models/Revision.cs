using System.ComponentModel.DataAnnotations;

namespace GestorElevadores.Api.Models;

public enum ResultadoRevision
{
    Favorable,
    ConDefectosLeves,
    Desfavorable // el elevador no debe usarse hasta repararlo
}

/// <summary>
/// Revisión periódica (mantenimiento preventivo) de un elevador.
/// </summary>
public class Revision
{
    public int Id { get; set; }

    public DateOnly Fecha { get; set; }

    [Required, MaxLength(100)]
    public string Tecnico { get; set; } = string.Empty;

    public ResultadoRevision Resultado { get; set; }

    [MaxLength(2000)]
    public string? Observaciones { get; set; }

    public int ElevadorId { get; set; }
    public Elevador? Elevador { get; set; }
}
