using GestorElevadores.Api.Models;

namespace GestorElevadores.Tests;

public class ElevadorTests
{
    [Fact]
    public void SinRevisiones_LaProximaEsUnAnoDespuesDeLaInstalacion()
    {
        var elevador = new Elevador { FechaInstalacion = new DateOnly(2026, 2, 14) };

        Assert.Equal(new DateOnly(2027, 2, 14), elevador.CalcularProximaRevision());
    }

    [Fact]
    public void ConRevisiones_LaProximaSeCalculaDesdeLaMasReciente()
    {
        var elevador = new Elevador
        {
            FechaInstalacion = new DateOnly(2023, 1, 1),
            Revisiones =
            [
                new Revision { Fecha = new DateOnly(2024, 1, 10) },
                new Revision { Fecha = new DateOnly(2025, 1, 20) }, // la más reciente
                new Revision { Fecha = new DateOnly(2023, 12, 5) }
            ]
        };

        Assert.Equal(new DateOnly(2026, 1, 20), elevador.CalcularProximaRevision());
    }
}

public class IncidenciaTests
{
    private static readonly DateTime Ahora = new(2026, 10, 21, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Cerrar_GuardaSolucionFechaYEstado()
    {
        var incidencia = new Incidencia { Titulo = "No sube" };

        incidencia.Cerrar("  Cambio de fusible  ", Ahora);

        Assert.Equal(EstadoIncidencia.Cerrada, incidencia.Estado);
        Assert.Equal("Cambio de fusible", incidencia.Solucion);
        Assert.Equal(Ahora, incidencia.FechaCierre);
    }

    [Fact]
    public void Cerrar_SinSolucion_LanzaError()
    {
        var incidencia = new Incidencia { Titulo = "No sube" };

        Assert.Throws<ArgumentException>(() => incidencia.Cerrar("   ", Ahora));
    }

    [Fact]
    public void Cerrar_DosVeces_LanzaError()
    {
        var incidencia = new Incidencia { Titulo = "No sube" };
        incidencia.Cerrar("Reparado", Ahora);

        Assert.Throws<InvalidOperationException>(() => incidencia.Cerrar("Otra vez", Ahora));
    }
}
