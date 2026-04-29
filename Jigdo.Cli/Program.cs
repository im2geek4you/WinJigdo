using Jigdo;

if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
{
    PrintHelp();
    Environment.Exit(0);
}

CliOptions opt = CliOptions.Parse(args);
if (opt.Error != null)
{
    Console.Error.WriteLine(opt.Error);
    PrintHelp();
    Environment.Exit(2);
}

string jigdoPath = Path.GetFullPath(opt.JigdoPath!);
string templatePath = Path.GetFullPath(opt.TemplatePath!);
string outputPath = Path.GetFullPath(opt.OutputPath!);
string cacheDir = string.IsNullOrEmpty(opt.CacheDirectory)
    ? Path.Combine(Path.GetDirectoryName(outputPath)!, ".jigdo-parts-cache")
    : Path.GetFullPath(opt.CacheDirectory);

if (!File.Exists(jigdoPath))
{
    Console.Error.WriteLine($"Jigdo file not found: {jigdoPath}");
    Environment.Exit(3);
}

if (!File.Exists(templatePath))
{
    Console.Error.WriteLine($"Template file not found: {templatePath}");
    Environment.Exit(3);
}

string outDir = Path.GetDirectoryName(outputPath)!;
if (string.IsNullOrEmpty(outDir))
    outDir = Directory.GetCurrentDirectory();
Directory.CreateDirectory(outDir);
Directory.CreateDirectory(cacheDir);

using var http = new HttpClient { Timeout = TimeSpan.FromHours(4) };
var progress = new Progress<JigdoDownloadProgress>(p =>
{
    double pct = p.TotalBytes == 0 ? 0 : 100.0 * p.BytesReceived / p.TotalBytes;
    string name = p.RelativePath.Length <= 72 ? p.RelativePath : p.RelativePath[..72];
    Console.Write($"\r  {pct,5:F1}%  {name,-72}");
});

Console.WriteLine($"Jigdo:    {jigdoPath}");
Console.WriteLine($"Template: {templatePath}");
Console.WriteLine($"Output:   {outputPath}");
Console.WriteLine($"Cache:    {cacheDir}");
Console.WriteLine();

try
{
    JigdoWebPipeline.DownloadAndBuildAsync(
            jigdoPath,
            templatePath,
            cacheDir,
            outputPath,
            progress,
            http,
            verifyImageSha256: !opt.NoVerify,
            cancellationToken: CancellationToken.None)
        .GetAwaiter()
        .GetResult();
}
catch (Exception ex)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine(ex.Message);
    if (ex.InnerException != null)
        Console.Error.WriteLine(ex.InnerException.Message);
    Environment.Exit(4);
}

Console.WriteLine();
Console.WriteLine("Done.");

static void PrintHelp()
{
    Console.WriteLine(
        """
        jigdo-build — assemble an ISO from a .jigdo file and .template (downloads parts from the web).

        Usage:
          jigdo-build --jigdo <file.jigdo> --template <file.template> --output <image.iso> [options]

        Options:
          -j, --jigdo       Path to the gzip .jigdo file
          -t, --template    Path to the .template file
          -o, --output      Path for the generated ISO (or raw image)
          -c, --cache       Directory to cache downloaded parts (default: .jigdo-parts-cache next to output)
          --no-verify       Do not verify final image SHA256 against the template
          -h, --help        Show this help

        Example:
          jigdo-build -j debian.jigdo -t debian.template -o debian.iso
        """);
}

internal sealed class CliOptions
{
    public string? JigdoPath { get; init; }
    public string? TemplatePath { get; init; }
    public string? OutputPath { get; init; }
    public string? CacheDirectory { get; init; }
    public bool NoVerify { get; init; }
    public string? Error { get; init; }

    public static CliOptions Parse(string[] args)
    {
        string? jigdo = null;
        string? template = null;
        string? output = null;
        string? cache = null;
        bool noVerify = false;

        for (int i = 0; i < args.Length; i++)
        {
            string a = args[i];
            if (a is "-h" or "--help")
                continue;

            if (a is "-j" or "--jigdo")
            {
                if (!TryTake(ref i, args, out jigdo))
                    return Err("Missing value for --jigdo");
                continue;
            }

            if (a is "-t" or "--template")
            {
                if (!TryTake(ref i, args, out template))
                    return Err("Missing value for --template");
                continue;
            }

            if (a is "-o" or "--output")
            {
                if (!TryTake(ref i, args, out output))
                    return Err("Missing value for --output");
                continue;
            }

            if (a is "-c" or "--cache")
            {
                if (!TryTake(ref i, args, out cache))
                    return Err("Missing value for --cache");
                continue;
            }

            if (a == "--no-verify")
            {
                noVerify = true;
                continue;
            }

            return Err($"Unknown argument: {a}");
        }

        if (jigdo == null || template == null || output == null)
            return new CliOptions { Error = "Required: --jigdo, --template, and --output." };

        return new CliOptions
        {
            JigdoPath = jigdo,
            TemplatePath = template,
            OutputPath = output,
            CacheDirectory = cache,
            NoVerify = noVerify,
        };

        static bool TryTake(ref int i, string[] arr, out string? value)
        {
            value = null;
            if (i + 1 >= arr.Length)
                return false;
            value = arr[++i];
            return !string.IsNullOrWhiteSpace(value);
        }

        static CliOptions Err(string message) => new() { Error = message };
    }
}
