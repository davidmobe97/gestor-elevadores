using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GestorElevadores.Tests;

/// <summary>
/// Arranca la API completa en memoria con una base de datos SQLite temporal.
/// Usa los usuarios de demo de appsettings.Development.json.
/// </summary>
public class ApiDePrueba : WebApplicationFactory<Program>
{
    private readonly string _bd = Path.Combine(Path.GetTempPath(), $"gestor-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:Default", $"Data Source={_bd}");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        try { File.Delete(_bd); } catch { /* da igual si no se puede borrar */ }
    }
}

public class SeguridadTests(ApiDePrueba api) : IClassFixture<ApiDePrueba>
{
    private record Sesion(string Token, string Rol);

    private async Task<HttpClient> ClienteConSesion(string email, string password)
    {
        var http = api.CreateClient();
        var r = await http.PostAsJsonAsync("/api/auth/login", new { email, password });
        r.EnsureSuccessStatusCode();
        var sesion = await r.Content.ReadFromJsonAsync<Sesion>();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sesion!.Token);
        return http;
    }

    private Task<HttpClient> ComoAdmin() => ClienteConSesion("admin@demo.local", "Admin2026!");
    private Task<HttpClient> ComoTecnico() => ClienteConSesion("tecnico@demo.local", "Tecnico2026!");

    [Fact]
    public async Task SinToken_LaApiResponde401()
    {
        var r = await api.CreateClient().GetAsync("/api/clientes");
        Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
    }

    [Fact]
    public async Task Login_ConContrasenaMala_Responde401()
    {
        var r = await api.CreateClient().PostAsJsonAsync("/api/auth/login",
            new { email = "admin@demo.local", password = "no-es-esta" });
        Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
    }

    [Fact]
    public async Task Login_Correcto_DevuelveTokenYRol()
    {
        var r = await api.CreateClient().PostAsJsonAsync("/api/auth/login",
            new { email = "ADMIN@demo.local", password = "Admin2026!" }); // el email no distingue mayúsculas
        r.EnsureSuccessStatusCode();
        var sesion = await r.Content.ReadFromJsonAsync<Sesion>();
        Assert.False(string.IsNullOrWhiteSpace(sesion!.Token));
        Assert.Equal("Administrador", sesion.Rol);
    }

    [Fact]
    public async Task Tecnico_PuedeConsultarYAbrirIncidencias()
    {
        var http = await ComoTecnico();

        Assert.Equal(HttpStatusCode.OK, (await http.GetAsync("/api/clientes")).StatusCode);

        var r = await http.PostAsJsonAsync("/api/incidencias",
            new { elevadorId = 1, titulo = "Ruido al subir", prioridad = "Media" });
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
    }

    [Fact]
    public async Task Tecnico_NoPuedeCrearNiBorrarClientes()
    {
        var http = await ComoTecnico();

        var crear = await http.PostAsJsonAsync("/api/clientes",
            new { nombreTaller = "Taller X", poblacion = "Sevilla", provincia = "Sevilla" });
        Assert.Equal(HttpStatusCode.Forbidden, crear.StatusCode);

        var borrar = await http.DeleteAsync("/api/clientes/3");
        Assert.Equal(HttpStatusCode.Forbidden, borrar.StatusCode);
    }

    [Fact]
    public async Task Admin_PuedeCrearClientes()
    {
        var http = await ComoAdmin();
        var r = await http.PostAsJsonAsync("/api/clientes",
            new { nombreTaller = "Taller de prueba", poblacion = "Dos Hermanas", provincia = "Sevilla" });
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
    }
}
