using GestorElevadores.Api.Data;
using GestorElevadores.Api.Dtos;
using GestorElevadores.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorElevadores.Api.Controllers;

[ApiController]
[Route("api/incidencias")]
public class IncidenciasController(AppDbContext db, TimeProvider reloj) : ControllerBase
{
    /// <summary>Lista de incidencias, filtrable por estado. Las urgentes salen primero.</summary>
    [HttpGet]
    public async Task<List<IncidenciaDto>> Listar([FromQuery] EstadoIncidencia? estado)
    {
        var consulta = db.Incidencias.AsNoTracking()
            .Include(i => i.Elevador).ThenInclude(e => e!.Cliente)
            .AsQueryable();

        if (estado is not null)
            consulta = consulta.Where(i => i.Estado == estado);

        var lista = await consulta.ToListAsync();

        return lista
            .OrderBy(i => i.Estado == EstadoIncidencia.Cerrada) // abiertas primero
            .ThenByDescending(i => i.Prioridad)                 // y de más a menos urgente
            .ThenByDescending(i => i.FechaApertura)
            .Select(i => i.ADto())
            .ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IncidenciaDto>> Obtener(int id)
    {
        var i = await db.Incidencias.AsNoTracking()
            .Include(x => x.Elevador).ThenInclude(e => e!.Cliente)
            .FirstOrDefaultAsync(x => x.Id == id);

        return i is null ? NotFound() : i.ADto();
    }

    [HttpPost]
    public async Task<ActionResult<IncidenciaDto>> Crear(IncidenciaCrearDto dto)
    {
        if (!await db.Elevadores.AnyAsync(e => e.Id == dto.ElevadorId))
            return BadRequest(new { mensaje = $"No existe el elevador {dto.ElevadorId}." });

        var incidencia = new Incidencia
        {
            ElevadorId = dto.ElevadorId,
            Titulo = dto.Titulo.Trim(),
            Descripcion = dto.Descripcion?.Trim(),
            Prioridad = dto.Prioridad,
            TecnicoAsignado = dto.TecnicoAsignado?.Trim(),
            Estado = string.IsNullOrWhiteSpace(dto.TecnicoAsignado) ? EstadoIncidencia.Abierta : EstadoIncidencia.EnCurso,
            FechaApertura = reloj.GetUtcNow().UtcDateTime
        };

        db.Incidencias.Add(incidencia);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obtener), new { id = incidencia.Id }, (await Obtener(incidencia.Id)).Value);
    }

    /// <summary>Cierra una incidencia indicando la solución aplicada.</summary>
    [HttpPost("{id:int}/cerrar")]
    public async Task<IActionResult> Cerrar(int id, IncidenciaCerrarDto dto)
    {
        var incidencia = await db.Incidencias.FindAsync(id);
        if (incidencia is null) return NotFound();

        try
        {
            // La regla de negocio vive en el modelo (Incidencia.Cerrar), no aquí.
            incidencia.Cerrar(dto.Solucion, reloj.GetUtcNow().UtcDateTime);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }

        await db.SaveChangesAsync();
        return NoContent();
    }
}
