using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PhpVersionManager.ServiceInterfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PhpVersionManager.Commands;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal sealed class SearchCommand(ICachingVersionDownloadsProvider downloadsProvider) : AsyncCommand<SearchCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("The version to search for.")]
        [CommandArgument(0, "<version>")]
        public string Version { get; init; } = null!;

        [Description("Don't use the cache.")]
        [CommandOption("-n|--no-cache")]
        public bool NoCache { get; init; }
    }

    private bool anyFound;

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (!settings.NoCache)
            await downloadsProvider.ClearCacheAsync();

        var query = settings.Version;

        await AnsiConsole
            .Status()
            .Spinner(Spinner.Known.Dots!)
            .StartAsync("Searching...", _ => SearchAsync(query));

        AnsiConsole.WriteLine();

        if (anyFound)
            AnsiConsole.MarkupLine("[grey]Use [bold]pvm install <version>[/] to install a version.[/]");
        else
            AnsiConsole.MarkupLine($"[red]No results found for '{query}'[/]");

        return 0;
    }

    private async Task SearchAsync(string query)
    {
        var data = downloadsProvider.GetVersionsAsync();

        AnsiConsole.MarkupLine("[bold]Results:[/]");

        await foreach (var datum in data)
        {
            if (!datum.Version.ToString().Contains(query, StringComparison.OrdinalIgnoreCase))
                continue;

            AnsiConsole.MarkupLine($"- [green]{datum.Version}[/]");

            anyFound = true;
        }
    }
}