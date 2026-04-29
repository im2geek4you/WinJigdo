namespace Jigdo;

/// <summary>Builds download URLs from <see cref="JigdoFileDocument"/> server lines and part paths.</summary>
public static class JigdoMirrorUrls
{
    /// <summary>
    /// All <see cref="JigdoServerLine"/> entries for <paramref name="mirrorLabel"/>, in file order
    /// (primary mirrors first; <c>--try-last</c> lines appear in-place as in the file).
    /// </summary>
    public static IReadOnlyList<JigdoServerLine> GetServerLinesForMirror(JigdoFileDocument doc, string mirrorLabel)
    {
        var list = new List<JigdoServerLine>();
        foreach (var line in doc.ServerLines)
        {
            if (line.MirrorLabel.Equals(mirrorLabel, StringComparison.Ordinal))
                list.Add(line);
        }

        return list;
    }

    /// <summary>Combine a server base URL with the relative path from a <see cref="JigdoPartEntry"/>.</summary>
    public static Uri CombinePartUrl(Uri serverBase, string relativePath)
    {
        ArgumentNullException.ThrowIfNull(serverBase);
        if (string.IsNullOrEmpty(relativePath))
            throw new ArgumentException("Relative path is required.", nameof(relativePath));

        if (!serverBase.AbsoluteUri.EndsWith('/'))
            serverBase = new Uri(serverBase.AbsoluteUri + "/");

        return new Uri(serverBase, relativePath);
    }
}
