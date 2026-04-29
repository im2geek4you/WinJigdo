namespace Jigdo;

/// <summary>Reads Debian/jigit-style .template files.</summary>
public static class JigdoTemplateReader
{
    /// <summary>Load and parse a template file from disk.</summary>
    public static JigdoTemplateDocument ReadFile(string path)
    {
        using var fs = File.OpenRead(path);
        return ReadStream(fs);
    }

    /// <summary>Parse a template from a seekable stream (full content).</summary>
    public static JigdoTemplateDocument ReadStream(Stream stream)
    {
        if (!stream.CanSeek)
            throw new ArgumentException("Stream must be seekable.", nameof(stream));

        long len = stream.Length;
        if (len < 16)
            throw new InvalidDataException("Template file too small.");

        stream.Position = len - 6;
        Span<byte> foot = stackalloc byte[6];
        if (stream.Read(foot) != 6)
            throw new EndOfStreamException();

        ulong descSize = EndianUtil.ReadLe48(foot);
        if (descSize > (ulong)len)
            throw new InvalidDataException("Invalid DESC size in footer.");

        long descStart = len - (long)descSize;
        if (descStart < 0)
            throw new InvalidDataException("Invalid DESC start offset.");

        var descTail = new byte[descSize];
        stream.Position = descStart;
        stream.ReadExactly(descTail);

        var entries = TemplateDescriptorParser.Parse(descTail);
        TemplateImageSha256Entry? img = null;
        foreach (var e in entries)
        {
            if (e is TemplateImageSha256Entry t)
            {
                img = t;
                break;
            }
        }

        long firstChunk = TemplateCompressedDataReader.FindFirstChunkOffset(stream);
        stream.Position = 0;
        var prefix = new byte[firstChunk];
        if (firstChunk > 0)
            stream.ReadExactly(prefix);

        string headerLine = "";
        var nl = MemoryExtensions.IndexOf(prefix.AsSpan(), (byte)'\n');
        if (nl >= 0)
            headerLine = System.Text.Encoding.UTF8.GetString(prefix, 0, nl);

        return new JigdoTemplateDocument
        {
            HeaderLine = headerLine,
            PrefixBytes = prefix,
            DescStartOffset = descStart,
            DescTailBytes = descTail,
            Entries = entries,
            ImageSha256 = img,
        };
    }
}
