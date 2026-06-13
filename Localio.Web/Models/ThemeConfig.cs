namespace Localio.Web.Models;

public class ThemeConfig
{
    // Preset identifier for documentation reference
    public string Preset { get; set; } = "health";

    // Color palette
    public string PrimaryColor { get; set; } = "#1E6B99";
    public string SecondaryColor { get; set; } = "#F0F7FC";
    public string AccentColor { get; set; } = "#F26419";
    public string TextColor { get; set; } = "#1A2332";
    public string TextMutedColor { get; set; } = "#5A6A7A";
    public string SurfaceColor { get; set; } = "#FFFFFF";
    public string SurfaceAltColor { get; set; } = "#F0F7FC";
    public string DarkColor { get; set; } = "#0D2840";

    // Typography
    public string HeadingFont { get; set; } = "'Nunito', sans-serif";
    public string BodyFont { get; set; } = "'Open Sans', sans-serif";
    public string FontsUrl { get; set; } = "https://fonts.googleapis.com/css2?family=Nunito:wght@400;600;700;800;900&family=Open+Sans:wght@400;500;600&display=swap";

    // Shape & shadows
    public string BorderRadius { get; set; } = "12px";
    public string BorderRadiusSm { get; set; } = "8px";
    public string BorderRadiusLg { get; set; } = "20px";

    // Style tokens — drive CSS attribute selectors on <body>
    public string ShadowIntensity { get; set; } = "soft";    // soft | medium | strong | none
    public string ButtonStyle { get; set; } = "rounded";     // rounded | sharp | pill
    public string CardStyle { get; set; } = "elevated";      // elevated | flat | outlined | dark
    public string HeroStyle { get; set; } = "gradient";      // gradient | image | solid | split
    public string Tone { get; set; } = "warm";               // warm | cool | bold | elegant | minimal
}
