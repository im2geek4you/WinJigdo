namespace Jigdo;

/// <summary>Writes jigit-compatible .template files (prefix + compressed data + DESC tail).</summary>
public static class JigdoTemplateWriter
{
    /// <summary>
    /// Write a complete template file. <paramref name="compressedDataRegion"/> is the byte range starting at the first DATA/BZIP chunk through the byte before <paramref name="descTail"/>.
    /// </summary>
    public static void WriteFile(string path, ReadOnlySpan<byte> prefixBytes, ReadOnlySpan<byte> compressedDataRegion, ReadOnlySpan<byte> descTail)
    {
        using var fs = File.Create(path);
        fs.Write(prefixBytes);
        fs.Write(compressedDataRegion);
        fs.Write(descTail);
    }

    /// <summary>Serialize descriptor entries and write a template when combined with prefix and compressed data (see <see cref="WriteFile"/>).</summary>
    public static byte[] SerializeDescSection(IReadOnlyList<TemplateDescriptorEntry> entries) =>
        TemplateDescriptorSerializer.SerializeDescSection(entries);
}
