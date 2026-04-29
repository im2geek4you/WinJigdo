using System.Text;

namespace Jigdo;

/// <summary>
/// Jigdo "pseudo-base64": alphabet A-Za-z0-9-_ packing bits MSB-first (see jigit jig-base64.c intent;
/// the published loop uses <c>bits += 8</c> style packing).
/// </summary>
public static class JigdoBase64
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

    /// <summary>Encode raw bytes to jigdo base64.</summary>
    public static string Encode(ReadOnlySpan<byte> data)
    {
        if (data.Length == 0)
            return string.Empty;

        var sb = new StringBuilder((data.Length * 8 + 5) / 6);
        int buffer = 0;
        int bits = 0;

        foreach (byte b in data)
        {
            buffer = (buffer << 8) | b;
            bits += 8;
            while (bits >= 6)
            {
                bits -= 6;
                sb.Append(Alphabet[(buffer >> bits) & 63]);
            }
        }

        if (bits > 0)
        {
            buffer <<= 6 - bits;
            sb.Append(Alphabet[buffer & 63]);
        }

        return sb.ToString();
    }

    /// <summary>Decode jigdo base64 into a buffer. Returns bytes written.</summary>
    public static int Decode(ReadOnlySpan<char> text, Span<byte> destination)
    {
        int buffer = 0;
        int bits = 0;
        int outPos = 0;

        for (int i = 0; i < text.Length; i++)
        {
            int idx = CharToIndex(text[i]);
            if (idx < 0)
                continue;

            buffer = (buffer << 6) | idx;
            bits += 6;

            while (bits >= 8 && outPos < destination.Length)
            {
                bits -= 8;
                destination[outPos++] = (byte)((buffer >> bits) & 0xFF);
            }
        }

        return outPos;
    }

    private static int CharToIndex(int ch)
    {
        if (ch >= 'A' && ch <= 'Z') return ch - 'A';
        if (ch >= 'a' && ch <= 'z') return 26 + (ch - 'a');
        if (ch >= '0' && ch <= '9') return 52 + (ch - '0');
        if (ch == '-') return 62;
        if (ch == '_') return 63;
        return -1;
    }
}
