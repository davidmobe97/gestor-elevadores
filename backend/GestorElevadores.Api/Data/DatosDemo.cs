using GestorElevadores.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorElevadores.Api.Data;

/// <summary>
/// Datos ficticios para que la demo no arranque vacía.
/// Las fechas son fijas: HasData necesita valores que no cambien entre ejecuciones.
/// </summary>
internal static class DatosDemo
{
    public static void Cargar(ModelBuilder mb)
    {
        var alta = new DateTime(2025, 1, 15, 9, 0, 0, DateTimeKind.Utc);

        mb.Entity<Cliente>().HasData(
            new Cliente { Id = 1, NombreTaller = "Talleres La Vega (demo)", PersonaContacto = "Antonio Ruiz", Telefono = "600000001", Email = "lavega@example.com", Poblacion = "Alcalá de Guadaíra", Provincia = "Sevilla", FechaAlta = alta },
            new Cliente { Id = 2, NombreTaller = "Neumáticos Aljarafe (demo)", PersonaContacto = "María López", Telefono = "600000002", Email = "aljarafe@example.com", Poblacion = "Bormujos", Provincia = "Sevilla", FechaAlta = alta },
            new Cliente { Id = 3, NombreTaller = "AutoServicio Campiña (demo)", PersonaContacto = "José Moreno", Telefono = "600000003", Email = "campina@example.com", Poblacion = "Carmona", Provincia = "Sevilla", FechaAlta = alta }
        );

        mb.Entity<Elevador>().HasData(
            new Elevador { Id = 1, ClienteId = 1, Marca = "Königstein", Modelo = "KN-2C40", NumeroSerie = "DEMO-0001", Tipo = TipoElevador.DosColumnas, CapacidadKg = 4000, FechaInstalacion = new DateOnly(2024, 3, 10) },
            new Elevador { Id = 2, ClienteId = 1, Marca = "Königstein", Modelo = "KN-T30", NumeroSerie = "DEMO-0002", Tipo = TipoElevador.Tijera, CapacidadKg = 3000, FechaInstalacion = new DateOnly(2025, 6, 2) },
            new Elevador { Id = 3, ClienteId = 2, Marca = "Cascos", Modelo = "C-4C50", NumeroSerie = "DEMO-0003", Tipo = TipoElevador.CuatroColumnas, CapacidadKg = 5000, FechaInstalacion = new DateOnly(2023, 11, 20) },
            new Elevador { Id = 4, ClienteId = 3, Marca = "Cascos", Modelo = "C-P27", NumeroSerie = "DEMO-0004", Tipo = TipoElevador.Parking, CapacidadKg = 2700, FechaInstalacion = new DateOnly(2026, 2, 14) }
        );

        mb.Entity<Revision>().HasData(
            new Revision { Id = 1, ElevadorId = 1, Fecha = new DateOnly(2025, 3, 12), Tecnico = "David", Resultado = ResultadoRevision.Favorable },
            new Revision { Id = 2, ElevadorId = 3, Fecha = new DateOnly(2024, 11, 25), Tecnico = "David", Resultado = ResultadoRevision.ConDefectosLeves, Observaciones = "Cables de seguridad con desgaste, sustituir en la próxima visita." }
        );

        mb.Entity<Incidencia>().HasData(
            new Incidencia { Id = 1, ElevadorId = 3, Titulo = "Pierde aceite el cilindro", Descripcion = "Mancha de aceite bajo la columna 2.", Prioridad = PrioridadIncidencia.Alta, Estado = EstadoIncidencia.Abierta, FechaApertura = new DateTime(2026, 10, 1, 8, 30, 0, DateTimeKind.Utc) },
            new Incidencia { Id = 2, ElevadorId = 2, Titulo = "No baja del todo", Descripcion = "Se queda a 10 cm del suelo.", Prioridad = PrioridadIncidencia.Media, Estado = EstadoIncidencia.Cerrada, TecnicoAsignado = "David", FechaApertura = new DateTime(2026, 9, 3, 10, 0, 0, DateTimeKind.Utc), FechaCierre = new DateTime(2026, 9, 4, 12, 0, 0, DateTimeKind.Utc), Solucion = "Ajuste del final de carrera inferior." }
        );
    }
}
