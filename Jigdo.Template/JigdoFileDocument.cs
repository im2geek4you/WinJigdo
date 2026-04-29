namespace Jigdo;

/// <summary>Parsed content of a gzip-compressed <c>.jigdo</c> text file (jigit / libjte format).</summary>
public sealed class JigdoFileDocument
{
    /// <summary>[Jigdo] section: e.g. <c>2.0</c> for SHA256 templates.</summary>
    public string? Version { get; init; }

    public string? Generator { get; init; }

    /// <summary>[Image] section.</summary>
    public JigdoImageSection Image { get; init; } = new();

    /// <summary>
    /// [Parts] lines: jigdo pseudo-base64 checksum → mirror label + relative path
    /// (same logical entry as <see cref="JigdoPartEntry"/>).
    /// </summary>
    public IReadOnlyDictionary<string, JigdoPartEntry> Parts { get; init; } =
        new Dictionary<string, JigdoPartEntry>(StringComparer.Ordinal);

    /// <summary>
    /// [Servers] entries in file order: mirror label (e.g. <c>Debian</c>) → base URLs for HTTP(S) download.
    /// </summary>
    public IReadOnlyList<JigdoServerLine> ServerLines { get; init; } = Array.Empty<JigdoServerLine>();

    /// <summary>
    /// Returns a copy with <paramref name="prepend"/> server lines inserted before the file&apos;s <see cref="ServerLines"/>
    /// (custom mirrors are tried first).
    /// </summary>
    public JigdoFileDocument WithPrependedServers(IReadOnlyList<JigdoServerLine> prepend)
    {
        ArgumentNullException.ThrowIfNull(prepend);
        if (prepend.Count == 0)
            return this;

        var merged = new List<JigdoServerLine>(prepend.Count + ServerLines.Count);
        merged.AddRange(prepend);
        merged.AddRange(ServerLines);
        return new JigdoFileDocument
        {
            Version = Version,
            Generator = Generator,
            Image = Image,
            Parts = Parts,
            ServerLines = merged,
        };
    }
}

/// <summary>[Image] fields from the jigdo file.</summary>
public sealed class JigdoImageSection
{
    public string? IsoFilename { get; init; }
    public string? TemplateFilename { get; init; }
    public string? TemplateSha256SumBase64 { get; init; }
    public string? TemplateMd5SumBase64 { get; init; }
}

/// <summary>One <c>[Servers]</c> line: <c>Label=URL</c> with optional <c>--try-last</c>.</summary>
public sealed class JigdoServerLine
{
    public string MirrorLabel { get; init; } = "";
    public Uri BaseUrl { get; init; } = null!;
    /// <summary>When true, GUI tools prefer other mirrors first; download order still follows file order.</summary>
    public bool TryLast { get; init; }
}
