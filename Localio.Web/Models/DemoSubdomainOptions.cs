namespace Localio.Web.Models;

// TODO: Demo Manager v0 — configuración de subdominios por demo privada.
//       Activar EnableDemoSubdomains = true en Azure una vez configurados DNS y certificados.

/// <summary>
/// Opciones para el soporte de subdominios por demo privada.
///
/// En producción configurar via variables de entorno:
///   Localio__RootDomain = localio.com.ar
///   Localio__EnableDemoSubdomains = true
///
/// En desarrollo dejar EnableDemoSubdomains = false (valor por defecto).
/// Para probar en local, configurar hosts en /etc/hosts o Windows hosts file
/// y activar con un override en appsettings.Development.json.
/// </summary>
public class DemoSubdomainOptions
{
    public const string SectionName = "Localio";

    /// <summary>
    /// Dominio raíz de la aplicación, sin www ni protocolo.
    /// Ejemplo: "localio.com.ar"
    /// </summary>
    public string RootDomain { get; set; } = string.Empty;

    /// <summary>
    /// Activa el routing por subdominio para demos privadas.
    /// Mantener en false en Development. Activar solo en producción
    /// una vez que DNS y certificados estén configurados en Azure.
    /// </summary>
    public bool EnableDemoSubdomains { get; set; } = false;
}
