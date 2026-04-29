namespace Jigdo;

/// <summary>
/// Lookup of jigdo <c>[Parts]</c> entries by pseudo-base64 checksum (legacy helper; prefer <see cref="JigdoFileDocument"/>).
/// </summary>
public sealed class JigdoPartsIndex
{
    public IReadOnlyDictionary<string, JigdoPartEntry> PartsByChecksum { get; }

    public JigdoPartsIndex(IReadOnlyDictionary<string, JigdoPartEntry> partsByChecksum) =>
        PartsByChecksum = partsByChecksum;

    public JigdoPartsIndex(JigdoFileDocument document) =>
        PartsByChecksum = document.Parts;

    /// <summary>Load from a gzip-compressed <c>.jigdo</c> file (same as <see cref="JigdoFileReader.ReadFile"/>).</summary>
    public static JigdoPartsIndex Load(string jigdoPath) =>
        new(JigdoFileReader.ReadFile(jigdoPath));

    /// <summary>Parse a gzip-compressed jigdo stream.</summary>
    public static JigdoPartsIndex Load(Stream gzipStream) =>
        new(JigdoFileReader.ReadStream(gzipStream));
}
