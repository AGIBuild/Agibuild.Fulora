namespace Agibuild.Fulora.Adapters.Abstractions;

/// <summary>
/// Parses the cookie JSON array format produced by macOS/GTK/iOS native shims.
/// Uses simple string parsing to avoid a System.Text.Json dependency in adapter assemblies.
/// </summary>
internal static class AdapterCookieParser
{
    /// <summary>
    /// Parses a JSON array of cookie objects into <see cref="WebViewCookie"/> instances.
    /// </summary>
    internal static IReadOnlyList<WebViewCookie> ParseCookiesJson(string? json)
    {
        var cookies = new List<WebViewCookie>();
        if (string.IsNullOrWhiteSpace(json) || json == "[]") return cookies;

        int idx = 0;
        while (idx < json.Length)
        {
            int objStart = json.IndexOf('{', idx);
            if (objStart < 0) break;
            int objEnd = json.IndexOf('}', objStart);
            if (objEnd < 0) break;

            var obj = json.Substring(objStart, objEnd - objStart + 1);
            idx = objEnd + 1;

            var name = ExtractJsonString(obj, "name");
            var value = ExtractJsonString(obj, "value");
            var domain = ExtractJsonString(obj, "domain");
            var path = ExtractJsonString(obj, "path");
            var expiresStr = ExtractJsonRaw(obj, "expires");
            var isSecure = ExtractJsonRaw(obj, "isSecure") == "true";
            var isHttpOnly = ExtractJsonRaw(obj, "isHttpOnly") == "true";

            DateTimeOffset? expires = null;
            if (double.TryParse(expiresStr, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var unix) && unix > 0)
            {
                expires = DateTimeOffset.FromUnixTimeSeconds((long)unix);
            }

            cookies.Add(new WebViewCookie(name, value, domain, path, expires, isSecure, isHttpOnly));
        }

        return cookies;
    }

    internal static string ExtractJsonString(string json, string key)
    {
        var needle = $"\"{key}\":\"";
        var start = json.IndexOf(needle, StringComparison.Ordinal);
        if (start < 0) return string.Empty;
        start += needle.Length;
        var end = start;
        while (end < json.Length)
        {
            if (json[end] == '"' && (end == start || json[end - 1] != '\\')) break;
            end++;
        }
        return json.Substring(start, end - start).Replace("\\\"", "\"").Replace("\\\\", "\\");
    }

    internal static string ExtractJsonRaw(string json, string key)
    {
        var needle = $"\"{key}\":";
        var start = json.IndexOf(needle, StringComparison.Ordinal);
        if (start < 0) return string.Empty;
        start += needle.Length;
        var end = start;
        while (end < json.Length && json[end] != ',' && json[end] != '}') end++;
        return json.Substring(start, end - start).Trim();
    }
}
