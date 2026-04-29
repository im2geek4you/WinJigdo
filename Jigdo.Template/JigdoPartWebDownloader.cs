using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace Jigdo;

/// <summary>Downloads jigdo parts over HTTP(S) into a cache directory with SHA256 verification.</summary>
public sealed class JigdoPartWebDownloader : IDisposable
{
    private readonly HttpClient _http;
    private readonly bool _disposeHttp;
    private readonly JigdoFileDocument _jigdo;
    private readonly string _cacheDirectory;

    public JigdoPartWebDownloader(JigdoFileDocument jigdo, string cacheDirectory, HttpClient? httpClient = null)
    {
        _jigdo = jigdo ?? throw new ArgumentNullException(nameof(jigdo));
        _cacheDirectory = cacheDirectory ?? throw new ArgumentNullException(nameof(cacheDirectory));
        if (httpClient != null)
        {
            _http = httpClient;
            _disposeHttp = false;
        }
        else
        {
            _http = new HttpClient { Timeout = TimeSpan.FromMinutes(30) };
            _disposeHttp = true;
        }

        Directory.CreateDirectory(_cacheDirectory);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposeHttp)
            _http.Dispose();
    }

    /// <summary>Default file name in cache: SHA256 hex + <c>.bin</c>.</summary>
    public static string CacheFileNameForChecksum(string checksumBase64)
    {
        Span<byte> buf = stackalloc byte[32];
        int n = JigdoBase64.Decode(checksumBase64.AsSpan(), buf);
        if (n != 32)
            throw new ArgumentException("Checksum must decode to 32 bytes (SHA256).", nameof(checksumBase64));
        return Convert.ToHexString(buf[..32]).ToLowerInvariant() + ".part";
    }

    /// <summary>Path to the cached file for this checksum if it exists and has the expected size.</summary>
    public string? TryGetCachedPath(string checksumBase64, ulong expectedSizeBytes)
    {
        string path = Path.Combine(_cacheDirectory, CacheFileNameForChecksum(checksumBase64));
        if (!File.Exists(path))
            return null;
        if ((ulong)new FileInfo(path).Length != expectedSizeBytes)
            return null;
        return path;
    }

    /// <summary>
    /// Download one part if missing or wrong size. Uses mirror URLs for <see cref="JigdoPartEntry.MirrorLabel"/> in file order.
    /// </summary>
    public async Task DownloadPartAsync(
        string checksumBase64,
        ulong expectedSizeBytes,
        IProgress<JigdoDownloadProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (!_jigdo.Parts.TryGetValue(checksumBase64, out JigdoPartEntry part))
            throw new KeyNotFoundException("Checksum not listed in jigdo [Parts]: " + checksumBase64);

        string dest = Path.Combine(_cacheDirectory, CacheFileNameForChecksum(checksumBase64));
        if (File.Exists(dest) && (ulong)new FileInfo(dest).Length == expectedSizeBytes)
        {
            if (VerifyFileSha256(dest, checksumBase64))
                return;
            File.Delete(dest);
        }

        var servers = JigdoMirrorUrls.GetServerLinesForMirror(_jigdo, part.MirrorLabel);
        if (servers.Count == 0)
            throw new InvalidOperationException(
                "No [Servers] URL for mirror label \"" + part.MirrorLabel + "\". Add a server line or use a local resolver.");

        Exception? last = null;
        foreach (var server in servers)
        {
            Uri url = JigdoMirrorUrls.CombinePartUrl(server.BaseUrl, part.RelativePath);
            progress?.Report(new JigdoDownloadProgress(part.RelativePath, url, 0, expectedSizeBytes, tryMirror: server.TryLast, part.PartIndex, _jigdo.Parts.Count));

            string temp = Path.Combine(_cacheDirectory, ".tmp_" + Guid.NewGuid().ToString("N"));
            try
            {
                await DownloadToFileAsync(url, temp, expectedSizeBytes, progress, part.RelativePath, part.PartIndex, _jigdo.Parts.Count, cancellationToken)
                    .ConfigureAwait(false);

                if (!VerifyFileSha256(temp, checksumBase64))
                {
                    try { File.Delete(temp); } catch { /* ignore */ }
                    throw new InvalidDataException("SHA256 mismatch after download: " + url);
                }

                if (File.Exists(dest))
                    File.Delete(dest);
                File.Move(temp, dest);
                return;
            }
            catch (Exception ex)
            {
                last = ex;
                try
                {
                    if (File.Exists(temp))
                        File.Delete(temp);
                }
                catch { /* ignore */ }
            }
        }

        throw new IOException("All mirror URLs failed for " + part.RelativePath + ".", last);
    }

    /// <summary>Download every part in the jigdo file (requires expected sizes from <see cref="JigdoTemplateChecksumSizes"/>).</summary>
    public async Task DownloadAllPartsAsync(
        IReadOnlyDictionary<string, ulong> checksumToSize,
        IProgress<JigdoDownloadProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        foreach (var kv in _jigdo.Parts)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!checksumToSize.TryGetValue(kv.Key, out ulong size))
                throw new KeyNotFoundException("Template has no SHA256 match for checksum: " + kv.Key);

            await DownloadPartAsync(kv.Key, size, progress, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task DownloadToFileAsync(
        Uri url,
        string tempPath,
        ulong expectedSize,
        IProgress<JigdoDownloadProgress>? progress,
        string relativePathForProgress,
        int partIndex,
        int totalParts,
        CancellationToken cancellationToken)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.UserAgent.ParseAdd("Jigdo.Template/1.0 (+https://github.com/)");

        using HttpResponseMessage resp = await _http.SendAsync(
            req,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken).ConfigureAwait(false);

        resp.EnsureSuccessStatusCode();

        await using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, useAsync: true))
        await using (Stream net = await resp.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
        {
            var buffer = new byte[65536];
            ulong total = 0;
            int read;
            while ((read = await net.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false)) > 0)
            {
                await fs.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
                total += (ulong)read;
                progress?.Report(new JigdoDownloadProgress(relativePathForProgress, url, total, expectedSize, tryMirror: false, partIndex, totalParts));
            }

            if (total != expectedSize)
                throw new IOException($"Downloaded {total} bytes, expected {expectedSize} for {url}.");
        }
    }

    private static bool VerifyFileSha256(string path, string checksumBase64)
    {
        using var sha = SHA256.Create();
        using var fs = File.OpenRead(path);
        byte[] hash = sha.ComputeHash(fs);
        Span<byte> expect = stackalloc byte[32];
        int n = JigdoBase64.Decode(checksumBase64.AsSpan(), expect);
        if (n != 32)
            return false;
        return hash.AsSpan().SequenceEqual(expect);
    }
}

/// <summary>Progress for a single file download.</summary>
public readonly struct JigdoDownloadProgress
{
    public JigdoDownloadProgress(string relativePath, Uri url, ulong bytesReceived, ulong totalBytes, bool tryMirror, int partIndex, int totalParts)
    {
        RelativePath = relativePath;
        Url = url;
        BytesReceived = bytesReceived;
        TotalBytes = totalBytes;
        TryLastMirror = tryMirror;
        PartIndex = partIndex;
        TotalParts = totalParts;
    }

    public string RelativePath { get; }
    public Uri Url { get; }
    public ulong BytesReceived { get; }
    public ulong TotalBytes { get; }
    public bool TryLastMirror { get; }
    public int PartIndex { get; }
    public int TotalParts { get; }
}
