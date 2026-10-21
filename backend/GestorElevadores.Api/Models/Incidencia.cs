using System.ComponentModel.DataAnnotations;

namespace GestorElevadores.Api.Models;

public enum PrioridadIncidencia
{
    Baja,
    Media,
    Alta,
    Urgente // elevador parado o inseguro
}

public enum EstadoIncidencia
{
    Abierta,
    EnCurso,
    Cerrada
}

/// <summary>
/// Avería o aviso de un cliente sobre un elevador concreto.
/// </summary>
public class Incidencia
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Descripcion { get; set; }

    public PrioridadIncidencia Prioridad { get; set; } = PrioridadIncidencia.Media;
    public EstadoIncidencia Estado { get; set; } = EstadoIncidencia.Abierta;

    [MaxLength(100)]
    public string? TecnicoAsignado { get; set; }

    public DateTime FechaApertura { get; set; } = DateTime.UtcNow;
    public DateTime? FechaCierre { get; set; }

    [MaxLength(2000)]
    public string? Solucion { get; set; }

    public int ElevadorId { get; set; }
    public Elevador? Elevador { get; set; }

    /// <summary>
    /// Cierra la incidencia guardando la solución aplicada.
    /// </summary>
    public void Cerrar(string solucion, DateTime fecha)
    {
        if (Estado == EstadoIncidencia.Cerrada)
            throw new InvalidOperationException("La incidencia ya está cerrada.");

        if (string.IsNullOrWhiteSpace(solucion))
            throw new ArgumentException("Hay que indicar la solución aplicada.", nameof(solucion));

        Estado = EstadoIncidencia.Cerrada;
        Solucion = solucion.Trim();
        FechaCierre = fecha;
    }
}
