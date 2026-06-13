using Localio.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Localio.Web.Pages.Demo;

public class ListModel : PageModel
{
    private readonly ISiteConfigService _siteConfigService;

    public ListModel(ISiteConfigService siteConfigService)
    {
        _siteConfigService = siteConfigService;
    }

    public IEnumerable<string> AvailableSites { get; private set; } = [];

    public void OnGet()
    {
        AvailableSites = _siteConfigService.GetAvailableSites();
    }
}
