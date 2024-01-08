using Microsoft.Extensions.Configuration;
using PhpVersionManager.ServiceInterfaces;

namespace PhpVersionManager.Services;

public class ConfiguredPvmEnvironment(IConfiguration config) : IPvmEnvironment
{
    public string VersionsInstallDirectory
    {
        get
        {
            var installDir = config["INSTALL_DIR"];

            return installDir ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".pvm", "versions");
        }
    }

    public string VersionsDownloadDirectory
    {
        get
        {
            var downloadDir = config["DOWNLOAD_DIR"];

            return downloadDir ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".pvm", "downloads");
        }
    }

    public string CurrentVersionDestination
    {
        get
        {
            var currentDir = config["CURRENT_DIR"];

            return currentDir ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "PHP");
        }
    }
}