using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PhpVersionManager.ServiceInterfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PhpVersionManager.Commands;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal sealed class UninstallCommand(ILocalVersionsProvider localVersionsProvider, ILinkManager linkManager, IPvmEnvironment environment, IUserPathManager pathManager) : AsyncCommand<UninstallCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("The version to uninstall.")]
        [CommandArgument(0, "<version>")]
        public string Version { get; set; } = null!;

        [Description("Force installation even if the version is currently in use.")]
        [CommandOption("-f|--force")]
        public bool Force { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var localData = await localVersionsProvider.GetVersionAsync(settings.Version);
        if (localData is null)
        {
            AnsiConsole.MarkupLine($"[red]Version {settings.Version} is not installed.[/]");
            return 1;
        }

        var currentVersion = await localVersionsProvider.GetCurrentVersionAsync();
        if (currentVersion is not null && currentVersion.Version == localData.Version)
        {
            if (!settings.Force)
            {
                AnsiConsole.MarkupLine($"[red]Version {localData.Version} is currently in use.[/]");
                AnsiConsole.MarkupLine("[red]Either switch to another version or use the --force option.[/]");
                return 1;
            }

            var currentVersionDestination = environment.CurrentVersionDestination;
            await linkManager.UnlinkAsync(currentVersionDestination);

            if (await pathManager.Contains(currentVersionDestination))
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[yellow]Warning:[/] {currentVersionDestination} is in your PATH.");

                if (AnsiConsole.Confirm("Do you want to remove it from your PATH?"))
                {
                    await pathManager.Remove(currentVersionDestination);

                    AnsiConsole.MarkupLine($"[green]{currentVersionDestination} removed from PATH.[/]");
                }
            }
        }

        Directory.Delete(localData.Directory.FullName, true);

        AnsiConsole.MarkupLine($"[green]Uninstalled version {localData.Version}.[/]");

        return 0;
    }
}