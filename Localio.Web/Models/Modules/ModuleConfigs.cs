using System.Text.Json.Serialization;

namespace Localio.Web.Models.Modules;

// ---------------------------------------------------------------------------
// BASE MODULE CONFIG — all modules derive from this
// The "type" discriminator in JSON drives polymorphic deserialization
// ---------------------------------------------------------------------------

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(NavbarModuleConfig),          "navbar")]
[JsonDerivedType(typeof(HeroModuleConfig),            "hero")]
[JsonDerivedType(typeof(ServicesModuleConfig),        "services")]
[JsonDerivedType(typeof(FeaturedServicesModuleConfig),"featured-services")]
[JsonDerivedType(typeof(AboutModuleConfig),           "about")]
[JsonDerivedType(typeof(BenefitsModuleConfig),        "benefits")]
[JsonDerivedType(typeof(GalleryModuleConfig),         "gallery")]
[JsonDerivedType(typeof(TestimonialsModuleConfig),    "testimonials")]
[JsonDerivedType(typeof(FaqModuleConfig),             "faq")]
[JsonDerivedType(typeof(HoursModuleConfig),           "hours")]
[JsonDerivedType(typeof(LocationModuleConfig),        "location")]
[JsonDerivedType(typeof(WhatsAppButtonModuleConfig),  "whatsapp-button")]
[JsonDerivedType(typeof(ContactFormModuleConfig),     "contact-form")]
[JsonDerivedType(typeof(CtaModuleConfig),             "cta")]
[JsonDerivedType(typeof(CatalogModuleConfig),         "catalog")]
[JsonDerivedType(typeof(StaffModuleConfig),           "staff")]
[JsonDerivedType(typeof(PromotionsModuleConfig),      "promotions")]
[JsonDerivedType(typeof(EmergencyModuleConfig),       "emergency")]
[JsonDerivedType(typeof(BeforeAfterModuleConfig),     "before-after")]
[JsonDerivedType(typeof(BrandsModuleConfig),          "brands")]
[JsonDerivedType(typeof(PaymentModuleConfig),         "payment")]
[JsonDerivedType(typeof(FooterModuleConfig),          "footer")]
public abstract class ModuleConfig
{
    public bool Enabled { get; set; } = true;
    public int Order { get; set; }
    public string? Variant { get; set; }
    public string? CssClass { get; set; }
}

// ---------------------------------------------------------------------------
// NAVBAR
// ---------------------------------------------------------------------------
public class NavbarModuleConfig : ModuleConfig
{
    public string? LogoUrl { get; set; }
    public string? LogoText { get; set; }
    public bool ShowPhone { get; set; } = true;
    public bool ShowWhatsApp { get; set; } = true;
    public bool Sticky { get; set; } = true;
    public List<NavItem> Items { get; set; } = [];
}

public class NavItem
{
    public string Label { get; set; } = "";
    public string Href { get; set; } = "#";
}

// ---------------------------------------------------------------------------
// HERO
// ---------------------------------------------------------------------------
public class HeroModuleConfig : ModuleConfig
{
    public string Headline { get; set; } = "";
    public string? Subheadline { get; set; }
    public string? CtaText { get; set; }
    public string? CtaSecondaryText { get; set; }
    public string? ImageUrl { get; set; }
    public string? BackgroundImageUrl { get; set; }
    public bool ShowBadge { get; set; }
    public string? BadgeText { get; set; }
}

// ---------------------------------------------------------------------------
// SERVICES
// ---------------------------------------------------------------------------
public class ServicesModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Nuestros Servicios";
    public string? Subtitle { get; set; }
    public List<ServiceItem> Items { get; set; } = [];
}

public class FeaturedServicesModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Servicios Destacados";
    public string? Subtitle { get; set; }
    public List<ServiceItem> Items { get; set; } = [];
}

public class ServiceItem
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public string? Price { get; set; }
    public bool Featured { get; set; }
    public string? WhatsAppText { get; set; }
}

// ---------------------------------------------------------------------------
// ABOUT
// ---------------------------------------------------------------------------
public class AboutModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Sobre Nosotros";
    public string Content { get; set; } = "";
    public string? ImageUrl { get; set; }
    public List<string> Highlights { get; set; } = [];
    public string? YearsExperience { get; set; }
}

// ---------------------------------------------------------------------------
// BENEFITS
// ---------------------------------------------------------------------------
public class BenefitsModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "¿Por qué elegirnos?";
    public string? Subtitle { get; set; }
    public List<BenefitItem> Items { get; set; } = [];
}

public class BenefitItem
{
    public string Icon { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
}

// ---------------------------------------------------------------------------
// GALLERY
// ---------------------------------------------------------------------------
public class GalleryModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Galería";
    public string? Subtitle { get; set; }
    public List<GalleryImage> Images { get; set; } = [];
}

public class GalleryImage
{
    public string Url { get; set; } = "";
    public string? Alt { get; set; }
    public string? Caption { get; set; }
}

// ---------------------------------------------------------------------------
// TESTIMONIALS
// ---------------------------------------------------------------------------
public class TestimonialsModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Lo que dicen nuestros clientes";
    public string? Subtitle { get; set; }
    public List<Testimonial> Items { get; set; } = [];
}

public class Testimonial
{
    public string Name { get; set; } = "";
    public string? Role { get; set; }
    public string Content { get; set; } = "";
    public int Stars { get; set; } = 5;
    public string? AvatarUrl { get; set; }
    public string? Source { get; set; }
}

