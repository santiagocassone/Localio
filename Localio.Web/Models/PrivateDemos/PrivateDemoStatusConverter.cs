using System.Text.Json;
using System.Text.Json.Serialization;

namespace Localio.Web.Models.PrivateDemos;

// TODO: Demo Manager v0 — converter de compatibilidad para demos.json.
//       Eliminar cuando ya no existan entradas con Status = "Active" en disco.

/// <summary>
/// JsonConverter que lee el campo "status" de demos.json y normaliza el
/// valor legado "Active" a <see cref="PrivateDemoStatus.ActivePrivate"/>.
/// Todos los demás valores del enum se parsean normalmente (case-insensitive).
/// </summary>
public sealed class PrivateDemoStatusConverter : JsonConverter<PrivateDemoStatus>
{
    public override PrivateDemoStatus Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var raw = reader.GetString();

        // Compatibilidad: "Active" → ActivePrivate.
        // El viejo estado Active equivale a demo privada visible por link.
        if (string.Equals(raw, "Active", StringComparison.OrdinalIgnoreCase))
            return PrivateDemoStatus.ActivePrivate;

        if (Enum.TryParse<PrivateDemoStatus>(raw, ignoreCase: true, out var parsed))
            return parsed;

        // Valor desconocido: degradar a Draft (seguro por defecto, no visible).
        return PrivateDemoStatus.Draft;
    }

    public override void Write(
        Utf8JsonWriter writer,
        PrivateDemoStatus value,
        JsonSerializerOptions options)
    {
        // Al serializar, siempre escribir el nombre canónico del enum.
        // Active obsoleto nunca debería escribirse; si aparece, normalizar.
#pragma warning disable CS0618
        if (value == PrivateDemoStatus.Active)
        {
            writer.WriteStringValue(nameof(PrivateDemoStatus.ActivePrivate));
            return;
        }
#pragma warning restore CS0618

        writer.WriteStringValue(value.ToString());
    }
}
