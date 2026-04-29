namespace Jigdo;

/// <summary>Parsed .template file: header text, descriptor entries, and layout offsets.</summary>
public sealed class JigdoTemplateDocument
{
    /// <summary>Full text header line (without trailing newline), e.g. <c>JigsawDownload template 1.0 genisoimage</c>.</summary>
    public string HeaderLine { get; init; } = "";

    /// <summary>Raw bytes before the first DATA/BZIP chunk (includes header line and padding).</summary>
    public ReadOnlyMemory<byte> PrefixBytes { get; init; }

    /// <summary>Offset in the file where the DESC tail begins.</summary>
    public long DescStartOffset { get; init; }

    /// <summary>Raw DESC tail (from <see cref="DescStartOffset"/> to EOF).</summary>
    public ReadOnlyMemory<byte> DescTailBytes { get; init; }

    public IReadOnlyList<TemplateDescriptorEntry> Entries { get; init; } = Array.Empty<TemplateDescriptorEntry>();

    /// <summary>IMAGE_SHA256 block if present.</summary>
    public TemplateImageSha256Entry? ImageSha256 { get; init; }

    /// <summary>Expected output size from IMAGE_* block (prefers SHA256).</summary>
    public ulong? ExpectedImageLength =>
        ImageSha256?.ImageLength;

    /// <summary>SHA256 of the full image from the template, if present.</summary>
    public ReadOnlyMemory<byte>? ExpectedImageSha256 =>
        ImageSha256?.Sha256;
}
