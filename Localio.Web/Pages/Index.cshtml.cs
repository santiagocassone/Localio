using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Localio.Web.Pages
{
    public class IndexModel : PageModel
    {
        // ── Configuración del sitio ──────────────────────────────────────────────
        // Editá estas constantes para cambiar WhatsApp, email y SEO sin tocar las vistas.

        public const string WhatsAppNumber  = "5491145678900"; // Formato internacional sin +
        public const string WhatsAppMessage = "Hola, quiero consultar por un sitio web para mi comercio.";
        public const string ContactEmail    = "hola@localio.com.ar";

        public const string SeoTitle       = "Localio — Sitios web para comercios locales";
        public const string SeoDescription = "Creamos sitios web profesionales para veterinarias, talleres, peluquerías y comercios. Claros, rápidos, con WhatsApp integrado y adaptados a celulares.";
        public const string SeoKeywords    = "sitios web comercios locales, páginas web pequeños negocios, web veterinaria, web taller mecánico, web peluquería, sitio web barrio";

        public string WhatsAppUrl =>
            $"https://wa.me/{WhatsAppNumber}?text={Uri.EscapeDataString(WhatsAppMessage)}";

        // ── Formulario de contacto ───────────────────────────────────────────────

        [BindProperty]
        public ContactFormModel ContactForm { get; set; } = new();

        public bool FormSubmitted { get; private set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // ─────────────────────────────────────────────────────────────────────
            // TODO: Implementar envío real del formulario aquí.
            //
            // Opciones recomendadas:
            //   • MailKit + SMTP    → NuGet: MailKit
            //   • SendGrid          → NuGet: SendGrid
            //   • Resend.com API    → HTTP client directo
            //   • Azure Communication Services → NuGet: Azure.Communication.Email
            //   • FormSpree         → Servicio externo (sin código en backend)
            //
            // Datos disponibles en ContactForm:
            //   .Name, .BusinessName, .Category, .Contact, .Message
            // ─────────────────────────────────────────────────────────────────────

            FormSubmitted = true;
            return Page();
        }
    }

    public class ContactFormModel
    {
        [Required(ErrorMessage = "Ingresá tu nombre")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Ingresá el nombre del comercio")]
        public string BusinessName { get; set; } = "";

        public string Category { get; set; } = "";

        [Required(ErrorMessage = "Ingresá un email o teléfono")]
        public string Contact { get; set; } = "";

        public string Message { get; set; } = "";
    }
}
