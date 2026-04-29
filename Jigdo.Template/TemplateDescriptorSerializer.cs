namespace Jigdo;

/// <summary>Serializes descriptor entries to the binary DESC section (jigit-compatible layout).</summary>
public static class TemplateDescriptorSerializer
{
    public static int GetEntrySize(TemplateDescriptorEntry entry) =>
        entry switch
        {
            TemplateDataEntry => JigdoSizes.DescEntryData,
            TemplateMatchSha256Entry => JigdoSizes.DescEntryMatchSha256,
            TemplateMatchMd5Entry => JigdoSizes.DescEntryMatchMd5,
            TemplateImageSha256Entry => JigdoSizes.DescEntryImageSha256,
            TemplateImageMd5Entry => JigdoSizes.DescEntryImageMd5,
            _ => throw new ArgumentException("Unsupported entry type.", nameof(entry)),
        };

    /// <summary>Build the full DESC tail: <c>DESC</c> + length + entries + footer pointer.</summary>
    public static byte[] SerializeDescSection(IReadOnlyList<TemplateDescriptorEntry> entries)
    {
        int body = 0;
        foreach (var e in entries)
            body += GetEntrySize(e);

        int descSize = 4 + 6 + body + 6;
        var buf = new byte[descSize];
        "DESC"u8.CopyTo(buf);

        EndianUtil.WriteLe48(buf.AsSpan(4, 6), (ulong)descSize);

        int pos = 10;
        foreach (var e in entries)
            pos += WriteEntry(buf.AsSpan(pos), e);

        if (pos != descSize - 6)
            throw new InvalidOperationException("Serialized size mismatch.");

        EndianUtil.WriteLe48(buf.AsSpan(descSize - 6, 6), (ulong)descSize);
        return buf;
    }

    private static int WriteEntry(Span<byte> dest, TemplateDescriptorEntry entry)
    {
        switch (entry)
        {
            case TemplateDataEntry d:
                dest[0] = JigdoBlockType.Data;
                EndianUtil.WriteLe48(dest.Slice(1, 6), d.ExtentBytes);
                return JigdoSizes.DescEntryData;

            case TemplateMatchSha256Entry m:
                dest[0] = JigdoBlockType.MatchSha256;
                EndianUtil.WriteLe48(dest.Slice(1, 6), m.ExtentBytes);
                EndianUtil.WriteLe64(dest.Slice(7, 8), m.RsyncDigest);
                m.Sha256.Span.CopyTo(dest.Slice(15, JigdoSizes.Sha256Bytes));
                return JigdoSizes.DescEntryMatchSha256;

            case TemplateMatchMd5Entry m:
                dest[0] = JigdoBlockType.MatchMd5;
                EndianUtil.WriteLe48(dest.Slice(1, 6), m.ExtentBytes);
                EndianUtil.WriteLe64(dest.Slice(7, 8), m.RsyncDigest);
                m.Md5.Span.CopyTo(dest.Slice(15, 16));
                return JigdoSizes.DescEntryMatchMd5;

            case TemplateImageSha256Entry i:
                dest[0] = JigdoBlockType.ImageSha256;
                EndianUtil.WriteLe48(dest.Slice(1, 6), i.ImageLength);
                i.Sha256.Span.CopyTo(dest.Slice(7, JigdoSizes.Sha256Bytes));
                EndianUtil.WriteLe32(dest.Slice(7 + JigdoSizes.Sha256Bytes, 4), i.RsyncBlockLength);
                return JigdoSizes.DescEntryImageSha256;

            case TemplateImageMd5Entry i:
                dest[0] = JigdoBlockType.ImageMd5;
                EndianUtil.WriteLe48(dest.Slice(1, 6), i.ImageLength);
                i.Md5.Span.CopyTo(dest.Slice(7, 16));
                EndianUtil.WriteLe32(dest.Slice(23, 4), i.RsyncBlockLength);
                return JigdoSizes.DescEntryImageMd5;

            default:
                throw new ArgumentException("Unsupported entry type.", nameof(entry));
        }
    }
}
