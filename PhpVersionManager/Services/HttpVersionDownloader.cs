using PhpVersionManager.ServiceInterfaces;

namespace PhpVersionManager.Services;

public class HttpVersionDownloader(HttpClient client) : IVersionDownloader
{
    public async Task DownloadAsync(string destination, string downloadUrl, Action<long> reportMaxSize, Action<long> incrementProgress, CancellationToken cancellationToken = default)
    {
        var destinationParent = Path.GetDirectoryName(destination);
        Directory.CreateDirectory(destinationParent!);

        using var response = await client.GetAsync(downloadUrl, cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var sourceStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var destinationStream = File.Open(destination, FileMode.Create, FileAccess.Write, FileShare.None);

        var totalBytes = response.Content.Headers.ContentLength ?? -1L;

        reportMaxSize(totalBytes);

        var buffer = new byte[8192];
        long bytesRead;

        do
        {
            bytesRead = await sourceStream.ReadAsync(buffer, cancellationToken);
            await destinationStream.WriteAsync(buffer.AsMemory(0, (int)bytesRead), cancellationToken);

            incrementProgress(bytesRead);
        } while (bytesRead > 0);
    }
}