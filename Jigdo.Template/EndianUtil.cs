using System.Buffers.Binary;

namespace Jigdo;

internal static class EndianUtil
{
    public static ulong ReadLe48(ReadOnlySpan<byte> b)
    {
        if (b.Length < 6)
            throw new ArgumentException("Need 6 bytes for LE48.");
        ulong v = BinaryPrimitives.ReadUInt32LittleEndian(b);
        v |= (ulong)BinaryPrimitives.ReadUInt16LittleEndian(b.Slice(4)) << 32;
        return v;
    }

    public static void WriteLe48(Span<byte> dest, ulong value)
    {
        if (dest.Length < 6)
            throw new ArgumentException("Need 6 bytes for LE48.");
        BinaryPrimitives.WriteUInt32LittleEndian(dest, (uint)(value & 0xFFFFFFFFu));
        BinaryPrimitives.WriteUInt16LittleEndian(dest.Slice(4), (ushort)((value >> 32) & 0xFFFF));
    }

    public static ulong ReadLe64(ReadOnlySpan<byte> b)
    {
        if (b.Length < 8)
            throw new ArgumentException("Need 8 bytes for LE64.");
        return BinaryPrimitives.ReadUInt64LittleEndian(b);
    }

    public static void WriteLe64(Span<byte> dest, ulong value)
    {
        if (dest.Length < 8)
            throw new ArgumentException("Need 8 bytes for LE64.");
        BinaryPrimitives.WriteUInt64LittleEndian(dest, value);
    }

    public static uint ReadLe32(ReadOnlySpan<byte> b)
    {
        if (b.Length < 4)
            throw new ArgumentException("Need 4 bytes for LE32.");
        return BinaryPrimitives.ReadUInt32LittleEndian(b);
    }

    public static void WriteLe32(Span<byte> dest, uint value)
    {
        if (dest.Length < 4)
            throw new ArgumentException("Need 4 bytes for LE32.");
        BinaryPrimitives.WriteUInt32LittleEndian(dest, value);
    }
}
