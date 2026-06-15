namespace Localio.Web.Models;

/// <summary>
/// Configuración de analytics. Habilitá por ambiente con variables de entorno:
///   Analytics__Clarity__Enabled=true
///   Analytics__Clarity__ProjectId=xxxxxxxx
/// </summary>
public class AnalyticsOptions
{
    public const string SectionName = "Analytics";

    public ClarityOptions Clarity { get; set; } = new();
}

public class ClarityOptions
{
    public bool   Enabled   { get; set; } = false;
    public string ProjectId { get; set; } = string.Empty;
}
