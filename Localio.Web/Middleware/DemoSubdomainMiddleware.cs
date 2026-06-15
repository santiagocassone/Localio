using System.Text.RegularExpressions;
using Localio.Web.Models;
using Microsoft.Extensions.Options;

namespace Localio.Web.Middleware;

// TODO: Demo Manager v0 — middleware de routing por subdominio.
//       Activar EnableDemoSubdomains en Azure una vez configurados DNS y certs.

/// <summary>
/// Middleware que detecta requests a subdominios de demo ({slug}.localio.com.ar)
/// y reescribe el path a /demos/{slug} para que Razor Pages lo resuelva normalmente.
///
/// Solo actúa si EnableDemoSubdomains = true. En Development permanece inactivo.
///
/// Hosts reservados ignorados (nunca se tratan como demos):
///   localio.com.ar, www, localhost, demo, demos, admin, api, app, mail, ftp
/// </summary>
public class DemoSubdomainMiddleware
{
    // Slugs de subdominio reservados — nunca se mapean a demos.
    private static readonly HashSet<string> ReservedSubdomains = new(StringComparer.OrdinalIgnoreCase)
    {
        "www", "demo", "demos", "admin", "api", "app", "mail", "ftp", "localhost"
    };

    // Slugs válidos: solo letras, dígitos y guiones; sin guión al inicio ni al final.
    private static readonly Regex SlugPattern = new(
        @"^[a-z0-9][a-z0-9\-]*[a-z0-9]$|^[a-z0-9]$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly RequestDelegate _next;
    private readonly ILogger<DemoSubdomainMiddleware> _logger;

    public DemoSubdomainMiddleware(
        RequestDelegate next,
        ILogger<DemoSubdomainMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IOptionsSnapshot<DemoSubdomainOptions> optionsSnapshot)
    {
        var options = optionsSnapshot.Value;

        // Salir rápido si la característica está deshabilitada o no hay RootDomain.
        if (!options.EnableDemoSubdomains || string.IsNullOrWhiteSpace(options.RootDomain))
        {
            await _next(context);
            return;
        }

        var slug = TryExtractSlug(context.Request.Host.Host, options.RootDomain);
        if (slug == null)
        {
            // Host no es un subdominio de demo: pipeline normal.
            await _next(context);
            return;
        }

        // Solo hacer el rewrite en el path raíz "/" o vacío.
        // Cualquier otro path (assets, favicons, etc.) pasa al pipeline normal
        // para que UseStaticFiles / MapStaticAssets los sirva correctamente.
        var path = context.Request.Path.Value ?? "/";
        if (path != "/" && path != string.Empty)
        {
            await _next(context);
            return;
        }

        // Reescribir path a la Razor Page de la demo.
        // El pipeline de ASP.NET Core evaluará la ruta con el nuevo path.
        var demosPath = $"/demos/{slug}";
        _logger.LogDebug(
            "Demo subdomain rewrite: {Host}{OriginalPath} → {DemosPath}",
            context.Request.Host, path, demosPath);

        context.Request.Path = demosPath;

        await _next(context);
    }

    /// <summary>
    /// Extrae el slug del host si es un subdominio válido y no reservado de rootDomain.
    /// Retorna null si el host no califica.
    /// </summary>
    private string? TryExtractSlug(string host, string rootDomain)
    {
        if (string.IsNullOrEmpty(host)) return null;

        // Normalizar a lowercase para comparar.
        host       = host.ToLowerInvariant();
        rootDomain = rootDomain.ToLowerInvariant();

        // El host debe terminar en ".{rootDomain}" y tener al menos un segmento antes.
        var suffix = "." + rootDomain;
        if (!host.EndsWith(suffix, StringComparison.Ordinal)) return null;

        var subdomain = host[..^suffix.Length];

        // Sin subdomain (es el dominio raíz) o con puntos (sub-subdominio) → ignorar.
        if (string.IsNullOrEmpty(subdomain) || subdomain.Contains('.')) return null;

        // Excluir hosts reservados.
        if (ReservedSubdomains.Contains(subdomain)) return null;

        // Validar que el slug solo contiene caracteres seguros.
        if (!SlugPattern.IsMatch(subdomain)) return null;

        return subdomain.ToLowerInvariant();
    }
}
