namespace PhpVersionManager.ServiceInterfaces;

public interface ICachingVersionDownloadsProvider : IVersionDownloadsProvider
{
    Task ClearCacheAsync(CancellationToken cancellationToken = default);
}