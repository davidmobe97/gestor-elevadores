using System.ComponentModel.DataAnnotations;
using GestorElevadores.Api.Models;

namespace GestorElevadores.Api.Dtos;

public record IncidenciaDto(
    int Id,
    string Titulo,
    string? Descripcion,
    PrioridadIncidencia Prioridad,
    EstadoIncidencia Estado,
    string? TecnicoAsignado,
    DateTime FechaApertura,
    DateTime? FechaCierre,
    string? Solucion,
    int ElevadorId,
    string ElevadorNombre,
    string NombreTaller);

public record IncidenciaCrearDto(
    [Range(1, int.MaxValue)] int ElevadorId,
    [Required, MaxLength(150)] string Titulo,
    [MaxLength(2000)] string? Descripcion,
    PrioridadIncidencia Prioridad,
    [MaxLength(100)] string? TecnicoAsignado);

public record IncidenciaCerrarDto(
    [Required, MaxLength(2000)] string Solucion);

public record RevisionDto(
    int Id,
    DateOnly Fecha,
    string Tecnico,
    ResultadoRevision Resultado,
    string? Observaciones,
    int ElevadorId);

public record RevisionCrearDto(
    [Range(1, int.MaxValue)] int ElevadorId,
    DateOnly Fecha,
    [Required, MaxLength(100)] string Tecnico,
    ResultadoRevision Resultado,
    [MaxLength(2000)] string? Observaciones);

public record PanelResumenDto(
    int TotalClientes,
    int TotalElevadores,
    int IncidenciasAbiertas,
    int IncidenciasUrgentes,
    int RevisionesVencidas,
    int RevisionesProximos30Dias,
    List<ElevadorResumenDto> ProximasRevisiones);
