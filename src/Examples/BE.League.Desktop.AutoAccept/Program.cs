using System.Text;
using BE.League.Desktop;
using BE.League.Desktop.Models;
using Spectre.Console;

// Default Settings
var settings = new AcceptSettings(DelayMs: 2000, AutoExit: true);

// Show Header
ShowHeader();


// Create the reader
var reader = new LiveClientObjectReader();

// CancellationToken for Ctrl+C
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

// Start monitoring
await MonitorWithKeyboardControl(reader, settings, cts.Token);

AnsiConsole.MarkupLine("");
AnsiConsole.MarkupLine("[yellow]Beendet[/]");
AnsiConsole.MarkupLine("");
AnsiConsole.MarkupLine("[grey]Drücke eine Taste zum Beenden...[/]");
Console.ReadKey();

// ========== HELPER METHODS ==========

static async Task MonitorWithKeyboardControl(LiveClientObjectReader reader, AcceptSettings settings,
    CancellationToken cancellationToken)
{
    var checkCount = 0;
    var lastCheckTime = DateTime.Now;
    var acceptCount = 0;

    Lobby? lobbyDto = await reader.GetLobbyAsync();

    await AnsiConsole.Live(CreateStatusTable(null, lobbyDto, checkCount, (DateTime.Now - lastCheckTime).TotalSeconds))
        .StartAsync(async ctx =>
        {
            ReadyCheckDto? readyCheck = null;
            while (!cancellationToken.IsCancellationRequested)
            {
                // Check for keyboard input
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Escape)
                    {
                        return;
                    }
                }

                checkCount++;
                var elapsed = (DateTime.Now - lastCheckTime).TotalSeconds;

                lobbyDto = await reader.GetLobbyAsync();
                ctx.UpdateTarget(CreateStatusTable(readyCheck, lobbyDto, checkCount, elapsed));

                try
                {
                    readyCheck = await reader.GetReadyCheckAsync(cancellationToken);

                    if (readyCheck != null && readyCheck.State == "InProgress")
                    {
                        // Ready Check gefunden!
                        acceptCount++;

                        // Show notification
                        var notificationTable = new Table()
                            .Border(TableBorder.Double)
                            .BorderColor(Color.Green)
                            .AddColumn(new TableColumn("[green bold]STATUS[/]").Centered());

                        notificationTable.AddRow($"[green bold]🔔 SPIEL GEFUNDEN! (#{acceptCount})[/]");
                        notificationTable.AddRow($"[grey]{DateTime.Now:HH:mm:ss}[/]");

                        ctx.UpdateTarget(notificationTable);
                        await Task.Delay(500, cancellationToken);

                        // Delay if configured
                        if (settings.DelayMs > 0)
                        {
                            var delayTable = new Table()
                                .Border(TableBorder.Rounded)
                                .BorderColor(Color.Yellow)
                                .AddColumn(new TableColumn("[yellow]VERZÖGERUNG[/]").Centered());

                            delayTable.AddRow($"[yellow]⏱️  Warte {settings.DelayMs}ms...[/]");
                            ctx.UpdateTarget(delayTable);

                            await Task.Delay(settings.DelayMs, cancellationToken);
                        }

                        // Accept!
                        var acceptingTable = new Table()
                            .Border(TableBorder.Rounded)
                            .BorderColor(Color.Green)
                            .AddColumn(new TableColumn("[green]AKZEPTIERE...[/]").Centered());

                        acceptingTable.AddRow("[green]✓ Sende Accept-Befehl...[/]");
                        ctx.UpdateTarget(acceptingTable);

                        var accepted = await reader.AcceptReadyCheckAsync(cancellationToken);

                        if (accepted)
                        {
                            // Success!
                            var successTable = new Table()
                                .Border(TableBorder.Double)
                                .BorderColor(Color.Green)
                                .AddColumn(new TableColumn("[green bold]ERFOLG[/]").Centered());

                            successTable.AddRow("[green bold]✓ ERFOLGREICH AKZEPTIERT![/]");
                            successTable.AddRow($"[white]Accept #{acceptCount}[/]");
                            successTable.AddRow($"[grey]{DateTime.Now:HH:mm:ss}[/]");

                            ctx.UpdateTarget(successTable);
                            await Task.Delay(2000, cancellationToken);

                            // Check if we should exit after accept
                            if (settings.AutoExit)
                            {
                                var exitTable = new Table()
                                    .Border(TableBorder.Rounded)
                                    .BorderColor(Color.Cyan1)
                                    .AddColumn(new TableColumn("[cyan]AUTO-EXIT[/]").Centered());

                                exitTable.AddRow("[cyan]Beende in 3 Sekunden...[/]");
                                ctx.UpdateTarget(exitTable);

                                await Task.Delay(3000, cancellationToken);
                                return;
                            }

                            // Reset and continue
                            lastCheckTime = DateTime.Now;
                            checkCount = 0;
                        }
                        else
                        {
                            var errorTable = new Table()
                                .Border(TableBorder.Rounded)
                                .BorderColor(Color.Red)
                                .AddColumn(new TableColumn("[red]FEHLER[/]").Centered());

                            errorTable.AddRow("[red]✗ Konnte nicht akzeptieren[/]");
                            ctx.UpdateTarget(errorTable);
                            await Task.Delay(2000, cancellationToken);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception)
                {
                    // Ignore errors, continue checking
                }

                await Task.Delay(500, cancellationToken);
            }
        });
}

static void ShowHeader()
{
    AnsiConsole.Clear();
    AnsiConsole.Write(
        new FigletText("Auto Accept")
            .LeftJustified()
            .Color(Color.Green));

    AnsiConsole.Write(
        new Rule("[yellow]League of Legends - Ready Check Auto-Accept[/]")
            .RuleStyle("grey")
            .LeftJustified());

    AnsiConsole.MarkupLine("");
}

static Table CreateStatusTable(ReadyCheckDto? readyCheck, Lobby? lobbyDto, int checkCount, double elapsedSeconds)
{
    var statusTable = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.Yellow)
        .AddColumn(new TableColumn("[yellow bold]STATUS[/]").Centered());

    string lobby = string.Empty;
    string start = string.Empty;
    if (lobbyDto != null)
    {
        lobby = $"{lobbyDto.GameConfig?.GameMode} {lobbyDto.Members?.Length} Spieler";

        if (lobbyDto.CanStartActivity)
        {
            start = $"[green]  Kann starten[/]";
        }
        else
        {
            start = $"[red]  Kann nicht starten[/]";
        }
    }
    

    statusTable.AddRow($"[yellow]🔍 Prüfe auf Ready Check...[/]");
    statusTable.AddRow($"[grey]Lobby: {lobby}[/]");
    statusTable.AddRow(start);
    statusTable.AddRow($"[grey]{((int)elapsedSeconds)}s[/]");
    statusTable.AddRow($"[grey]{DateTime.Now:HH:mm:ss}[/]");

    return statusTable;
}

class AcceptSettings
{
    public int DelayMs { get; set; }
    public bool AutoExit { get; set; }

    public AcceptSettings(int DelayMs, bool AutoExit)
    {
        this.DelayMs = DelayMs;
        this.AutoExit = AutoExit;
    }
}