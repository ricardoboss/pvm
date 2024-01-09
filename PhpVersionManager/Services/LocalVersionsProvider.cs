using System.Runtime.CompilerServices;
using PhpVersionManager.Models;
using PhpVersionManager.ServiceInterfaces;

namespace PhpVersionManager.Services;

public class LocalVersionsProvider(IPvmEnvironment environment) : ILocalVersionsProvider
{
    private DirectoryInfo GetVersionsDirectory() => new(environment.VersionsInstallDirectory);

    private DirectoryInfo GetCurrentVersionDirectory() => new(environment.CurrentVersionDestination);

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async IAsyncEnumerable<LocalVersionData> GetVersionsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var versionsDirectory = GetVersionsDirectory();
        if (!versionsDirectory.Exists)
            yield break;

        foreach (var versionDirectory in versionsDirectory.EnumerateDirectories())
            yield return new(PhpVersion.FromDirectoryName(versionDirectory.Name), versionDirectory);
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

    public Task<LocalVersionData?> GetVersionAsync(string version, CancellationToken cancellationToken = default)
    {
        var versionsDirectory = GetVersionsDirectory();
        if (!versionsDirectory.Exists)
            return Task.FromResult<LocalVersionData?>(null);

        var versionDirectory = versionsDirectory.EnumerateDirectories(version).FirstOrDefault();
        if (versionDirectory is null)
            return Task.FromResult<LocalVersionData?>(null);

        return Task.FromResult<LocalVersionData?>(new(PhpVersion.FromDirectoryName(versionDirectory.Name), versionDirectory));
    }

    public Task<LocalVersionData?> GetCurrentVersionAsync(CancellationToken cancellationToken = default)
    {
        var currentVersionDirectory = GetCurrentVersionDirectory();
        if (!currentVersionDirectory.Exists)
            return Task.FromResult<LocalVersionData?>(null);

        if (currentVersionDirectory.LinkTarget is not { } target)
            return Task.FromResult<LocalVersionData?>(null);

        var currentTargetDirectory = new DirectoryInfo(target);
        if (!currentTargetDirectory.Exists)
            return Task.FromResult<LocalVersionData?>(null);

        return Task.FromResult<LocalVersionData?>(new(PhpVersion.FromDirectoryName(currentTargetDirectory.Name), currentTargetDirectory));
    }

    public async Task<LocalVersionData?> GetLatestVersionAsync(CancellationToken cancellationToken = default)
    {
        var versions = await GetVersionsAsync(cancellationToken).ToListAsync(cancellationToken);
        if (versions.Count == 0)
            return null;

        return versions
            .OrderByDescending(v => v.Version.Major)
            .ThenByDescending(v => v.Version.Minor)
            .ThenByDescending(v => v.Version.Patch)
            .FirstOrDefault();
    }
}
