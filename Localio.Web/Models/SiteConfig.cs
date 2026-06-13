using Localio.Web.Models.Modules;

namespace Localio.Web.Models;

public class SiteConfig
{
    public string SiteId { get; set; } = "";
    public string BusinessName { get; set; } = "";
    public string Category { get; set; } = "";
    public string Slogan { get; set; } = "";
    public string ShortDescription { get; set; } = "";
    public string LongDescription { get; set; } = "";
    public ContactInfo Contact { get; set; } = new();
    public SocialLinks Social { get; set; } = new();
    public SeoConfig Seo { get; set; } = new();
    public WhatsAppConfig WhatsApp { get; set; } = new();
    public List<ModuleConfig> Modules { get; set; } = [];

    public IEnumerable<ModuleConfig> ActiveModules =>
        Modules.Where(m => m.Enabled).OrderBy(m => m.Order);
}

public class ContactInfo
{
    public string Phone { get; set; } = "";
    public string WhatsApp { get; set; } = "";
    public string Email { get; set; } = "";
    public string Address { get; set; } = "";
    public string Zone { get; set; } = "";
    public string City { get; set; } = "";
    public string GoogleMapsUrl { get; set; } = "";
    public string? EmbedMapUrl { get; set; }
}

public class SocialLinks
{
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? Twitter { get; set; }
    public string? YouTube { get; set; }
    public string? TikTok { get; set; }
    public string? LinkedIn { get; set; }
}

public class SeoConfig
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string? Keywords { get; set; }
    public string? OgImage { get; set; }
    public string? CanonicalUrl { get; set; }
    public string SchemaType { get; set; } = "LocalBusiness";
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? PriceRange { get; set; }
}

public class WhatsAppConfig
{
    public string Number { get; set; } = "";
    public string DefaultMessage { get; set; } = "Hola, vi su sitio web y quería hacer una consulta.";
    public bool ShowFloat { get; set; } = true;
}
