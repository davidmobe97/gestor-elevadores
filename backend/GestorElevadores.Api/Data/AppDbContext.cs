using GestorElevadores.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorElevadores.Api.Data;

/// <summary>
/// Punto de entrada a la base de datos. Cada DbSet es una tabla.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Elevador> Elevadores => Set<Elevador>();
    public DbSet<Incidencia> Incidencias => Set<Incidencia>();
    public DbSet<Revision> Revisiones => Set<Revision>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // SQLite no guarda la zona horaria. Todas las fechas con hora se guardan en UTC
        // y al leerlas se marcan como UTC, para que el JSON lleve la "Z" final
        // y Angular las convierta bien a la hora de España.
        configurationBuilder.Properties<DateTime>().HaveConversion<FechaUtcConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<FechaUtcConverter>();
    }

    private sealed class FechaUtcConverter() : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
        guardar => guardar.Kind == DateTimeKind.Utc ? guardar : guardar.ToUniversalTime(),
        leer => DateTime.SpecifyKind(leer, DateTimeKind.Utc));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Guardar los enums como texto ("Tijera") en vez de números (2):
        // la base de datos se entiende mejor y no se rompe si cambia el orden.
        modelBuilder.Entity<Elevador>().Property(e => e.Tipo).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Incidencia>().Property(i => i.Prioridad).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Incidencia>().Property(i => i.Estado).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Revision>().Property(r => r.Resultado).HasConversion<string>().HasMaxLength(20);

        // No puede haber dos elevadores con el mismo número de serie de la misma marca.
        modelBuilder.Entity<Elevador>()
            .HasIndex(e => new { e.Marca, e.NumeroSerie })
            .IsUnique();

        // Si se intenta borrar un cliente con elevadores, se bloquea
        // (para no perder el historial por error).
        modelBuilder.Entity<Elevador>()
            .HasOne(e => e.Cliente)
            .WithMany(c => c.Elevadores)
            .HasForeignKey(e => e.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Al borrar un elevador sí se borran sus incidencias y revisiones.
        modelBuilder.Entity<Incidencia>()
            .HasOne(i => i.Elevador)
            .WithMany(e => e.Incidencias)
            .HasForeignKey(i => i.ElevadorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Revision>()
            .HasOne(r => r.Elevador)
            .WithMany(e => e.Revisiones)
            .HasForeignKey(r => r.ElevadorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Usuarios: email único y rol guardado como texto.
        modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Usuario>().Property(u => u.Rol).HasConversion<string>().HasMaxLength(20);

        DatosDemo.Cargar(modelBuilder);
    }
}
