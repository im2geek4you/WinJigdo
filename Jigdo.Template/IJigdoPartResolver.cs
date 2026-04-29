namespace Jigdo;

/// <summary>
/// Resolves jigdo "parts" (files identified by SHA-256) to local content when building an image.
/// </summary>
public interface IJigdoPartResolver
{
    /// <summary>
    /// Open a readable stream for the file whose full-file SHA-256 is <paramref name="sha256"/>.
    /// The file must be exactly <paramref name="fileSizeBytes"/> bytes (as in the mirror).
    /// Return <c>null</c> if the part is missing.
    /// </summary>
    Stream? TryOpenPart(ReadOnlySpan<byte> sha256, ulong fileSizeBytes);
}
