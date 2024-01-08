namespace PhpVersionManager.ServiceInterfaces;

public interface IHtmlDownloader
{
    Task<string> DownloadAsync(string url, CancellationToken cancellationToken = default);
}