// ---------------------------------------------------------------------------
// FAQ
// ---------------------------------------------------------------------------
public class FaqModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Preguntas frecuentes";
    public string? Subtitle { get; set; }
    public List<FaqItem> Items { get; set; } = [];
}

public class FaqItem
{
    public string Question { get; set; } = "";
    public string Answer { get; set; } = "";
}

// ---------------------------------------------------------------------------
// HOURS
// ---------------------------------------------------------------------------
public class HoursModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Horarios de Atención";
    public List<BusinessHour> Items { get; set; } = [];
    public string? Note { get; set; }
    public bool ShowWhatsApp { get; set; } = true;
}

public class BusinessHour
{
    public string Days { get; set; } = "";
    public string Hours { get; set; } = "";
    public bool Closed { get; set; }
}

// ---------------------------------------------------------------------------
// LOCATION
// ---------------------------------------------------------------------------
public class LocationModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Cómo llegarnos";
    public string? EmbedMapUrl { get; set; }
    public string? Address { get; set; }
    public string? Zone { get; set; }
    public string? GoogleMapsUrl { get; set; }
    public string? TransportNote { get; set; }
}

// ---------------------------------------------------------------------------
// WHATSAPP FLOATING BUTTON
// ---------------------------------------------------------------------------
public class WhatsAppButtonModuleConfig : ModuleConfig
{
    public string? CustomMessage { get; set; }
    public string Position { get; set; } = "bottom-right";
    public bool ShowPulse { get; set; } = true;
    public bool ShowTooltip { get; set; } = true;
    public string TooltipText { get; set; } = "¡Escribinos!";
}

// ---------------------------------------------------------------------------
// CONTACT FORM
// ---------------------------------------------------------------------------
public class ContactFormModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Contactanos";
    public string? Subtitle { get; set; }
    public List<string> Fields { get; set; } = ["name", "phone", "message"];
    public string SubmitText { get; set; } = "Enviar mensaje";
    public string? SuccessMessage { get; set; }
}

// ---------------------------------------------------------------------------
// CTA
// ---------------------------------------------------------------------------
public class CtaModuleConfig : ModuleConfig
{
    public string Headline { get; set; } = "";
    public string? Subheadline { get; set; }
    public string ButtonText { get; set; } = "Contactanos por WhatsApp";
    public string? BackgroundImageUrl { get; set; }
    public bool Overlay { get; set; } = true;
}

// ---------------------------------------------------------------------------
// CATALOG
// ---------------------------------------------------------------------------
public class CatalogModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Catálogo";
    public string? Subtitle { get; set; }
    public List<CatalogCategory> Categories { get; set; } = [];
}

public class CatalogCategory
{
    public string Name { get; set; } = "";
    public List<CatalogItem> Items { get; set; } = [];
}

public class CatalogItem
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Price { get; set; }
    public string? ImageUrl { get; set; }
}

// ---------------------------------------------------------------------------
// STAFF
// ---------------------------------------------------------------------------
public class StaffModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Nuestro Equipo";
    public string? Subtitle { get; set; }
    public List<StaffMember> Members { get; set; } = [];
}

public class StaffMember
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string? Bio { get; set; }
    public string? PhotoUrl { get; set; }
    public List<string> Specialties { get; set; } = [];
}

// ---------------------------------------------------------------------------
// PROMOTIONS
// ---------------------------------------------------------------------------
public class PromotionsModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Promociones";
    public string? Subtitle { get; set; }
    public List<Promotion> Items { get; set; } = [];
}

public class Promotion
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string? Discount { get; set; }
    public string? ValidUntil { get; set; }
    public string? ImageUrl { get; set; }
    public string? CtaText { get; set; }
}

// ---------------------------------------------------------------------------
// EMERGENCY
// ---------------------------------------------------------------------------
public class EmergencyModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Atención de Urgencias";
    public string Message { get; set; } = "";
    public string Phone { get; set; } = "";
    public bool ShowWhatsApp { get; set; } = true;
    public string? Hours { get; set; }
}

// ---------------------------------------------------------------------------
// BEFORE & AFTER
// ---------------------------------------------------------------------------
public class BeforeAfterModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Antes y Después";
    public string? Subtitle { get; set; }
    public List<BeforeAfterPair> Items { get; set; } = [];
}

public class BeforeAfterPair
{
    public string BeforeUrl { get; set; } = "";
    public string AfterUrl { get; set; } = "";
    public string? Caption { get; set; }
}

// ---------------------------------------------------------------------------
// BRANDS
// ---------------------------------------------------------------------------
public class BrandsModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Marcas con las que trabajamos";
    public List<BrandItem> Items { get; set; } = [];
}

public class BrandItem
{
    public string Name { get; set; } = "";
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
}

// ---------------------------------------------------------------------------
// PAYMENT METHODS
// ---------------------------------------------------------------------------
public class PaymentModuleConfig : ModuleConfig
{
    public string Title { get; set; } = "Métodos de pago";
    public List<string> Methods { get; set; } = [];
    public string? Note { get; set; }
}

// ---------------------------------------------------------------------------
// FOOTER
// ---------------------------------------------------------------------------
public class FooterModuleConfig : ModuleConfig
{
    public string? Copyright { get; set; }
    public List<FooterLink> Links { get; set; } = [];
    public bool ShowSocial { get; set; } = true;
    public bool ShowContact { get; set; } = true;
    public string? PoweredBy { get; set; }
}

public class FooterLink
{
    public string Label { get; set; } = "";
    public string Href { get; set; } = "#";
}
