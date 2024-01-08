namespace PhpVersionManager.ServiceInterfaces;

public interface IZipExtractor
{
    Task ExtractAsync(string zipPath, string destination, Action<long> reportMaxSize, Action<long> incrementProgress, CancellationToken cancellationToken = default);
}