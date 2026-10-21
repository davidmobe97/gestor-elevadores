using System.Text;
using System.Text.Json.Serialization;
using GestorElevadores.Api.Data;
using GestorElevadores.Api.Models;
using GestorElevadores.Api.Seguridad;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

const string CorsAngular = "angular";

// Base de datos: SQLite en un archivo local (la cadena está en appsettings.json).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Reloj del sistema inyectable: en los tests se puede sustituir por una fecha fija.
builder.Services.AddSingleton(TimeProvider.System);

// ---------- Seguridad: JWT ----------
var jwt = builder.Configuration.GetSection(JwtOpciones.Seccion).Get<JwtOpciones>() ?? new JwtOpciones();
if (Encoding.UTF8.GetByteCount(jwt.Clave) < 32)
    throw new InvalidOperationException("Falta la clave JWT (Jwt:Clave) o tiene menos de 32 bytes.");

builder.Services.Configure<JwtOpciones>(builder.Configuration.GetSection(JwtOpciones.Seccion));
builder.Services.AddScoped<GeneradorTokens>();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.Emisor,
        ValidateAudience = true,
        ValidAudience = jwt.Audiencia,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1),
    });

// Por defecto, TODA la API exige estar autenticado.
// Las excepciones se marcan con [AllowAnonymous] (por ejemplo, el login).
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

builder.Services.AddControllers()
    // Los enums viajan en el JSON como texto ("Tijera") y no como número (2).
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Permitir que la app Angular (otro puerto) llame a esta API.
var origenes = builder.Configuration.GetSection("Cors:Origenes").Get<string[]>() ?? ["http://localhost:4200"];
builder.Services.AddCors(o => o.AddPolicy(CorsAngular, p =>
    p.WithOrigins(origenes).AllowAnyHeader().AllowAnyMethod()));

// OpenAPI con el botón "Authorize" para probar en Swagger con el token.
builder.Services.AddOpenApi(o => o.AddDocumentTransformer((doc, _, _) =>
{
    doc.Components ??= new OpenApiComponents();
    doc.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
    doc.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pega aquí el token que devuelve POST /api/auth/login",
    };
    doc.Security ??= [];
    doc.Security.Add(new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", doc)] = [] });
    return Task.CompletedTask;
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();

    // Página de pruebas de la API en /swagger
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/openapi/v1.json", "Gestor de Elevadores API");
        o.DocumentTitle = "Gestor de Elevadores – API";
    });

    // En desarrollo, crea o actualiza la base de datos y los usuarios de demo al arrancar.
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
    await UsuariosDemo.CrearSiNoHayAsync(scope.ServiceProvider, app.Configuration);
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors(CorsAngular);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Necesario para poder arrancar la API completa desde los tests de integración.
public partial class Program;
