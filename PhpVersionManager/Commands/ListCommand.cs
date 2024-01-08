using PhpVersionManager.ServiceInterfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace PhpVersionManager.Commands;

internal sealed class ListCommand(ILocalVersionsProvider versionsProvider) : AsyncCommand<ListCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var versions = await versionsProvider.GetVersionsAsync().ToListAsync();
        var currentVersion = await versionsProvider.GetCurrentVersionAsync();

        AnsiConsole.MarkupLine("[bold]Installed versions:[/]");
        AnsiConsole.WriteLine();

        var table = new Table();
        table.Border(new NoTableBorder());
        table.HideHeaders();
        table.AddColumn("");
        table.AddColumn("");

        foreach (var version in versions)
        {
            table.AddRow(version.Version.ToString(), version.Directory.FullName == currentVersion?.Directory.FullName ? "[green]*[/]" : "");
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine("[green]*[/] Current version");

        return 0;
    }
}