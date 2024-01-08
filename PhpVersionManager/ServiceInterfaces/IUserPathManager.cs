namespace PhpVersionManager.ServiceInterfaces;

public interface IUserPathManager
{
    Task<bool> Contains(string path, CancellationToken cancellationToken = default);

    Task Add(string path, CancellationToken cancellationToken = default);

    Task Remove(string path, CancellationToken cancellationToken = default);
}
