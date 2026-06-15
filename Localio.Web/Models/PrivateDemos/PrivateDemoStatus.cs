namespace Localio.Web.Models.PrivateDemos;

// TODO: Demo Manager v0 — enum de estados para el registro JSON.
//       Migrar a tabla Demo en base de datos cuando corresponda.

/// <summary>
/// Estados posibles de una demo privada.
///
/// Guía rápida:
///   Draft         — interna, no enviada. noindex/nofollow. No visible públicamente.
///   ActivePrivate — enviada al prospecto. Visible por link. noindex/nofollow.
///   ActivePublic  — autorizada por el comercio para aparecer en buscadores.
///                   USAR SOLO SI EL COMERCIO AUTORIZÓ INDEXACIÓN. index/follow.
///   Paused        — temporalmente desactivada. noindex/nofollow. No visible.
///   Expired       — venció ExpiresAt. noindex/nofollow. No visible.
///   Converted     — cliente activo. index/follow. Visible.
///   Rejected      — descartada. noindex/nofollow. No visible.
///
/// Compatibilidad: el valor "Active" en demos.json se mapea automáticamente a
/// ActivePrivate por PrivateDemoStatusConverter. No usar Active en entradas nuevas.
/// </summary>
public enum PrivateDemoStatus
{
    /// <summary>Demo interna, todavía no enviada. No visible públicamente. noindex/nofollow.</summary>
    Draft,

    /// <summary>
    /// Demo activa privada. Visible por link para el prospecto. noindex/nofollow.
    /// Equivale al antiguo estado "Active".
    /// </summary>
    ActivePrivate,

    /// <summary>
    /// Demo activa pública. Visible e indexable. index/follow.
    /// USAR SOLO si el comercio autorizó que esta demo aparezca en buscadores
    /// y que sus enlaces sean follow.
    /// </summary>
    ActivePublic,

    /// <summary>Temporalmente inactiva. No visible. noindex/nofollow.</summary>
    Paused,

    /// <summary>Venció su fecha de expiración. No visible. noindex/nofollow.</summary>
    Expired,

    /// <summary>El comercio contrató el servicio. Visible. index/follow.</summary>
    Converted,

    /// <summary>El comercio no está interesado. No visible. noindex/nofollow.</summary>
    Rejected,

    /// <summary>
    /// OBSOLETO — compatibilidad con entradas viejas en demos.json.
    /// PrivateDemoStatusConverter lo mapea a ActivePrivate automáticamente.
    /// No usar en entradas nuevas.
    /// </summary>
    [Obsolete("Usar ActivePrivate. Este valor solo existe para compatibilidad con demos.json antiguo.")]
    Active
}
