namespace Localio.Web.Models.PrivateDemos;

// TODO: Demo Manager v0 — modelos para el registro JSON de demos privadas.
//       Migrar a tabla Demo en base de datos cuando corresponda.

/// <summary>
/// Configuración de una demo privada individual, leída desde demos.json.
/// </summary>
public class PrivateDemoConfig
{
    public string Id            { get; set; } = string.Empty;
    public string Slug          { get; set; } = string.Empty;
    public string BusinessName  { get; set; } = string.Empty;
    public string BusinessType  { get; set; } = string.Empty;
    public PrivateDemoStatus Status { get; set; } = PrivateDemoStatus.Draft;
    public string PublicUrl     { get; set; } = string.Empty;
    public string ContactName   { get; set; } = string.Empty;
    public string ContactPhone  { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? Notes        { get; set; }
    public DateTime CreatedAt   { get; set; }
    public DateTime UpdatedAt   { get; set; }
    public DateTime? ExpiresAt  { get; set; }
    public bool IsIndexed       { get; set; }
}

/// <summary>
/// Raíz del archivo demos.json.
/// </summary>
public class PrivateDemoRegistry
{
    public List<PrivateDemoConfig> Demos { get; set; } = [];
}
