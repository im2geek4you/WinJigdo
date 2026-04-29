using System.IO.Compression;
using System.Text;

namespace Jigdo;

/// <summary>Parses <c>.jigdo</c> files (gzip-compressed text; sections <c>[Jigdo]</c>, <c>[Image]</c>, <c>[Parts]</c>, <c>[Servers]</c>).</summary>
public static class JigdoFileReader
{
    /// <summary>Read and parse a <c>.jigdo</c> file from disk (gzip).</summary>
    public static JigdoFileDocument ReadFile(string path)
    {
        using var fs = File.OpenRead(path);
        return ReadStream(fs);
    }

    /// <summary>Parse a gzip-compressed jigdo stream.</summary>
    public static JigdoFileDocument ReadStream(Stream gzipStream)
    {
        using var gz = new GZipStream(gzipStream, CompressionMode.Decompress, leaveOpen: true);
        using var reader = new StreamReader(gz, Encoding.UTF8);
        return LoadLines(ReadAllLines(reader));
    }

    /// <summary>Parse already-decompressed jigdo text (UTF-8).</summary>
    public static JigdoFileDocument ReadText(string text) => LoadLines(text.Split(['\r', '\n'], StringSplitOptions.None));

    private static List<string> ReadAllLines(TextReader reader)
    {
        var list = new List<string>();
        string? line;
        while ((line = reader.ReadLine()) != null)
            list.Add(line);
        return list;
    }

    private static JigdoFileDocument LoadLines(IReadOnlyList<string> lines)
    {
        string? section = null;
        string? version = null;
        string? generator = null;
        string? isoName = null;
        string? templateName = null;
        string? tmplSha = null;
        string? tmplMd5 = null;
        var parts = new Dictionary<string, JigdoPartEntry>(StringComparer.Ordinal);
        int partIndex = 0;
        var serverLines = new List<JigdoServerLine>();

        foreach (var raw in lines)
        {
            var line = raw.TrimEnd();
            if (line.Length == 0)
                continue;

            if (line[0] == '#' || line[0] == ';')
                continue;

            if (line[0] == '[' && line[^1] == ']')
            {
                section = line[1..^1].Trim();
                continue;
            }

            if (section == null)
                continue;

            if (section.Equals("Jigdo", StringComparison.OrdinalIgnoreCase))
            {
                ParseKeyValue(line, out var k, out var v);
                if (k.Equals("Version", StringComparison.OrdinalIgnoreCase))
                    version = v;
                else if (k.Equals("Generator", StringComparison.OrdinalIgnoreCase))
                    generator = v;
            }
            else if (section.Equals("Image", StringComparison.OrdinalIgnoreCase))
            {
                ParseKeyValue(line, out var k, out var v);
                if (k.Equals("Filename", StringComparison.OrdinalIgnoreCase))
                    isoName = v;
                else if (k.Equals("Template", StringComparison.OrdinalIgnoreCase))
                    templateName = v;
                else if (k.Equals("Template-SHA256Sum", StringComparison.OrdinalIgnoreCase))
                    tmplSha = v.TrimEnd();
                else if (k.Equals("Template-MD5Sum", StringComparison.OrdinalIgnoreCase))
                    tmplMd5 = v.TrimEnd();
            }
            else if (section.Equals("Parts", StringComparison.OrdinalIgnoreCase))
            {
                partIndex++;
                ParsePartLine(line, parts, partIndex);
            }
            else if (section.Equals("Servers", StringComparison.OrdinalIgnoreCase))
            {
                ParseServerLine(line, serverLines);
            }
        }

        return new JigdoFileDocument
        {
            Version = version,
            Generator = generator,
            Image = new JigdoImageSection
            {
                IsoFilename = isoName,
                TemplateFilename = templateName,
                TemplateSha256SumBase64 = tmplSha,
                TemplateMd5SumBase64 = tmplMd5,
            },
            Parts = parts,
            ServerLines = serverLines,
        };
    }

    private static void ParseKeyValue(string line, out string key, out string value)
    {
        int eq = line.IndexOf('=');
        if (eq < 0)
        {
            key = line;
            value = "";
            return;
        }

        key = line[..eq].Trim();
        value = line[(eq + 1)..].Trim();
    }

    /// <summary>Format: <c>BASE64_CHECKSUM=mirrorLabel:relative/path</c> (first <c>=</c> and first <c>:</c> after it).</summary>
    private static void ParsePartLine(string line, Dictionary<string, JigdoPartEntry> parts, int partIndex)
    {
        int eq = line.IndexOf('=');
        if (eq <= 0)
            return;

        string checksum = line[..eq];
        string rest = line[(eq + 1)..];
        int col = rest.IndexOf(':');
        if (col <= 0)
            return;

        string mirror = rest[..col];
        string path = rest[(col + 1)..];
        parts[checksum] = new JigdoPartEntry(checksum, mirror, path, partIndex);
    }

    /// <summary>Format: <c>Label=http://host/base/</c> optionally followed by <c> --try-last</c>.</summary>
    private static void ParseServerLine(string line, List<JigdoServerLine> serverLines)
    {
        int eq = line.IndexOf('=');
        if (eq <= 0)
            return;

        string label = line[..eq].Trim();
        string rest = line[(eq + 1)..].Trim();
        bool tryLast = false;
        const string suffix = "--try-last";
        if (rest.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        {
            tryLast = true;
            rest = rest[..^suffix.Length].TrimEnd();
        }

        if (!Uri.TryCreate(rest, UriKind.Absolute, out Uri? uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return;

        serverLines.Add(new JigdoServerLine
        {
            MirrorLabel = label,
            BaseUrl = uri,
            TryLast = tryLast,
        });
    }
}
