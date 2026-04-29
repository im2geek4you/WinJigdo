using System.IO.Compression;
using SharpCompress.Compressors.BZip2;

namespace Jigdo;

/// <summary>
/// Sequentially reads compressed DATA/BZIP chunks from a template file (same layout as jigit <c>read_data_block</c>).
/// </summary>
public sealed class TemplateCompressedDataReader
{
    private readonly Stream _stream;
    private long _chunkStartOffset;

    public TemplateCompressedDataReader(Stream stream, long? firstChunkOffset = null)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        if (!stream.CanSeek)
            throw new ArgumentException("Stream must be seekable.", nameof(stream));
        _chunkStartOffset = firstChunkOffset ?? FindFirstChunkOffset(stream);
    }

    /// <summary>File offset of the next DATA/BZIP chunk (for debugging).</summary>
    public long NextChunkOffset => _chunkStartOffset;

    /// <summary>Find the first occurrence of DATA or BZIP magic in the first <paramref name="scanLimit"/> bytes.</summary>
    public static long FindFirstChunkOffset(Stream stream, int scanLimit = 65536)
    {
        long len = stream.Length;
        int toRead = (int)Math.Min(scanLimit, len);
        var buf = new byte[toRead];
        stream.Position = 0;
        int n = stream.ReadAtLeast(buf, toRead, throwOnEndOfStream: false);
        ReadOnlySpan<byte> span = buf.AsSpan(0, n);
        int iData = span.IndexOf("DATA"u8);
        int iBzip = span.IndexOf("BZIP"u8);
        int i = -1;
        if (iData >= 0 && iBzip >= 0)
            i = Math.Min(iData, iBzip);
        else
            i = Math.Max(iData, iBzip);
        if (i < 0)
            throw new InvalidDataException("No DATA or BZIP compressed block found in template.");
        return i;
    }

    /// <summary>
    /// Read and decompress the next chunk. Returns false at end of compressed region (e.g. at DESC).
    /// </summary>
    public bool TryReadNext(out byte[]? uncompressed, out JigdoCompressionKind compression)
    {
        uncompressed = null;
        compression = default;

        if (_chunkStartOffset >= _stream.Length - 16)
            return false;

        _stream.Position = _chunkStartOffset;
        Span<byte> hdr = stackalloc byte[16];
        if (_stream.Read(hdr) != 16)
            return false;

        bool isZlib = hdr.StartsWith("DATA"u8);
        bool isBzip = hdr.StartsWith("BZIP"u8);
        if (!isZlib && !isBzip)
            return false;

        ulong chunkSpan = EndianUtil.ReadLe48(hdr.Slice(4, 6));
        ulong uncompressedLen = EndianUtil.ReadLe48(hdr.Slice(10, 6));

        if (chunkSpan < 16)
            throw new InvalidDataException("Invalid chunk span (< 16).");

        int payloadLen = (int)(chunkSpan - 16);
        if (payloadLen < 0 || (ulong)_chunkStartOffset + chunkSpan > (ulong)_stream.Length)
            throw new InvalidDataException("Chunk extends past end of file.");

        var payload = new byte[payloadLen];
        _stream.ReadExactly(payload);

        _chunkStartOffset += (long)chunkSpan;

        if (isZlib)
        {
            compression = JigdoCompressionKind.GZip;
            uncompressed = DecompressZlib(payload, uncompressedLen);
            return true;
        }

        compression = JigdoCompressionKind.BZip2;
        uncompressed = DecompressBzip2(payload, uncompressedLen);
        return true;
    }

    private static byte[] DecompressZlib(ReadOnlySpan<byte> payload, ulong expectedUncompressed)
    {
        using var ms = new MemoryStream(payload.ToArray());
        using var zs = new ZLibStream(ms, CompressionMode.Decompress);
        using var outMs = new MemoryStream((int)Math.Min(expectedUncompressed, int.MaxValue));
        zs.CopyTo(outMs);
        return outMs.ToArray();
    }

    private static byte[] DecompressBzip2(ReadOnlySpan<byte> payload, ulong expectedUncompressed)
    {
        using var ms = new MemoryStream(payload.ToArray());
        using var bz = new BZip2Stream(ms, SharpCompress.Compressors.CompressionMode.Decompress, decompressConcatenated: false);
        using var outMs = new MemoryStream((int)Math.Min(expectedUncompressed, int.MaxValue));
        bz.CopyTo(outMs);
        return outMs.ToArray();
    }

    /// <summary>Reset sequential reading to the first DATA/BZIP chunk.</summary>
    public void Rewind() => _chunkStartOffset = FindFirstChunkOffset(_stream);
}
