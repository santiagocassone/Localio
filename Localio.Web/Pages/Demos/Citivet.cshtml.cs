// ============================================================
// Demo privada para prospecto real: Veterinaria Citivet.
// NO publicar como sitio oficial. NO indexar. NO enlazar desde la landing pública.
// Esta página es una muestra interna para enviar al comercio.
// ============================================================

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Localio.Web.Pages.Demos;

public class CitivetModel : PageModel
{
    // ── Datos del comercio ──────────────────────────────────────────────────────
    // Editá estas constantes para actualizar cualquier dato sin tocar el HTML.

    public const string BusinessName   = "Veterinaria Citivet";
    public const string BusinessTagline = "Fisioterapia y acupuntura veterinaria en CABA";

    // Dirección
    public const string Address        = "Julián Álvarez 211, CABA";
    public const string MapsUrl        = "https://www.google.com/maps/search/?api=1&query=Juli%C3%A1n%20%C3%81lvarez%20211%2C%20CABA";

    // WhatsApp — formato internacional sin + ni espacios
    public const string WhatsAppNumber  = "5491144390976";
    public const string WhatsAppMessage = "Hola, quiero consultar por Citivet y coordinar una visita.";

    // Instagram
    public const string InstagramHandle = "veterinaria_citivet";
    public const string InstagramUrl    = "https://www.instagram.com/veterinaria_citivet/";

    // Contacto visible
    public const string ContactName     = "Dra. Judith Groisman";

    // ── URL calculada de WhatsApp ────────────────────────────────────────────────
    public string WhatsAppUrl =>
        $"https://wa.me/{WhatsAppNumber}?text={Uri.EscapeDataString(WhatsAppMessage)}";

    // ── SEO (noindex) ───────────────────────────────────────────────────────────
    // Nota interna: noindex/nofollow activo. No publicar como sitio oficial.
    public const string PageTitle       = "Veterinaria Citivet — Fisioterapia y acupuntura veterinaria en CABA";
    public const string PageDescription = "Veterinaria Citivet ofrece fisioterapia y acupuntura veterinaria en Julián Álvarez 211, CABA. Consultá por WhatsApp.";

    public void OnGet() { }
}
