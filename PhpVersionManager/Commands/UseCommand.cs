using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PhpVersionManager.Models;
using PhpVersionManager.ServiceInterfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PhpVersionManager.Commands;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal sealed class UseCommand(ILocalVersionsProvider localVersions, ILinkManager linkManager, IPvmEnvironment environment, IUserPathManager pathManager) : AsyncCommand<UseCommand.Settings>
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
        var currentVersionDestination = environment.CurrentVersionDestination;
        if (currentVersion is not null)
            await linkManager.UnlinkAsync(currentVersionDestination);

        await linkManager.LinkAsync(localData.Directory.FullName, currentVersionDestination);

        AnsiConsole.MarkupLine($"[green]Version {localData.Version} is now in use.[/]");

        if (await pathManager.Contains(currentVersionDestination))
            return 0;

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[yellow]Warning:[/] {currentVersionDestination} is not in your PATH.");

        if (!AnsiConsole.Confirm("Do you want to add it to your PATH?"))
            return 0;

        await pathManager.Add(currentVersionDestination);

        AnsiConsole.MarkupLine($"[green]{currentVersionDestination} added to PATH.[/]");

        return 0;
    }

    private Task<LocalVersionData?> GetLocalData(string version)
    {
        return version == "latest" ? localVersions.GetLatestVersionAsync() : localVersions.GetVersionAsync(version);
    }
}
