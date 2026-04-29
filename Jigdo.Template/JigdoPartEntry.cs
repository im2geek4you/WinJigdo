namespace Jigdo;

/// <summary>One line from <c>[Parts]</c>: checksum, mirror label, and path relative to that mirror.</summary>
public readonly record struct JigdoPartEntry(string ChecksumBase64, string MirrorLabel, string RelativePath, int PartIndex);
