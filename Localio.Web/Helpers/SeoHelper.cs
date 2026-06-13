using System.Text;
using System.Text.Json;
using Localio.Web.Models;

namespace Localio.Web.Helpers;

public static class SeoHelper
{
    public static string GenerateJsonLd(SiteConfig site)
    {
        var contact = site.Contact;
        var seo = site.Seo;

        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = MapSchemaType(site.Category, seo.SchemaType),
            ["name"] = site.BusinessName,
            ["description"] = site.ShortDescription,
            ["telephone"] = contact.Phone,
            ["email"] = string.IsNullOrEmpty(contact.Email) ? null : contact.Email,
            ["url"] = seo.CanonicalUrl,
            ["priceRange"] = seo.PriceRange,
            ["address"] = new Dictionary<string, object>
            {
                ["@type"] = "PostalAddress",
                ["streetAddress"] = contact.Address,
                ["addressLocality"] = contact.City,
                ["addressCountry"] = "AR"
            }
        };

        if (seo.Latitude.HasValue && seo.Longitude.HasValue)
        {
            schema["geo"] = new Dictionary<string, object>
            {
                ["@type"] = "GeoCoordinates",
                ["latitude"] = seo.Latitude.Value,
                ["longitude"] = seo.Longitude.Value
            };
        }

        if (!string.IsNullOrEmpty(seo.OgImage))
            schema["image"] = seo.OgImage;

        var social = new List<string?>();
        if (!string.IsNullOrEmpty(site.Social.Facebook)) social.Add(site.Social.Facebook);
        if (!string.IsNullOrEmpty(site.Social.Instagram)) social.Add(site.Social.Instagram);
        if (!string.IsNullOrEmpty(site.Social.Twitter)) social.Add(site.Social.Twitter);
        if (social.Count > 0)
            schema["sameAs"] = social;

        // Remove null values for clean output
        var cleaned = schema.Where(kv => kv.Value != null)
                            .ToDictionary(kv => kv.Key, kv => kv.Value);

        return JsonSerializer.Serialize(cleaned, new JsonSerializerOptions { WriteIndented = false });
    }

    private static string MapSchemaType(string category, string fallback) => category.ToLowerInvariant() switch
    {
        "veterinary" or "veterinaria" => "VeterinaryCare",
        "dental" or "odontologia" or "odontólogo" => "Dentist",
        "auto-repair" or "taller" or "mecanica" or "mecánica" => "AutoRepair",
        "beauty" or "estetica" or "estética" or "peluqueria" or "peluquería" => "BeautySalon",
        "gym" or "gimnasio" => "ExerciseGym",
        "restaurant" or "restaurante" => "Restaurant",
        "electrician" or "electricista" => "Electrician",
        "plumber" or "plomero" => "Plumber",
        "locksmith" or "cerrajero" => "Locksmith",
        _ => string.IsNullOrEmpty(fallback) ? "LocalBusiness" : fallback
    };

    public static string BuildPageTitle(SiteConfig site)
    {
        if (!string.IsNullOrEmpty(site.Seo.Title)) return site.Seo.Title;
        var parts = new List<string> { site.BusinessName };
        if (!string.IsNullOrEmpty(site.Contact.City)) parts.Add(site.Contact.City);
        return string.Join(" | ", parts);
    }

    public static string BuildDescription(SiteConfig site)
    {
        if (!string.IsNullOrEmpty(site.Seo.Description)) return site.Seo.Description;
        return site.ShortDescription.Length > 160
            ? site.ShortDescription[..157] + "..."
            : site.ShortDescription;
    }

    /// <summary>Generates CSS custom property block from ThemeConfig.</summary>
    public static string GenerateThemeCss(ThemeConfig theme)
    {
        var sb = new StringBuilder();
        sb.AppendLine(":root {");
        sb.AppendLine($"  --color-primary: {theme.PrimaryColor};");
        sb.AppendLine($"  --color-secondary: {theme.SecondaryColor};");
        sb.AppendLine($"  --color-accent: {theme.AccentColor};");
        sb.AppendLine($"  --color-text: {theme.TextColor};");
        sb.AppendLine($"  --color-text-muted: {theme.TextMutedColor};");
        sb.AppendLine($"  --color-surface: {theme.SurfaceColor};");
        sb.AppendLine($"  --color-surface-alt: {theme.SurfaceAltColor};");
        sb.AppendLine($"  --color-dark: {theme.DarkColor};");
        sb.AppendLine($"  --font-heading: {theme.HeadingFont};");
        sb.AppendLine($"  --font-body: {theme.BodyFont};");
        sb.AppendLine($"  --radius: {theme.BorderRadius};");
        sb.AppendLine($"  --radius-sm: {theme.BorderRadiusSm};");
        sb.AppendLine($"  --radius-lg: {theme.BorderRadiusLg};");
        sb.AppendLine($"  {GetShadowVars(theme.ShadowIntensity)}");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string GetShadowVars(string intensity) => intensity switch
    {
        "none" =>  "--shadow: none; --shadow-md: none; --shadow-hover: none;",
        "strong" => "--shadow: 0 4px 24px rgba(0,0,0,0.18); --shadow-md: 0 8px 40px rgba(0,0,0,0.22); --shadow-hover: 0 12px 48px rgba(0,0,0,0.28);",
        "medium" => "--shadow: 0 2px 16px rgba(0,0,0,0.12); --shadow-md: 0 6px 28px rgba(0,0,0,0.15); --shadow-hover: 0 8px 36px rgba(0,0,0,0.20);",
        _ =>        "--shadow: 0 2px 12px rgba(0,0,0,0.07); --shadow-md: 0 4px 20px rgba(0,0,0,0.10); --shadow-hover: 0 6px 28px rgba(0,0,0,0.14);"
    };
}
