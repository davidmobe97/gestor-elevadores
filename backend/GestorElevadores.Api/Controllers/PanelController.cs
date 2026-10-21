using GestorElevadores.Api.Data;
using GestorElevadores.Api.Dtos;
using GestorElevadores.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorElevadores.Api.Controllers;

/// <summary>
/// Datos resumidos para la pantalla de inicio.
/// </summary>
[ApiController]
[Route("api/panel")]
public class PanelController(AppDbContext db, TimeProvider reloj) : ControllerBase
{
    [HttpGet]
    public async Task<PanelResumenDto> Obtener()
    {
        var hoy = DateOnly.FromDateTime(reloj.GetUtcNow().UtcDateTime);
        var dentroDe30Dias = hoy.AddDays(30);

        var elevadores = (await db.Elevadores.AsNoTracking()
                .Include(e => e.Cliente)
                .Include(e => e.Revisiones)
                .Include(e => e.Incidencias)
                .ToListAsync())
            .Select(e => e.AResumen(hoy))
            .ToList();

        var abiertas = db.Incidencias.Where(i => i.Estado != EstadoIncidencia.Cerrada);

        return new PanelResumenDto(
            TotalClientes: await db.Clientes.CountAsync(),
            TotalElevadores: elevadores.Count,
            IncidenciasAbiertas: await abiertas.CountAsync(),
            IncidenciasUrgentes: await abiertas.CountAsync(i => i.Prioridad == PrioridadIncidencia.Urgente),
            RevisionesVencidas: elevadores.Count(e => e.RevisionVencida),
            RevisionesProximos30Dias: elevadores.Count(e => !e.RevisionVencida && e.ProximaRevision <= dentroDe30Dias),
            ProximasRevisiones: elevadores.OrderBy(e => e.ProximaRevision).Take(5).ToList());
    }
}
