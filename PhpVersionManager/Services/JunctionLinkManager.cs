using System.Diagnostics;
using PhpVersionManager.ServiceInterfaces;

namespace PhpVersionManager.Services;

public class JunctionLinkManager : ILinkManager
{
    public async Task LinkAsync(string targetDirectory, string linkName)
    {
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "cmd",
            Arguments = $"/c mklink /J \"{linkName}\" \"{targetDirectory}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        });

        await process!.WaitForExitAsync();
    }

    public Task UnlinkAsync(string linkName)
    {
        Directory.Delete(linkName);

        return Task.CompletedTask;
    }
}