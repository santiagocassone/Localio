using System.Text.Json;
using Localio.Web.Models;

namespace Localio.Web.Services;

public class SiteConfigService : ISiteConfigService
{
    private readonly string _sitesRoot;
    private readonly Dictionary<string, (SiteConfig Site, ThemeConfig Theme)> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<SiteConfigService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    public SiteConfigService(IConfiguration config, IWebHostEnvironment env, ILogger<SiteConfigService> logger)
    {
        _logger = logger;
        var sitesPath = config["Localio:SitesPath"] ?? "../Sites";
        _sitesRoot = Path.IsPathRooted(sitesPath)
            ? sitesPath
            : Path.GetFullPath(Path.Combine(env.ContentRootPath, sitesPath));
        _logger.LogInformation("Sites directory resolved to: {Path}", _sitesRoot);
    }

    public async Task<(SiteConfig? Site, ThemeConfig? Theme)> LoadAsync(string siteId)
    {
        if (_cache.TryGetValue(siteId, out var cached))
            return (cached.Site, cached.Theme);

        var siteDir = Path.Combine(_sitesRoot, siteId);
        if (!Directory.Exists(siteDir))
        {
            _logger.LogWarning("Site directory not found: {Dir}", siteDir);
            return (null, null);
        }

        var sitePath = Path.Combine(siteDir, "site.json");
        if (!File.Exists(sitePath))
        {
            _logger.LogWarning("site.json not found in: {Dir}", siteDir);
            return (null, null);
        }

        SiteConfig? site;
        try
        {
            await using var siteStream = File.OpenRead(sitePath);
            site = await JsonSerializer.DeserializeAsync<SiteConfig>(siteStream, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse site.json for '{SiteId}'", siteId);
            return (null, null);
        }

        ThemeConfig theme = new();
        var themePath = Path.Combine(siteDir, "theme.json");
        if (File.Exists(themePath))
        {
            try
            {
                await using var themeStream = File.OpenRead(themePath);
                theme = await JsonSerializer.DeserializeAsync<ThemeConfig>(themeStream, JsonOptions) ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse theme.json for '{SiteId}', using defaults", siteId);
            }
        }

        if (site == null) return (null, null);

        site.SiteId = siteId;
        _cache[siteId] = (site, theme);
        return (site, theme);
    }

    public IEnumerable<string> GetAvailableSites()
    {
        if (!Directory.Exists(_sitesRoot)) return [];
        return Directory.GetDirectories(_sitesRoot)
            .Select(Path.GetFileName)
            .Where(d => !string.IsNullOrEmpty(d))!;
    }

    public void InvalidateCache(string siteId) => _cache.Remove(siteId);
}
