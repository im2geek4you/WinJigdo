using Jigdo;
using Xunit;

namespace Jigdo.Template.Tests;

public class JigdoFileReaderTests
{
    private static string JigdoPath =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "debian-13.4.0-amd64-DVD-2.jigdo"));

    [Fact]
    public void ReadDebianJigdo_HasServersAndImage()
    {
        if (!File.Exists(JigdoPath))
            return;

        var doc = JigdoFileReader.ReadFile(JigdoPath);
        Assert.Equal("2.0", doc.Version);
        Assert.Contains("debian-13.4.0-amd64-DVD-2.iso", doc.Image.IsoFilename, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(doc.Image.TemplateSha256SumBase64);
        Assert.True(doc.Parts.Count > 1000);
        Assert.True(doc.ServerLines.Count >= 1);
        Assert.Contains(doc.ServerLines, s => s.MirrorLabel == "Debian" && s.BaseUrl.Host.Length > 0);
    }

    [Fact]
    public void CombinePartUrl_DebianExample()
    {
        var baseUri = new Uri("http://us.cdimage.debian.org/cdimage/snapshot/Debian/");
        var u = JigdoMirrorUrls.CombinePartUrl(baseUri, "pool/main/a/apt/apt_2.9.8_amd64.deb");
        Assert.Equal(
            "http://us.cdimage.debian.org/cdimage/snapshot/Debian/pool/main/a/apt/apt_2.9.8_amd64.deb",
            u.AbsoluteUri);
    }
}
