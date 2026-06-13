using System.Web;

namespace Localio.Web.Helpers;

public static class WhatsAppHelper
{
    private const string BaseUrl = "https://wa.me/";

    /// <summary>Generates a wa.me deep-link with optional pre-filled message.</summary>
    public static string BuildUrl(string number, string? message = null)
    {
        var clean = CleanNumber(number);
        if (string.IsNullOrEmpty(clean)) return "#";

        var url = BaseUrl + clean;
        if (!string.IsNullOrWhiteSpace(message))
            url += "?text=" + HttpUtility.UrlEncode(message);

        return url;
    }

    /// <summary>Builds a contextual message for a specific service.</summary>
    public static string BuildServiceMessage(string businessName, string serviceName) =>
        $"Hola {businessName}, vi su sitio web y quería consultar por {serviceName}.";

    /// <summary>Strips all non-digit characters, prepends country code if missing.</summary>
    private static string CleanNumber(string number)
    {
        var digits = new string(number.Where(char.IsDigit).ToArray());
        // If local Argentine number starting with 011/0*: strip leading 0
        if (digits.StartsWith("0") && digits.Length > 1)
            digits = digits[1..];
        // Prepend AR country code if not present
        if (!digits.StartsWith("54") && digits.Length <= 12)
            digits = "54" + digits;
        return digits;
    }
}
