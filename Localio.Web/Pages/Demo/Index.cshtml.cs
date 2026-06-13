using Localio.Web.Helpers;
using Localio.Web.Models;
using Localio.Web.Models.Modules;
using Localio.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Localio.Web.Pages.Demo;

public class IndexModel : PageModel
{
    private readonly ISiteConfigService _siteConfigService;

    public IndexModel(ISiteConfigService siteConfigService)
    {
        _siteConfigService = siteConfigService;
    }

    [BindProperty(SupportsGet = true)]
    public string SiteId { get; set; } = "";

    public SiteConfig Site { get; private set; } = new();
    public ThemeConfig Theme { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrWhiteSpace(SiteId))
        {
            var available = _siteConfigService.GetAvailableSites().ToList();
            if (available.Count > 0)
                return RedirectToPage(new { SiteId = available[0] });
            return NotFound("No hay sitios configurados. Crea una carpeta en /Sites con site.json y theme.json.");
        }

        var (site, theme) = await _siteConfigService.LoadAsync(SiteId);
        if (site == null)
            return NotFound($"Sitio '{SiteId}' no encontrado. Verifica que exista /Sites/{SiteId}/site.json.");

        Site = site;
        Theme = theme ?? new ThemeConfig();
        return Page();
    }

    /// <summary>Maps a ModuleConfig to its partial view name.</summary>
    public static string? GetPartialName(ModuleConfig module) => module switch
    {
        NavbarModuleConfig          => "Navbar",
        HeroModuleConfig            => "Hero",
        ServicesModuleConfig        => "Services",
        FeaturedServicesModuleConfig => "FeaturedServices",
        AboutModuleConfig           => "About",
        BenefitsModuleConfig        => "Benefits",
        GalleryModuleConfig         => "Gallery",
        TestimonialsModuleConfig    => "Testimonials",
        FaqModuleConfig             => "Faq",
        HoursModuleConfig           => "Hours",
        LocationModuleConfig        => "Location",
        WhatsAppButtonModuleConfig  => "WhatsAppButton",
        ContactFormModuleConfig     => "ContactForm",
        CtaModuleConfig             => "Cta",
        CatalogModuleConfig         => "Catalog",
        StaffModuleConfig           => "Staff",
        PromotionsModuleConfig      => "Promotions",
        EmergencyModuleConfig       => "Emergency",
        BeforeAfterModuleConfig     => "BeforeAfter",
        BrandsModuleConfig          => "Brands",
        PaymentModuleConfig         => "Payment",
        FooterModuleConfig          => "Footer",
        _                           => null
    };

    public string WhatsAppUrl(string? customMessage = null) =>
        WhatsAppHelper.BuildUrl(Site.WhatsApp.Number, customMessage ?? Site.WhatsApp.DefaultMessage);

    public string WhatsAppServiceUrl(string serviceName) =>
        WhatsAppHelper.BuildUrl(Site.WhatsApp.Number,
            WhatsAppHelper.BuildServiceMessage(Site.BusinessName, serviceName));
}
