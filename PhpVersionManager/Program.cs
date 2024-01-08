using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhpVersionManager.Commands;
using PhpVersionManager.ServiceInterfaces;
using PhpVersionManager.Services;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;

const string appName = "pvm";
var appVersion = GitVersionInformation.ShortSha!;

// ensure UTF-8 is used for input and output (enables animated spinners)
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var services = new ServiceCollection();
ConfigureServices(services);
using var registrar = new DependencyInjectionRegistrar(services);

var app = new CommandApp(registrar);
app.Configure(ConfigureCommands);
return await app.RunAsync(args);

void ConfigureServices(IServiceCollection s)
{
    var config = new ConfigurationBuilder()
        .AddEnvironmentVariables("PVM_")
        .Build();

    s.AddSingleton<IConfiguration>(config);

    s.AddSingleton<IPvmEnvironment, ConfiguredPvmEnvironment>();

    s.AddSingleton<IVersionDownloadsProvider, StaticVersionDownloadsProvider>();
    // s.AddSingleton<IVersionDownloadsProvider, OnlineVersionDownloadsProvider>();
    s.AddSingleton<ICachingVersionDownloadsProvider, CachingVersionDownloadsProvider>();

    s.AddHttpClient<IVersionDownloader, HttpVersionDownloader>().ConfigureHttpClient(c =>
    {
        c.DefaultRequestHeaders.Accept.Add(new("application/zip", 1.0));
        c.DefaultRequestHeaders.Accept.Add(new("application/octet-stream", 0.9));
        c.DefaultRequestHeaders.Accept.Add(new("*/*", 0.8));

        c.DefaultRequestHeaders.UserAgent.Add(new(appName, appVersion));
        c.DefaultRequestHeaders.UserAgent.Add(new("(github.com/ricardoboss/pvm)"));
    });

    s.AddSingleton<IZipExtractor, ZipExtractor>();
}

void ConfigureCommands(IConfigurator c)
{
    c.SetApplicationName(appName);
    c.SetApplicationVersion(appVersion);

    c.AddCommand<SearchCommand>("search");
    c.AddCommand<InstallCommand>("install");
}
