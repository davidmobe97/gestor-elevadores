using GestorElevadores.Api.Models;

namespace GestorElevadores.Api.Dtos;

/// <summary>
/// Conversiones de modelo a DTO. Están en un único sitio para no repetirlas
/// en cada controlador.
/// </summary>
public static class Mapeos
{
    public static ElevadorResumenDto AResumen(this Elevador e, DateOnly hoy)
    {
        var proxima = e.CalcularProximaRevision();
        return new ElevadorResumenDto(
            e.Id, e.Marca, e.Modelo, e.NumeroSerie, e.Tipo,
            e.ClienteId, e.Cliente?.NombreTaller ?? "",
            proxima, proxima < hoy,
            e.Incidencias.Count(i => i.Estado != EstadoIncidencia.Cerrada));
    }

    public static IncidenciaDto ADto(this Incidencia i) => new(
        i.Id, i.Titulo, i.Descripcion, i.Prioridad, i.Estado, i.TecnicoAsignado,
        i.FechaApertura, i.FechaCierre, i.Solucion, i.ElevadorId,
        i.Elevador is null ? "" : $"{i.Elevador.Marca} {i.Elevador.Modelo}",
        i.Elevador?.Cliente?.NombreTaller ?? "");

    public static RevisionDto ADto(this Revision r) =>
        new(r.Id, r.Fecha, r.Tecnico, r.Resultado, r.Observaciones, r.ElevadorId);
}
