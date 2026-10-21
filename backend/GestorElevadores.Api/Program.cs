using System.Text.Json.Serialization;
using GestorElevadores.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string CorsAngular = "angular";

// Base de datos: SQLite en un archivo local (la cadena está en appsettings.json).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Reloj del sistema inyectable: en los tests se puede sustituir por una fecha fija.
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddControllers()
    // Los enums viajan en el JSON como texto ("Tijera") y no como número (2).
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Permitir que la app Angular (otro puerto) llame a esta API.
var origenes = builder.Configuration.GetSection("Cors:Origenes").Get<string[]>() ?? ["http://localhost:4200"];
builder.Services.AddCors(o => o.AddPolicy(CorsAngular, p =>
    p.WithOrigins(origenes).AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Página de pruebas de la API en /swagger
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/openapi/v1.json", "Gestor de Elevadores API");
        o.DocumentTitle = "Gestor de Elevadores – API";
    });

    // En desarrollo, crea o actualiza la base de datos al arrancar.
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors(CorsAngular);
app.UseAuthorization();
app.MapControllers();

app.Run();

// Necesario para poder arrancar la API completa desde los tests de integración.
public partial class Program;
