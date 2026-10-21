using GestorElevadores.Api.Data;
using GestorElevadores.Api.Dtos;
using GestorElevadores.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorElevadores.Api.Controllers;

[ApiController]
[Route("api/revisiones")]
public class RevisionesController(AppDbContext db, TimeProvider reloj) : ControllerBase
{
    /// <summary>Registra la revisión anual de un elevador.</summary>
    [HttpPost]
    public async Task<ActionResult<RevisionDto>> Crear(RevisionCrearDto dto)
    {
        if (!await db.Elevadores.AnyAsync(e => e.Id == dto.ElevadorId))
            return BadRequest(new { mensaje = $"No existe el elevador {dto.ElevadorId}." });

        var hoy = DateOnly.FromDateTime(reloj.GetUtcNow().UtcDateTime);
        if (dto.Fecha > hoy)
            return BadRequest(new { mensaje = "No se puede registrar una revisión con fecha futura." });

        var revision = new Revision
        {
            ElevadorId = dto.ElevadorId,
            Fecha = dto.Fecha,
            Tecnico = dto.Tecnico.Trim(),
            Resultado = dto.Resultado,
            Observaciones = dto.Observaciones?.Trim()
        };

        db.Revisiones.Add(revision);
        await db.SaveChangesAsync();

        return Created($"/api/revisiones/{revision.Id}", revision.ADto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Borrar(int id)
    {
        var revision = await db.Revisiones.FindAsync(id);
        if (revision is null) return NotFound();

        db.Revisiones.Remove(revision);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
