using Localio.Web.Models.PrivateDemos;

namespace Localio.Web.Services;

// TODO: Demo Manager v0 — contrato del servicio JSON de demos privadas.
//       Reemplazar implementación cuando se migre a base de datos.

/// <summary>
/// Servicio para consultar el estado de demos privadas desde demos.json.
/// </summary>
public interface IPrivateDemoService
{
    /// <summary>
    /// Retorna la configuración de una demo privada por su slug,
    /// o null si no existe en el registro.
    /// </summary>
    Task<PrivateDemoConfig?> GetBySlugAsync(string slug);

    /// <summary>
    /// Determina si una demo debe mostrarse públicamente.
    /// Retorna true para ActivePrivate, ActivePublic y Converted (sin ExpiresAt vencido).
    /// </summary>
    bool IsPubliclyVisible(PrivateDemoConfig demo);

    /// <summary>
    /// Determina si una demo puede indexarse (index/follow).
    /// Retorna true solo para ActivePublic y Converted (sin ExpiresAt vencido).
    /// </summary>
    bool IsIndexable(PrivateDemoConfig demo);

    /// <summary>
    /// Determina si se debe renderizar la meta tag noindex/nofollow.
    /// Es el inverso lógico de <see cref="IsIndexable"/>.
    /// Siempre true para la pantalla de demo no disponible.
    /// </summary>
    bool ShouldRenderNoIndex(PrivateDemoConfig? demo);

    /// <summary>
    /// Determina si una demo puede incluirse en el sitemap.
    /// Solo true para ActivePublic y Converted (sin ExpiresAt vencido).
    /// </summary>
    bool CanBeIncludedInSitemap(PrivateDemoConfig demo);
}

