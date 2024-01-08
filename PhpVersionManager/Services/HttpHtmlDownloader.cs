using PhpVersionManager.ServiceInterfaces;

namespace PhpVersionManager.Services;

public class HttpHtmlDownloader(HttpClient client) : IHtmlDownloader
{
    public async Task<string> DownloadAsync(string url, CancellationToken cancellationToken = default)
    {
        using var response = await client.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}