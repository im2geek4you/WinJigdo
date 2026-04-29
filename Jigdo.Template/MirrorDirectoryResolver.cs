namespace Jigdo;

/// <summary>
/// <see cref="IJigdoPartResolver"/> that maps checksums to files under one or more mirror roots
/// (using entries from <see cref="JigdoPartsIndex"/>).
/// </summary>
public sealed class MirrorDirectoryResolver : IJigdoPartResolver
{
    private readonly JigdoPartsIndex _index;
    private readonly IReadOnlyDictionary<string, string> _mirrorRoots;

    /// <param name="index">Parsed .jigdo parts list.</param>
    /// <param name="mirrorRoots">Map mirror label (e.g. <c>Debian</c>) to local root directory.</param>
    public MirrorDirectoryResolver(JigdoPartsIndex index, IReadOnlyDictionary<string, string> mirrorRoots)
    {
        _index = index;
        _mirrorRoots = mirrorRoots;
    }

    /// <inheritdoc cref="MirrorDirectoryResolver(JigdoPartsIndex, IReadOnlyDictionary{string, string})" />
    public MirrorDirectoryResolver(JigdoFileDocument jigdo, IReadOnlyDictionary<string, string> mirrorRoots)
        : this(new JigdoPartsIndex(jigdo), mirrorRoots)
    {
    }

    public Stream? TryOpenPart(ReadOnlySpan<byte> sha256, ulong fileSizeBytes)
    {
        string key = JigdoBase64.Encode(sha256);
        if (!_index.PartsByChecksum.TryGetValue(key, out JigdoPartEntry entry))
            return null;

        if (!_mirrorRoots.TryGetValue(entry.MirrorLabel, out string? root))
            return null;

        string full = Path.Combine(root, entry.RelativePath.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(full))
            return null;

        var fs = File.OpenRead(full);
        try
        {
            if ((ulong)fs.Length != fileSizeBytes)
            {
                fs.Dispose();
                return null;
            }

            return fs;
        }
        catch
        {
            fs.Dispose();
            throw;
        }
    }
}
