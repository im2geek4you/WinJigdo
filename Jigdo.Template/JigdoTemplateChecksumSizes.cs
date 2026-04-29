namespace Jigdo;

/// <summary>Maps jigdo checksum keys (pseudo-base64) to expected file sizes from a parsed template.</summary>
public static class JigdoTemplateChecksumSizes
{
    /// <summary>
    /// For each distinct <see cref="TemplateMatchSha256Entry"/> SHA256, records the extent size
    /// (which equals the whole file size for typical Debian-style jigdo parts).
    /// </summary>
    public static IReadOnlyDictionary<string, ulong> FromTemplate(JigdoTemplateDocument template)
    {
        var map = new Dictionary<string, ulong>(StringComparer.Ordinal);
        foreach (var e in template.Entries)
        {
            if (e is not TemplateMatchSha256Entry m)
                continue;
            string key = JigdoBase64.Encode(m.Sha256.Span);
            map.TryAdd(key, m.ExtentBytes);
        }

        return map;
    }
}
