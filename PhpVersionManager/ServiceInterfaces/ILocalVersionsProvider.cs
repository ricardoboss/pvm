using PhpVersionManager.Models;

namespace PhpVersionManager.ServiceInterfaces;

public interface ILocalVersionsProvider
{
    public IAsyncEnumerable<LocalVersionData> GetVersionsAsync(CancellationToken cancellationToken = default);
    public Task<LocalVersionData?> GetVersionAsync(string version, CancellationToken cancellationToken = default);
    public Task<LocalVersionData?> GetCurrentVersionAsync(CancellationToken cancellationToken = default);
    public Task<LocalVersionData?> GetLatestVersionAsync(CancellationToken cancellationToken = default);
}
