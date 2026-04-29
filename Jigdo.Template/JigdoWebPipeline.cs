namespace Jigdo;

/// <summary>High-level steps: download parts from <c>[Servers]</c> URLs, then build the ISO using the cache.</summary>
public static class JigdoWebPipeline
{
    /// <summary>Download all parts referenced by the template into <paramref name="partCacheDirectory"/>.</summary>
    public static async Task DownloadPartsAsync(
        string jigdoPath,
        string templatePath,
        string partCacheDirectory,
        IProgress<JigdoDownloadProgress>? progress = null,
        HttpClient? httpClient = null,
        CancellationToken cancellationToken = default) =>
        await DownloadPartsAsync(
            JigdoFileReader.ReadFile(jigdoPath),
            templatePath,
            partCacheDirectory,
            progress,
            httpClient,
            cancellationToken).ConfigureAwait(false);

    /// <summary>Download parts using an already-parsed jigdo document (e.g. after <see cref="JigdoFileDocument.WithPrependedServers"/>).</summary>
    public static async Task DownloadPartsAsync(
        JigdoFileDocument jigdo,
        string templatePath,
        string partCacheDirectory,
        IProgress<JigdoDownloadProgress>? progress = null,
        HttpClient? httpClient = null,
        CancellationToken cancellationToken = default)
    {
        JigdoTemplateDocument template = JigdoTemplateReader.ReadFile(templatePath);
        IReadOnlyDictionary<string, ulong> sizes = JigdoTemplateChecksumSizes.FromTemplate(template);

        using var downloader = new JigdoPartWebDownloader(jigdo, partCacheDirectory, httpClient);
        await downloader.DownloadAllPartsAsync(sizes, progress, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>After <see cref="DownloadPartsAsync"/>, builds the image from the template using cached downloads.</summary>
    public static void BuildImageFromCache(
        string templatePath,
        string jigdoPath,
        string partCacheDirectory,
        string outputImagePath,
        HttpClient? httpClient = null,
        bool verifyImageSha256 = true) =>
        BuildImageFromCache(
            JigdoFileReader.ReadFile(jigdoPath),
            templatePath,
            partCacheDirectory,
            outputImagePath,
            httpClient,
            verifyImageSha256);

    /// <inheritdoc cref="BuildImageFromCache(string, string, string, string, HttpClient?, bool)"/>
    public static void BuildImageFromCache(
        JigdoFileDocument jigdo,
        string templatePath,
        string partCacheDirectory,
        string outputImagePath,
        HttpClient? httpClient = null,
        bool verifyImageSha256 = true)
    {
        using (var resolver = new JigdoHttpPartResolver(jigdo, partCacheDirectory, httpClient))
        using (FileStream output = File.Create(outputImagePath))
            IsoImageBuilder.BuildImage(templatePath, output, resolver, verifyImageSha256);
    }

    /// <summary>Download (if needed) and build in one call.</summary>
    public static async Task DownloadAndBuildAsync(
        string jigdoPath,
        string templatePath,
        string partCacheDirectory,
        string outputImagePath,
        IProgress<JigdoDownloadProgress>? progress = null,
        HttpClient? httpClient = null,
        bool verifyImageSha256 = true,
        CancellationToken cancellationToken = default) =>
        await DownloadAndBuildAsync(
            JigdoFileReader.ReadFile(jigdoPath),
            templatePath,
            partCacheDirectory,
            outputImagePath,
            progress,
            httpClient,
            verifyImageSha256,
            cancellationToken).ConfigureAwait(false);

    /// <summary>Download and build using a parsed jigdo document (e.g. merged with extra servers).</summary>
    public static async Task DownloadAndBuildAsync(
        JigdoFileDocument jigdo,
        string templatePath,
        string partCacheDirectory,
        string outputImagePath,
        IProgress<JigdoDownloadProgress>? progress = null,
        HttpClient? httpClient = null,
        bool verifyImageSha256 = true,
        CancellationToken cancellationToken = default)
    {
        await DownloadPartsAsync(jigdo, templatePath, partCacheDirectory, progress, httpClient, cancellationToken)
            .ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        BuildImageFromCache(jigdo, templatePath, partCacheDirectory, outputImagePath, httpClient, verifyImageSha256);
    }
}
