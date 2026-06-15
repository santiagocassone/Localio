// ============================================================
// Demo privada para prospecto real: Veterinaria Urquiza.
// NO publicar como sitio oficial. NO indexar. NO enlazar desde la landing pública.
// Esta página es una muestra interna para enviar al comercio.
// ============================================================

using Microsoft.AspNetCore.Mvc.RazorPages;
using Localio.Web.Services;

namespace Localio.Web.Pages.Demos;

public class UrquizaModel : PageModel
{
    private readonly IPrivateDemoService _demoService;

    public UrquizaModel(IPrivateDemoService demoService)
    {
        _demoService = demoService;
    }

    /// <summary>True si la demo debe mostrarse; false para la pantalla no disponible.</summary>
    public bool IsVisible { get; private set; }

    /// <summary>
    /// True si se debe emitir &lt;meta name="robots" content="noindex, nofollow"&gt;.
    /// ActivePrivate, Draft, Paused, Expired y Rejected producen noindex.
    /// ActivePublic y Converted quedan indexables.
    /// </summary>
    public bool ShouldRenderNoIndex { get; private set; }

    // ── Datos del comercio ────────────────────────────────────────────────────
    // Editá estas constantes para actualizar cualquier dato sin tocar el HTML.
    // Todos los datos abajo están validados desde fuentes públicas.

    public const string BusinessName    = "Veterinaria Urquiza";
    public const string BusinessTagline = "Atención veterinaria cercana y profesional en Villa Urquiza";

    // Dirección
    public const string Address      = "Dr. Pedro Ignacio Rivera 5245, Villa Urquiza, CABA";
    public const string AddressShort = "Villa Urquiza, CABA";
    public const string MapsUrl      = "https://www.google.com.ar/maps/place/Veterinaria+Urquiza/@-34.5728652,-58.4926999,903m/data=!3m2!1e3!4b1!4m6!3m5!1s0x95bcb7433e0afd0f:0x50391198c6ceec3f!8m2!3d-34.5728696!4d-58.490125!16s%2Fg%2F11r9gm4g1m?entry=ttu&g_ep=EgoyMDI2MDYxMC4wIKXMDSoASAFQAw%3D%3D";

    // WhatsApp — formato internacional sin + ni espacios
    public const string WhatsAppNumber      = "5491133196217";
    public const string WhatsAppDisplayText = "11 3319-6217";
    public const string WhatsAppMessage     = "Hola, quiero consultar por mi mascota en Veterinaria Urquiza.";

    // Instagram — validado
    public const string InstagramHandle = "@urquizavet";
    public const string InstagramUrl    = "https://www.instagram.com/urquizavet/?hl=es";

    // Reputación — validada desde Google Maps
    public const string Rating       = "4.8";
    public const string ReviewsCount = "114";

    // ── URL calculada de WhatsApp ─────────────────────────────────────────────
    public string WhatsAppUrl =>
        $"https://wa.me/{WhatsAppNumber}?text={Uri.EscapeDataString(WhatsAppMessage)}";

    // ── SEO (noindex) ─────────────────────────────────────────────────────────
    // Nota interna: noindex/nofollow activo. No publicar como sitio oficial.
    public const string PageTitle       = "Veterinaria Urquiza — Atención veterinaria en Villa Urquiza, CABA";
    public const string PageDescription = "Veterinaria Urquiza ofrece atención veterinaria cercana y profesional para perros y gatos en Villa Urquiza, CABA. Contacto directo por WhatsApp.";

    public async Task OnGetAsync()
    {
        var demo = await _demoService.GetBySlugAsync("urquiza");
        IsVisible           = demo != null && _demoService.IsPubliclyVisible(demo);
        ShouldRenderNoIndex = _demoService.ShouldRenderNoIndex(demo);
    }
}
