using System.Text.RegularExpressions;

namespace api.Sanitization;

public static class InputSanitizer
{
    /*
    Primary sanitizer for plain text fields (title, author, quote).
    Steps:
    - remove script tags
    - remove any remaining HTML tags
    - allow only alphanumeric characters and basic punctuation
    - normalize whitespace and trim
    - truncate to maxLen
    */
    public static string SanitizeText(string? input, int maxLen = 1000)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove <script> tags
        var noScript = Regex.Replace(input, @"<script[\s\S]*?>[\s\S]*?</script>", "", RegexOptions.IgnoreCase);

        // Remove remaining HTML tags
        var noHtml = Regex.Replace(noScript, @"<[^>]+>", "");

        // Allow only alphanumeric and basic punctuation
        var sanitized = Regex.Replace(noHtml, @"[^a-zA-Z0-9\s.,:;!?()\[\]""'-]", "");

        // Normalize whitespace
        sanitized = Regex.Replace(sanitized, @"\s+", " ").Trim();

        // Truncate
        if (sanitized.Length > maxLen)
            sanitized = sanitized.Substring(0, maxLen);

        return sanitized;
    }

    // Quick check if the sanitized text is non-empty
    public static bool IsSanitizedNonEmpty(string? input, int maxLen = 1000)
    {
        return !string.IsNullOrWhiteSpace(SanitizeText(input, maxLen));
    }

    // Sanitizes a numeric date field, allowing only digits
    public static string SanitizeDate(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var digitsOnly = Regex.Replace(input, @"[^0-9]", "").Trim();
        return digitsOnly;
    }
}
