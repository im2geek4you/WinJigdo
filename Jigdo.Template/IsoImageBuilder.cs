using System.Security.Cryptography;

namespace Jigdo;

/// <summary>
/// Reconstructs an image from a .template and locally available parts (jigit <c>mkimage</c> behaviour for SHA-256).
/// </summary>
public static class IsoImageBuilder
{
    /// <summary>Build an image from a template file and write it to <paramref name="output"/>.</summary>
    public static void BuildImage(
        string templatePath,
        Stream output,
        IJigdoPartResolver parts,
        bool verifyImageSha256 = true)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(parts);

        using var fs = File.OpenRead(templatePath);
        var doc = JigdoTemplateReader.ReadStream(fs);
        BuildImage(fs, doc, output, parts, verifyImageSha256);
    }

    /// <summary>Build using an already-parsed document and an open seekable template stream positioned at 0.</summary>
    public static void BuildImage(
        Stream templateStream,
        JigdoTemplateDocument doc,
        Stream output,
        IJigdoPartResolver parts,
        bool verifyImageSha256 = true)
    {
        ArgumentNullException.ThrowIfNull(templateStream);
        ArgumentNullException.ThrowIfNull(doc);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(parts);

        if (!templateStream.CanSeek)
            throw new ArgumentException("Template stream must be seekable.", nameof(templateStream));

        templateStream.Position = 0;
        var reader = new TemplateCompressedDataReader(templateStream);
        byte[]? chunk = null;
        int chunkPos = 0;

        void DrainTemplateData(long byteCount, Stream outStream, IncrementalHash? imageHash)
        {
            while (byteCount > 0)
            {
                if (chunk == null || chunkPos >= chunk.Length)
                {
                    if (!reader.TryReadNext(out byte[]? nextChunk, out _) || nextChunk == null)
                        throw new InvalidDataException("Unexpected end of compressed template data.");
                    chunk = nextChunk;
                    chunkPos = 0;
                }

                int n = (int)Math.Min(byteCount, chunk!.Length - chunkPos);
                ReadOnlySpan<byte> slice = chunk.AsSpan(chunkPos, n);
                outStream.Write(slice);
                imageHash?.AppendData(slice);
                chunkPos += n;
                byteCount -= n;
            }
        }

        IncrementalHash? shaAll = verifyImageSha256 && doc.ImageSha256 != null
            ? IncrementalHash.CreateHash(HashAlgorithmName.SHA256)
            : null;

        if (verifyImageSha256 && doc.ImageSha256 == null)
            throw new InvalidOperationException("Template has no IMAGE_SHA256 block; set verifyImageSha256 to false or use a SHA256-based template.");

        foreach (var entry in doc.Entries)
        {
            switch (entry)
            {
                case TemplateDataEntry d:
                    DrainTemplateData((long)d.ExtentBytes, output, shaAll);
                    break;

                case TemplateMatchSha256Entry m:
                {
                    using Stream? part = parts.TryOpenPart(m.Sha256.Span, m.ExtentBytes);
                    if (part == null)
                        throw new FileNotFoundException(
                            "Missing jigdo part for SHA256 " + JigdoBase64.Encode(m.Sha256.Span) + ".");

                    var buffer = new byte[65536];
                    long remain = (long)m.ExtentBytes;
                    using IncrementalHash partHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
                    while (remain > 0)
                    {
                        int want = (int)Math.Min(buffer.Length, remain);
                        int r = part.Read(buffer, 0, want);
                        if (r == 0)
                            throw new EndOfStreamException("Part file too short for template extent.");
                        ReadOnlySpan<byte> got = buffer.AsSpan(0, r);
                        output.Write(got);
                        shaAll?.AppendData(got);
                        partHash.AppendData(got);
                        remain -= r;
                    }

                    if (!m.Sha256.Span.SequenceEqual(partHash.GetHashAndReset()))
                        throw new InvalidDataException("SHA256 mismatch for jigdo part (expected by template).");
                    break;
                }

                case TemplateImageSha256Entry:
                case TemplateImageMd5Entry:
                    break;

                case TemplateMatchMd5Entry:
                    throw new NotSupportedException(
                        "This template uses MD5 file matches; only SHA256 matches are supported for building.");

                default:
                    throw new InvalidOperationException("Unknown descriptor entry type: " + entry.GetType().Name);
            }
        }

        if (shaAll != null && doc.ImageSha256 != null)
        {
            ReadOnlySpan<byte> expect = doc.ImageSha256.Sha256.Span;
            if (!expect.SequenceEqual(shaAll.GetHashAndReset()))
                throw new InvalidDataException("Output image SHA256 does not match IMAGE_SHA256 in template.");
        }
    }
}
