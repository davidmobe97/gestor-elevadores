using GestorElevadores.Api.Data;
using GestorElevadores.Api.Dtos;
using GestorElevadores.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorElevadores.Api.Controllers;

[ApiController]
[Route("api/elevadores")]
public class ElevadoresController(AppDbContext db, TimeProvider reloj) : ControllerBase
{
    private DateOnly Hoy => DateOnly.FromDateTime(reloj.GetUtcNow().UtcDateTime);

    /// <summary>
    /// Lista de elevadores. Filtros opcionales: cliente y "solo con la revisión vencida".
    /// </summary>
    [HttpGet]
    public async Task<List<ElevadorResumenDto>> Listar([FromQuery] int? clienteId, [FromQuery] bool soloVencidos = false)
    {
        var consulta = db.Elevadores.AsNoTracking()
            .Include(e => e.Cliente)
            .Include(e => e.Revisiones)
            .Include(e => e.Incidencias)
            .AsQueryable();

        if (clienteId is not null)
            consulta = consulta.Where(e => e.ClienteId == clienteId);

        // La próxima revisión se calcula en C# (no en SQL), por eso primero se cargan.
        var hoy = Hoy;
        var lista = (await consulta.ToListAsync())
            .Select(e => e.AResumen(hoy))
            .Where(e => !soloVencidos || e.RevisionVencida)
            .OrderBy(e => e.ProximaRevision)
            .ToList();

        return lista;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ElevadorDetalleDto>> Obtener(int id)
    {
        var e = await db.Elevadores.AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Revisiones)
            .Include(x => x.Incidencias)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (e is null) return NotFound();

        foreach (var i in e.Incidencias) i.Elevador = e;
        var proxima = e.CalcularProximaRevision();

        return new ElevadorDetalleDto(
            e.Id, e.Marca, e.Modelo, e.NumeroSerie, e.Tipo, e.CapacidadKg, e.FechaInstalacion,
            e.ClienteId, e.Cliente!.NombreTaller, proxima, proxima < Hoy,
            e.Revisiones.OrderByDescending(r => r.Fecha).Select(r => r.ADto()).ToList(),
            e.Incidencias.OrderByDescending(i => i.FechaApertura).Select(i => i.ADto()).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<ElevadorDetalleDto>> Crear(ElevadorGuardarDto dto)
    {
        var error = await Validar(dto, idActual: null);
        if (error is not null) return error;

        var elevador = new Elevador();
        Copiar(dto, elevador);
        db.Elevadores.Add(elevador);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obtener), new { id = elevador.Id }, (await Obtener(elevador.Id)).Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, ElevadorGuardarDto dto)
    {
        var elevador = await db.Elevadores.FindAsync(id);
        if (elevador is null) return NotFound();

        var error = await Validar(dto, idActual: id);
        if (error is not null) return error;

        Copiar(dto, elevador);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Borrar(int id)
    {
        var elevador = await db.Elevadores.FindAsync(id);
        if (elevador is null) return NotFound();

        db.Elevadores.Remove(elevador); // sus incidencias y revisiones se borran en cascada
        await db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Comprueba que el cliente existe y que el nº de serie no está repetido.</summary>
    private async Task<ActionResult?> Validar(ElevadorGuardarDto dto, int? idActual)
    {
        if (!await db.Clientes.AnyAsync(c => c.Id == dto.ClienteId))
            return BadRequest(new { mensaje = $"No existe el cliente {dto.ClienteId}." });

        var repetido = await db.Elevadores.AnyAsync(e =>
            e.Id != idActual &&
            e.Marca == dto.Marca.Trim() &&
            e.NumeroSerie == dto.NumeroSerie.Trim());

        if (repetido)
            return Conflict(new { mensaje = "Ya existe un elevador de esa marca con ese número de serie." });

        if (dto.FechaInstalacion > Hoy)
            return BadRequest(new { mensaje = "La fecha de instalación no puede ser futura." });

        return null;
    }

    private static void Copiar(ElevadorGuardarDto dto, Elevador e)
    {
        e.Marca = dto.Marca.Trim();
        e.Modelo = dto.Modelo.Trim();
        e.NumeroSerie = dto.NumeroSerie.Trim();
        e.Tipo = dto.Tipo;
        e.CapacidadKg = dto.CapacidadKg;
        e.FechaInstalacion = dto.FechaInstalacion;
        e.ClienteId = dto.ClienteId;
    }
}
