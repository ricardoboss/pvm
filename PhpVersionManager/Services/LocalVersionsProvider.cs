using System.Runtime.CompilerServices;
using PhpVersionManager.Models;
using PhpVersionManager.ServiceInterfaces;

namespace PhpVersionManager.Services;

public class LocalVersionsProvider(IPvmEnvironment environment) : ILocalVersionsProvider
{
    private DirectoryInfo GetVersionsDirectory() => new(environment.VersionsInstallDirectory);

    private DirectoryInfo GetCurrentVersionDirectory() => new(environment.CurrentVersionDestination);

    public async IAsyncEnumerable<LocalVersionData> GetVersionsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var versionsDirectory = GetVersionsDirectory();
        if (!versionsDirectory.Exists)
            yield break;

        foreach (var versionDirectory in versionsDirectory.EnumerateDirectories())
            yield return new(PhpVersion.FromDirectoryName(versionDirectory.Name), versionDirectory);
    }

    public async Task<LocalVersionData?> GetVersionAsync(string version, CancellationToken cancellationToken = default)
    {
        var versionsDirectory = GetVersionsDirectory();
        if (!versionsDirectory.Exists)
            return null;

        var versionDirectory = versionsDirectory.EnumerateDirectories(version).FirstOrDefault();
        if (versionDirectory is null)
            return null;

        return new(PhpVersion.FromDirectoryName(versionDirectory.Name), versionDirectory);
    }

    public async Task<LocalVersionData?> GetCurrentVersionAsync(CancellationToken cancellationToken = default)
    {
        var currentVersionDirectory = GetCurrentVersionDirectory();
        if (!currentVersionDirectory.Exists)
            return null;

        if (currentVersionDirectory.LinkTarget is not { } target)
            return null;

        var currentTargetDirectory = new DirectoryInfo(target);
        if (!currentTargetDirectory.Exists)
            return null;

        return new(PhpVersion.FromDirectoryName(currentTargetDirectory.Name), currentTargetDirectory);
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
