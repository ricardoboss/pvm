using PhpVersionManager.ServiceInterfaces;

namespace PhpVersionManager.Services;

public class EnvironmentUserPathManager : IUserPathManager
{
    private static string GetPath() => Environment.GetEnvironmentVariable("PATH")!;

    private static void SetPath(string path) =>
        Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.User);

    public Task<bool> Contains(string path, CancellationToken cancellationToken = default)
    {
        var currentPath = GetPath();
        return Task.FromResult(currentPath.Split(Path.PathSeparator).Contains(path));
    }

    public Task Add(string path, CancellationToken cancellationToken = default)
    {
        var currentPath = GetPath();
        var newPath = string.Join(Path.PathSeparator, currentPath.Split(Path.PathSeparator).Append(path));
        SetPath(newPath);
        return Task.CompletedTask;
    }

    public Task Remove(string path, CancellationToken cancellationToken = default)
    {
        var currentPath = GetPath();
        var newPath = string.Join(Path.PathSeparator, currentPath.Split(Path.PathSeparator).Where(p => p != path));
        SetPath(newPath);
        return Task.CompletedTask;
    }
}