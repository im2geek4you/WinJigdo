namespace Jigdo;

/// <summary>One entry from the template DESC section.</summary>
public abstract class TemplateDescriptorEntry
{
    public abstract byte BlockType { get; }
}

/// <summary>Unmatched template data (compressed in the DATA section of the file).</summary>
public sealed class TemplateDataEntry : TemplateDescriptorEntry
{
    public override byte BlockType => JigdoBlockType.Data;

    /// <summary>Number of bytes contributed to the output image.</summary>
    public ulong ExtentBytes { get; }

    public TemplateDataEntry(ulong extentBytes) => ExtentBytes = extentBytes;
}

/// <summary>Match a span of the output image to a file identified by SHA-256.</summary>
public sealed class TemplateMatchSha256Entry : TemplateDescriptorEntry
{
    public override byte BlockType => JigdoBlockType.MatchSha256;

    public ulong ExtentBytes { get; }
    /// <summary>8-byte rsync rolling checksum / digest stored in the template.</summary>
    public ulong RsyncDigest { get; }
    public ReadOnlyMemory<byte> Sha256 { get; }

    public TemplateMatchSha256Entry(ulong extentBytes, ulong rsyncDigest, ReadOnlyMemory<byte> sha256)
    {
        if (sha256.Length != JigdoSizes.Sha256Bytes)
            throw new ArgumentException("SHA256 must be 32 bytes.", nameof(sha256));
        ExtentBytes = extentBytes;
        RsyncDigest = rsyncDigest;
        Sha256 = sha256;
    }
}

/// <summary>Global image metadata (expected size and whole-image SHA-256).</summary>
public sealed class TemplateImageSha256Entry : TemplateDescriptorEntry
{
    public override byte BlockType => JigdoBlockType.ImageSha256;

    public ulong ImageLength { get; }
    public ReadOnlyMemory<byte> Sha256 { get; }
    public uint RsyncBlockLength { get; }

    public TemplateImageSha256Entry(ulong imageLength, ReadOnlyMemory<byte> sha256, uint rsyncBlockLength)
    {
        if (sha256.Length != JigdoSizes.Sha256Bytes)
            throw new ArgumentException("SHA256 must be 32 bytes.", nameof(sha256));
        ImageLength = imageLength;
        Sha256 = sha256;
        RsyncBlockLength = rsyncBlockLength;
    }
}

/// <summary>MD5 match entry — parsed for round-trip; not used when building with SHA256-only policy.</summary>
public sealed class TemplateMatchMd5Entry : TemplateDescriptorEntry
{
    public override byte BlockType => JigdoBlockType.MatchMd5;

    public ulong ExtentBytes { get; }
    public ulong RsyncDigest { get; }
    public ReadOnlyMemory<byte> Md5 { get; }

    public TemplateMatchMd5Entry(ulong extentBytes, ulong rsyncDigest, ReadOnlyMemory<byte> md5)
    {
        if (md5.Length != 16)
            throw new ArgumentException("MD5 must be 16 bytes.", nameof(md5));
        ExtentBytes = extentBytes;
        RsyncDigest = rsyncDigest;
        Md5 = md5;
    }
}

/// <summary>Image metadata using MD5 — parsed for round-trip.</summary>
public sealed class TemplateImageMd5Entry : TemplateDescriptorEntry
{
    public override byte BlockType => JigdoBlockType.ImageMd5;

    public ulong ImageLength { get; }
    public ReadOnlyMemory<byte> Md5 { get; }
    public uint RsyncBlockLength { get; }

    public TemplateImageMd5Entry(ulong imageLength, ReadOnlyMemory<byte> md5, uint rsyncBlockLength)
    {
        if (md5.Length != 16)
            throw new ArgumentException("MD5 must be 16 bytes.", nameof(md5));
        ImageLength = imageLength;
        Md5 = md5;
        RsyncBlockLength = rsyncBlockLength;
    }
}
