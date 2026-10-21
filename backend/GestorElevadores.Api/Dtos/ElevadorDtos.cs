using System.ComponentModel.DataAnnotations;
using GestorElevadores.Api.Models;

namespace GestorElevadores.Api.Dtos;

public record ElevadorResumenDto(
    int Id,
    string Marca,
    string Modelo,
    string NumeroSerie,
    TipoElevador Tipo,
    int ClienteId,
    string NombreTaller,
    DateOnly ProximaRevision,
    bool RevisionVencida,
    int IncidenciasAbiertas);

public record ElevadorDetalleDto(
    int Id,
    string Marca,
    string Modelo,
    string NumeroSerie,
    TipoElevador Tipo,
    int CapacidadKg,
    DateOnly FechaInstalacion,
    int ClienteId,
    string NombreTaller,
    DateOnly ProximaRevision,
    bool RevisionVencida,
    List<RevisionDto> Revisiones,
    List<IncidenciaDto> Incidencias);

public record ElevadorGuardarDto(
    [Required, MaxLength(80)] string Marca,
    [Required, MaxLength(80)] string Modelo,
    [Required, MaxLength(60)] string NumeroSerie,
    TipoElevador Tipo,
    [Range(500, 20000)] int CapacidadKg,
    DateOnly FechaInstalacion,
    [Range(1, int.MaxValue)] int ClienteId);
