using System.ComponentModel;
using PhpVersionManager.Models;
using PhpVersionManager.ServiceInterfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PhpVersionManager.Commands;

internal sealed class UseCommand(ILocalVersionsProvider localVersions, ILinkManager linkManager, IPvmEnvironment environment) : AsyncCommand<UseCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("The version to uninstall.")]
        [CommandArgument(0, "<version>")]
        public string Version { get; set; } = null!;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var localData = await GetLocalData(settings.Version);
        if (localData is null)
        {
            AnsiConsole.MarkupLine($"[red]Version {settings.Version} is not installed.[/]");
            return 1;
        }

        var currentVersion = await localVersions.GetCurrentVersionAsync();
        if (currentVersion is not null)
            await linkManager.UnlinkAsync(environment.CurrentVersionDestination);

        await linkManager.LinkAsync(localData.Directory.FullName, environment.CurrentVersionDestination);

        AnsiConsole.MarkupLine($"[green]Version {localData.Version} is now in use.[/]");

        return 0;
    }

    private Task<LocalVersionData?> GetLocalData(string version)
    {
        return version == "latest" ? localVersions.GetLatestVersionAsync() : localVersions.GetVersionAsync(version);
    }
}
