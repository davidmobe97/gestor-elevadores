using GestorElevadores.Api.Data;
using GestorElevadores.Api.Dtos;
using GestorElevadores.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorElevadores.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController(AppDbContext db, TimeProvider reloj) : ControllerBase
{
    /// <summary>Lista de clientes, con búsqueda opcional por nombre o población.</summary>
    [HttpGet]
    public async Task<List<ClienteResumenDto>> Listar([FromQuery] string? buscar)
    {
        var consulta = db.Clientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim().ToLower();
            consulta = consulta.Where(c =>
                c.NombreTaller.ToLower().Contains(texto) ||
                c.Poblacion.ToLower().Contains(texto));
        }

        return await consulta
            .OrderBy(c => c.NombreTaller)
            .Select(c => new ClienteResumenDto(
                c.Id, c.NombreTaller, c.Poblacion, c.Provincia, c.Telefono, c.Elevadores.Count))
            .ToListAsync();
    }

    /// <summary>Ficha de un cliente con sus elevadores.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteDetalleDto>> Obtener(int id)
    {
        var c = await db.Clientes.AsNoTracking()
            .Include(x => x.Elevadores).ThenInclude(e => e.Revisiones)
            .Include(x => x.Elevadores).ThenInclude(e => e.Incidencias)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (c is null) return NotFound();

        var hoy = DateOnly.FromDateTime(reloj.GetUtcNow().UtcDateTime);
        foreach (var e in c.Elevadores) e.Cliente = c;

        return new ClienteDetalleDto(
            c.Id, c.NombreTaller, c.Cif, c.PersonaContacto, c.Telefono, c.Email,
            c.Direccion, c.Poblacion, c.Provincia, c.FechaAlta,
            c.Elevadores.Select(e => e.AResumen(hoy)).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<ClienteDetalleDto>> Crear(ClienteGuardarDto dto)
    {
        var cliente = new Cliente { FechaAlta = reloj.GetUtcNow().UtcDateTime };
        Copiar(dto, cliente);

        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();

        // 201 Created + cabecera Location con la URL del nuevo cliente.
        return CreatedAtAction(nameof(Obtener), new { id = cliente.Id }, (await Obtener(cliente.Id)).Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, ClienteGuardarDto dto)
    {
        var cliente = await db.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();

        Copiar(dto, cliente);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Borrar(int id)
    {
        var cliente = await db.Clientes.Include(c => c.Elevadores).FirstOrDefaultAsync(c => c.Id == id);
        if (cliente is null) return NotFound();

        // Regla de negocio: no se borra un cliente con elevadores (se perdería el historial).
        if (cliente.Elevadores.Count > 0)
            return Conflict(new { mensaje = "No se puede borrar un cliente que tiene elevadores registrados." });

        db.Clientes.Remove(cliente);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static void Copiar(ClienteGuardarDto dto, Cliente c)
    {
        c.NombreTaller = dto.NombreTaller.Trim();
        c.Cif = dto.Cif?.Trim();
        c.PersonaContacto = dto.PersonaContacto?.Trim();
        c.Telefono = dto.Telefono?.Trim();
        c.Email = dto.Email?.Trim();
        c.Direccion = dto.Direccion?.Trim();
        c.Poblacion = dto.Poblacion.Trim();
        c.Provincia = dto.Provincia.Trim();
    }
}
