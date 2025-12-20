using BE.League.Desktop.Console;
using Microsoft.Extensions.Hosting;
using RazorConsole.Core;

var token = CancellationToken.None;

IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
    .UseRazorConsole<LobbyView>();

IHost host = hostBuilder.Build();
await host.RunAsync();

