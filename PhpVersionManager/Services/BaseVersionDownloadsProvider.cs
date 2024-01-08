using PhpVersionManager.Models;
using PhpVersionManager.ServiceInterfaces;

namespace PhpVersionManager.Services;

public abstract class BaseVersionDownloadsProvider : IVersionDownloadsProvider
{
    public abstract IAsyncEnumerable<VersionDownloadData> GetVersionsAsync(
        CancellationToken cancellationToken = default);

    public async Task<VersionDownloadData?> GetVersionAsync(string version,
        CancellationToken cancellationToken = default)
        => await GetVersionsAsync(cancellationToken)
            .FirstOrDefaultAsync(v => v.Version.ToString() == version, cancellationToken);

    public async Task<VersionDownloadData?> GetLatestVersionAsync(CancellationToken cancellationToken = default)
    {
        return await GetVersionsAsync(cancellationToken)
            .OrderByDescending(v => v.Version.Major)
            .ThenByDescending(v => v.Version.Minor)
            .ThenByDescending(v => v.Version.Patch)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
