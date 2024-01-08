namespace PhpVersionManager.ServiceInterfaces;

public interface IPvmEnvironment
{
    string VersionsInstallDirectory { get; }

    string VersionsDownloadDirectory { get; }

    string CurrentVersionDestination { get; }
}
