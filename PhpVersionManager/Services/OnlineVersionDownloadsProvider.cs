using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using PhpVersionManager.Models;
using PhpVersionManager.ServiceInterfaces;

namespace PhpVersionManager.Services;

public partial class OnlineVersionDownloadsProvider(IHtmlDownloader htmlDownloader) : BaseVersionDownloadsProvider
{
    private const string WindowsDownloadPageBase = "https://windows.php.net";
    private const string WindowsDownloadPage = WindowsDownloadPageBase + "/download";

    /// <summary>
    /// Should match links like https://windows.php.net/downloads/releases/php-8.3.1-nts-Win32-vs16-x64.zip
    /// </summary>
    [GeneratedRegex("""<li>\s*?<a href="(?<url>[^\r\n]+php-(?<major>[0-9]+)\.(?<minor>[0-9]+)\.(?<patch>[0-9]+)-nts-[^\r\n]+-x64.zip)">Zip<\/a>.*?<span class="md5sum">sha256: (?<sha256>[0-9a-f]+)<\/span>.*?<\/li>""", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex DownloadDataRegex();

    public override async IAsyncEnumerable<VersionDownloadData> GetVersionsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var htmlPage = await htmlDownloader.DownloadAsync(WindowsDownloadPage, cancellationToken);

        var matches = DownloadDataRegex().Matches(htmlPage).ToList();
        foreach (var match in matches)
        {
            match.Groups.TryGetValue("url", out var urlGroup);
            match.Groups.TryGetValue("sha256", out var sha256Group);
            match.Groups.TryGetValue("major", out var majorGroup);
            match.Groups.TryGetValue("minor", out var minorGroup);
            match.Groups.TryGetValue("patch", out var patchGroup);

            if (urlGroup is null || sha256Group is null || majorGroup is null || minorGroup is null || patchGroup is null)
                continue;

            var url = WindowsDownloadPageBase + urlGroup.Value;
            var sha256 = sha256Group.Value;
            var major = int.Parse(majorGroup.Value);
            var minor = int.Parse(minorGroup.Value);
            var patch = int.Parse(patchGroup.Value);

            yield return new(new(major, minor, patch), url, sha256);
        }
    }
}
