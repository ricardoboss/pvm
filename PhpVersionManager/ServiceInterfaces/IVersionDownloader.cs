namespace PhpVersionManager.ServiceInterfaces;

public interface IVersionDownloader
{
    public Task DownloadAsync(string destination, string downloadUrl, Action<long> reportMaxSize,
        Action<long> incrementProgress, CancellationToken cancellationToken = default);
}