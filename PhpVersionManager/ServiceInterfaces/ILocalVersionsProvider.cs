using PhpVersionManager.Models;

namespace PhpVersionManager.ServiceInterfaces;

public interface ILocalVersionsProvider
{
    public IAsyncEnumerable<KeyValuePair<PhpVersion, DirectoryInfo>> GetVersionsAsync(CancellationToken cancellationToken = default);
}