using PhpVersionManager.Models;

namespace PhpVersionManager.ServiceInterfaces;

public interface IVersionDownloadsProvider
{
    public IAsyncEnumerable<VersionDownloadData> GetVersionsAsync(CancellationToken cancellationToken = default);

    public Task<VersionDownloadData?> GetVersionAsync(string version, CancellationToken cancellationToken = default);

    public Task<VersionDownloadData?> GetLatestVersionAsync(CancellationToken cancellationToken = default);
}
