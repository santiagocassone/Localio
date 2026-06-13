using Localio.Web.Models;

namespace Localio.Web.Services;

public interface ISiteConfigService
{
    Task<(SiteConfig? Site, ThemeConfig? Theme)> LoadAsync(string siteId);
    IEnumerable<string> GetAvailableSites();
    void InvalidateCache(string siteId);
}
