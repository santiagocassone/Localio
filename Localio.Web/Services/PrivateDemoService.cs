using System.Text.Json;
using Localio.Web.Models.PrivateDemos;

namespace Localio.Web.Services;

// TODO: Demo Manager v0 — implementación basada en archivo JSON.
//       Migrar a repositorio de base de datos cuando corresponda.

/// <summary>
/// Carga y consulta el registro de demos privadas desde demos.json.
/// Usa una caché con TTL de 60 segundos para evitar lecturas excesivas en disco.
/// </summary>
public class PrivateDemoService : IPrivateDemoService
{
    private readonly string _demosFilePath;
    private readonly ILogger<PrivateDemoService> _logger;
    private readonly TimeSpan _cacheTtl = TimeSpan.FromSeconds(60);

    private PrivateDemoRegistry? _cachedRegistry;
    private DateTime _cacheExpiry = DateTime.MinValue;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        // PrivateDemoStatusConverter normaliza "Active" → ActivePrivate y maneja
        // cualquier otro valor de cadena del enum de forma case-insensitive.
        Converters = { new PrivateDemoStatusConverter() }
    };

    public PrivateDemoService(IConfiguration config, IWebHostEnvironment env, ILogger<PrivateDemoService> logger)
    {
        _logger = logger;

        var configured = config["Localio:PrivateDemosPath"];
        var demosPath  = !string.IsNullOrWhiteSpace(configured) ? configured : "../PrivateDemos";

        var resolvedDir = Path.IsPathRooted(demosPath)
            ? demosPath
            : Path.GetFullPath(Path.Combine(env.ContentRootPath, demosPath));

        _demosFilePath = Path.Combine(resolvedDir, "demos.json");

        var source = !string.IsNullOrWhiteSpace(configured) ? "Localio:PrivateDemosPath config" : "fallback (../PrivateDemos)";
        _logger.LogInformation("Private demos file resolved to: {Path} (source: {Source})", _demosFilePath, source);
    }

    /// <inheritdoc/>
    public async Task<PrivateDemoConfig?> GetBySlugAsync(string slug)
    {
        var registry = await GetRegistryAsync();
        if (registry == null) return null;

        return registry.Demos.FirstOrDefault(d =>
            string.Equals(d.Slug, slug, StringComparison.OrdinalIgnoreCase));
    }

    // ── Reglas de visibilidad / robots / sitemap ───────────────────────────────
    //
    //  Estado          │ Visible │ Indexable │ Sitemap
    //  ────────────────┼─────────┼───────────┼────────
    //  Draft           │   No    │    No     │   No
    //  ActivePrivate   │   Sí    │    No     │   No
    //  ActivePublic    │   Sí    │    Sí     │   Sí
    //  Paused          │   No    │    No     │   No
    //  Expired         │   No    │    No     │   No
    //  Converted       │   Sí    │    Sí     │   Sí
    //  Rejected        │   No    │    No     │   No
    //
    //  Si ExpiresAt está vencido → no visible, no indexable, no sitemap.

    /// <inheritdoc/>
    public bool IsPubliclyVisible(PrivateDemoConfig demo)
    {
        if (IsExpiredByDate(demo)) return false;

#pragma warning disable CS0618
        return demo.Status is
            PrivateDemoStatus.ActivePrivate or
            PrivateDemoStatus.ActivePublic  or
            PrivateDemoStatus.Converted     or
            PrivateDemoStatus.Active;   // compat: Active ya viene normalizado por el converter
#pragma warning restore CS0618
    }

    /// <inheritdoc/>
    public bool IsIndexable(PrivateDemoConfig demo)
    {
        if (IsExpiredByDate(demo)) return false;

        return demo.Status is
            PrivateDemoStatus.ActivePublic or
            PrivateDemoStatus.Converted;
    }

    /// <inheritdoc/>
    public bool ShouldRenderNoIndex(PrivateDemoConfig? demo)
    {
        if (demo == null) return true;
        return !IsIndexable(demo);
    }

    /// <inheritdoc/>
    public bool CanBeIncludedInSitemap(PrivateDemoConfig demo)
    {
        if (IsExpiredByDate(demo)) return false;

        return demo.Status is
            PrivateDemoStatus.ActivePublic or
            PrivateDemoStatus.Converted;
    }

    // ── Helpers internos ──────────────────────────────────────────────────────

    private static bool IsExpiredByDate(PrivateDemoConfig demo) =>
        demo.ExpiresAt.HasValue && demo.ExpiresAt.Value < DateTime.UtcNow;

    // ── Carga interna con caché TTL

    private async Task<PrivateDemoRegistry?> GetRegistryAsync()
    {
        if (_cachedRegistry != null && DateTime.UtcNow < _cacheExpiry)
            return _cachedRegistry;

        await _lock.WaitAsync();
        try
        {
            // Double-check tras adquirir el lock.
            if (_cachedRegistry != null && DateTime.UtcNow < _cacheExpiry)
                return _cachedRegistry;

            _cachedRegistry = await LoadFromDiskAsync();
            _cacheExpiry    = DateTime.UtcNow.Add(_cacheTtl);
            return _cachedRegistry;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<PrivateDemoRegistry?> LoadFromDiskAsync()
    {
        if (!File.Exists(_demosFilePath))
        {
            _logger.LogWarning("Private demos file not found: {Path}", _demosFilePath);
            return null;
        }

        try
        {
            await using var stream = File.OpenRead(_demosFilePath);
            var registry = await JsonSerializer.DeserializeAsync<PrivateDemoRegistry>(stream, JsonOptions);
            _logger.LogInformation("Private demos registry loaded: {Count} demo(s)", registry?.Demos.Count ?? 0);
            return registry;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse private demos file: {Path}", _demosFilePath);
            return null;
        }
    }
}
