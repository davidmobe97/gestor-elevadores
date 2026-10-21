using System.ComponentModel.DataAnnotations;

namespace GestorElevadores.Api.Models;

public enum TipoElevador
{
    DosColumnas,
    CuatroColumnas,
    Tijera,
    Parking
}

/// <summary>
/// Elevador de automoción instalado en el taller de un cliente.
/// </summary>
public class Elevador
{
    /// <summary>Por normativa, cada elevador necesita una revisión al año.</summary>
    public const int MesesEntreRevisiones = 12;

    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Marca { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Modelo { get; set; } = string.Empty;

    [Required, MaxLength(60)]
    public string NumeroSerie { get; set; } = string.Empty;

    public TipoElevador Tipo { get; set; }

    [Range(500, 20000)]
    public int CapacidadKg { get; set; }

    public DateOnly FechaInstalacion { get; set; }

    // Clave foránea + navegación al cliente.
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public List<Incidencia> Incidencias { get; set; } = [];
    public List<Revision> Revisiones { get; set; } = [];

    /// <summary>
    /// Fecha en la que toca la siguiente revisión: un año después de la última
    /// revisión o, si no tiene ninguna, un año después de la instalación.
    /// </summary>
    public DateOnly CalcularProximaRevision()
    {
        var ultima = Revisiones.Count > 0
            ? Revisiones.Max(r => r.Fecha)
            : FechaInstalacion;

        return ultima.AddMonths(MesesEntreRevisiones);
    }
}
