using PhpVersionManager.Models;

namespace PhpVersionManager.Services;

public class OnlineVersionDownloadsProvider : BaseVersionDownloadsProvider
{
    private const string WindowsDownloadPage = "https://windows.php.net/download";

    public override IAsyncEnumerable<VersionDownloadData> GetVersionsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}