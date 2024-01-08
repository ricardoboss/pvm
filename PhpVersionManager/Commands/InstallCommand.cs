using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PhpVersionManager.Models;
using PhpVersionManager.ServiceInterfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PhpVersionManager.Commands;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal sealed class InstallCommand(ICachingVersionDownloadsProvider downloadsProvider, IPvmEnvironment environment, IVersionDownloader versionDownloader, IZipExtractor zipExtractor) : AsyncCommand<InstallCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("The version to install.")]
        [CommandArgument(0, "<version>")]
        public string Version { get; init; } = null!;

        [Description("Don't use the cache.")]
        [CommandOption("-n|--no-cache")]
        public bool NoCache { get; init; }

        [Description("Force installation even if the version is already installed.")]
        [CommandOption("-f|--force")]
        public bool Force { get; init; }
    }
    
    private VersionDownloadData? downloadData;

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (!settings.NoCache)
            await downloadsProvider.ClearCacheAsync();

        await LoadDownloadData(settings.Version);
        if (downloadData is null)
        {
            AnsiConsole.MarkupLine($"[red]Version {settings.Version} not found.[/]");

            return 1;
        }

        var versionsDirectory = Path.Combine(environment.VersionsInstallDirectory, downloadData.Version.ToString());
        if (Directory.Exists(versionsDirectory) && !settings.Force)
        {
            AnsiConsole.MarkupLine($"[red]Version {downloadData.Version} is already installed.[/]");

            AnsiConsole.MarkupLine("[yellow]To switch to this version, run:[/]");
            AnsiConsole.MarkupLine($"[yellow]pvm use {downloadData.Version}[/]");

            return 1;
        }

        AnsiConsole.MarkupLine($"[green]Installing version {downloadData.Version}...[/]");
        await AnsiConsole
            .Progress()
            .StartAsync(async ctx =>
            {
                var downloadTask = ctx.AddTask("[green]Downloading zip archive[/]");

                var zipPath = await DownloadAsync(downloadTask, downloadData, settings.Force);

                var extractTask = ctx.AddTask("[green]Extracting zip archive[/]");

                await ExtractAsync(extractTask, zipPath, settings.Force);

                var cleanupTask = ctx.AddTask("[green]Cleaning up[/]").MaxValue(1).IsIndeterminate();

                File.Delete(zipPath);

                cleanupTask.Increment(1);
                cleanupTask.StopTask();
            });

        AnsiConsole.MarkupLine($"[green]Version {downloadData.Version} installed![/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[yellow]To switch to this version, run:[/]");
        AnsiConsole.MarkupLine($"[yellow]pvm use {downloadData.Version}[/]");

        return 0;
    }

    private async Task<string> DownloadAsync(ProgressTask task, VersionDownloadData data, bool force)
    {
        var zipPath = Path.Combine(environment.VersionsDownloadDirectory, $"{data.Version}.zip");
        if (File.Exists(zipPath))
        {
            if (!force)
            {
                task.Description("[green]Zip archive already downloaded[/]")
                    .StopTask();

                return zipPath;
            }

            File.Delete(zipPath);
        }
        
        var downloadUrl = data.ZipUrl;

        await versionDownloader.DownloadAsync(
            zipPath,
            downloadUrl,
            max => task.MaxValue(max),
            increment => task.Increment(increment)
        );

        task.StopTask();

        return zipPath;
    }

    private async Task ExtractAsync(ProgressTask task, string zipPath, bool force)
    {
        var versionDirectory = Path.Combine(environment.VersionsInstallDirectory, downloadData!.Version.ToString());
        if (Directory.Exists(versionDirectory))
        {
            if (!force)
            {
                task.Description("[green]Version already installed[/]")
                    .StopTask();

                return;
            }

            Directory.Delete(versionDirectory, true);
        }

        await zipExtractor.ExtractAsync(
            zipPath,
            versionDirectory,
            max => task.MaxValue(max),
            increment => task.Increment(increment)
        );

        task.StopTask();
    }

    private async Task LoadDownloadData(string version)
    {
        await AnsiConsole
            .Status()
            .Spinner(Spinner.Known.Dots10!)
            .StartAsync($"Looking up {version}...", async _ =>
            {
                if (version == "latest")
                    downloadData = await downloadsProvider.GetLatestVersionAsync();
                else
                    downloadData = await downloadsProvider.GetVersionAsync(version);
            });
    }
}