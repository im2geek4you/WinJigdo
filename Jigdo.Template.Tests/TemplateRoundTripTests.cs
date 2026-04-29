using Jigdo;
using Xunit;

namespace Jigdo.Template.Tests;

public class TemplateRoundTripTests
{
    private static string TemplatePath =>
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "debian-13.4.0-amd64-DVD-2.template");

    [Fact]
    public void ReadDebianTemplate_HasExpectedImageLength()
    {
        string path = Path.GetFullPath(TemplatePath);
        if (!File.Exists(path))
            return; // optional in CI without large fixtures

        var doc = JigdoTemplateReader.ReadFile(path);
        Assert.NotNull(doc.ImageSha256);
        Assert.Equal(4537075712UL, doc.ImageSha256!.ImageLength);
    }

    [Fact]
    public void DescSection_RoundTripsBinary()
    {
        string path = Path.GetFullPath(TemplatePath);
        if (!File.Exists(path))
            return;

        var doc = JigdoTemplateReader.ReadFile(path);
        byte[] serialized = TemplateDescriptorSerializer.SerializeDescSection(doc.Entries);
        Assert.Equal(doc.DescTailBytes.ToArray(), serialized);
    }

    [Fact]
    public void FirstCompressedChunk_DecompressesToExpectedSize()
    {
        string path = Path.GetFullPath(TemplatePath);
        if (!File.Exists(path))
            return;

        using var fs = File.OpenRead(path);
        var reader = new TemplateCompressedDataReader(fs);
        Assert.True(reader.TryReadNext(out byte[]? chunk, out var kind));
        Assert.NotNull(chunk);
        Assert.Equal(JigdoCompressionKind.GZip, kind);
        Assert.Equal(1048576, chunk!.Length);
    }
}
