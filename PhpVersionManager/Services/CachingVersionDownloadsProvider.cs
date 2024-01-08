using System.Runtime.CompilerServices;
using PhpVersionManager.Models;
using PhpVersionManager.ServiceInterfaces;

namespace PhpVersionManager.Services;

public class CachingVersionDownloadsProvider(IVersionDownloadsProvider inner) : BaseVersionDownloadsProvider, ICachingVersionDownloadsProvider
{
    private List<VersionDownloadData>? cache;

    public override async IAsyncEnumerable<VersionDownloadData> GetVersionsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cache ??= await inner.GetVersionsAsync(cancellationToken).ToListAsync(cancellationToken);

        foreach (var version in cache)
            yield return version;
    }

    public Task ClearCacheAsync(CancellationToken cancellationToken = default)
    {
        cache = null;

        return Task.CompletedTask;
    }
}
