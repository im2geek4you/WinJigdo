namespace Jigdo;

/// <summary>
/// <see cref="IJigdoPartResolver"/> that serves files from a download cache. Use
/// <see cref="JigdoPartWebDownloader"/> or <see cref="JigdoWebPipeline.DownloadPartsAsync"/> first to populate the cache.
/// </summary>
public sealed class JigdoHttpPartResolver : IJigdoPartResolver, IDisposable
{
    private readonly JigdoPartWebDownloader _downloader;

    public JigdoHttpPartResolver(JigdoFileDocument jigdo, string cacheDirectory, HttpClient? httpClient = null) =>
        _downloader = new JigdoPartWebDownloader(jigdo, cacheDirectory, httpClient);

    /// <inheritdoc />
    public Stream? TryOpenPart(ReadOnlySpan<byte> sha256, ulong fileSizeBytes)
    {
        string key = JigdoBase64.Encode(sha256);
        string? path = _downloader.TryGetCachedPath(key, fileSizeBytes);
        if (path == null)
            return null;
        return File.OpenRead(path);
    }

    /// <summary>Ensure the part is present (download if needed).</summary>
    public Task EnsurePartAsync(
        ReadOnlySpan<byte> sha256,
        ulong fileSizeBytes,
        IProgress<JigdoDownloadProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        string key = JigdoBase64.Encode(sha256);
        return _downloader.DownloadPartAsync(key, fileSizeBytes, progress, cancellationToken);
    }

    /// <summary>Download all parts listed in the jigdo file using sizes from the template.</summary>
    public Task DownloadAllPartsAsync(
        JigdoTemplateDocument template,
        IProgress<JigdoDownloadProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyDictionary<string, ulong> map = JigdoTemplateChecksumSizes.FromTemplate(template);
        return _downloader.DownloadAllPartsAsync(map, progress, cancellationToken);
    }

    /// <summary>Underlying downloader (e.g. for <see cref="JigdoPartWebDownloader.TryGetCachedPath"/>).</summary>
    public JigdoPartWebDownloader Downloader => _downloader;

    /// <inheritdoc />
    public void Dispose() => _downloader.Dispose();
}
