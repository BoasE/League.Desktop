using System.Text;
using BE.League.Desktop;
using BE.League.Desktop.AutoAccept;
using BE.League.Desktop.Models;
using Spectre.Console;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    AnsiConsole.WriteLine("Ending...");
    e.Cancel = true;
    cts.Cancel();
};

Displays.WriteHeader();

await MonitorLoop.Run(cts.Token);


AnsiConsole.WriteLine("Ended");