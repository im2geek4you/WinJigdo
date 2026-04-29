namespace Jigdo;

/// <summary>Jigdo template descriptor block types (jigit jigdo.h).</summary>
public static class JigdoBlockType
{
    public const byte Data = 2;
    public const byte ImageMd5 = 5;
    public const byte MatchMd5 = 6;
    public const byte WrittenMd5 = 7;
    public const byte ImageSha256 = 8;
    public const byte MatchSha256 = 9;
    public const byte WrittenSha256 = 10;
}

/// <summary>Compression used inside DATA/BZIP chunks.</summary>
public enum JigdoCompressionKind
{
    GZip = 1,
    BZip2 = 2,
}

public static class JigdoSizes
{
    public const int Sha256Bytes = 32;
    public const int RsyncDigestBytes = 8;
    /// <summary>Jigdo pseudo-base64 length for SHA-256 (ceil(256/6) chars, no null).</summary>
    public const int Base64Sha256Chars = 43;

    public const int DescEntryData = 7;
    /// <summary>MD5 match descriptor size (type + le48 + rsync + md5).</summary>
    public const int DescEntryMatchMd5 = 31;
    public const int DescEntryMatchSha256 = 47;
    public const int DescEntryImageMd5 = 27;
    public const int DescEntryImageSha256 = 43;
}
