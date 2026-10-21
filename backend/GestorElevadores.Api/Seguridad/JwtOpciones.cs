namespace GestorElevadores.Api.Seguridad;

/// <summary>
/// Configuración del token (sección "Jwt" de appsettings).
/// La clave NO va en appsettings.json: en desarrollo está en appsettings.Development.json
/// y en producción se pasa como variable de entorno Jwt__Clave.
/// </summary>
public class JwtOpciones
{
    public const string Seccion = "Jwt";

    public string Emisor { get; set; } = "GestorElevadores";
    public string Audiencia { get; set; } = "GestorElevadores";
    public string Clave { get; set; } = string.Empty;
    public int MinutosValidez { get; set; } = 480; // una jornada de trabajo
}

/// <summary>Nombres de los roles, para usarlos en [Authorize(Roles = ...)].</summary>
public static class Roles
{
    public const string Administrador = nameof(Models.RolUsuario.Administrador);
    public const string Tecnico = nameof(Models.RolUsuario.Tecnico);
}
