using BE.League.Desktop.Models;
using Spectre.Console;

namespace BE.League.Desktop.AutoAccept;

public static class Displays
{
    public static void WriteHeader()
    {
        AnsiConsole.Write(
            new FigletText("Auto Accept")
                .Centered()
                .Color(Color.Green));

        AnsiConsole.Write(
            new Rule("[yellow]Simple Ready Check and autoaccepting new games[/]")
                .RuleStyle("grey")
                .Centered());

        AnsiConsole.MarkupLine("");
    }


    public static async Task WriteError(CancellationToken cancellationToken, LiveDisplayContext ctx)
    {
        var errorTable = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Red)
            .AddColumn(new TableColumn("[red]FEHLER[/]").Centered());

        errorTable.AddRow("[red]✗ Konnte nicht akzeptieren[/]");
        ctx.UpdateTarget(errorTable);
        await Task.Delay(2000, cancellationToken);
    }

    public static void WriteAcceptingNotification(LiveDisplayContext ctx)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Green)
            .AddColumn(new TableColumn("[green]AKZEPTIERE...[/]").Centered());

        table.AddRow("[green]✓ Sende Accept-Befehl...[/]");

        ctx.UpdateTarget(table);
    }

    public static void WriteSuccessNotification(LiveDisplayContext ctx)
    {
        var table = new Table()
            .Border(TableBorder.Double)
            .BorderColor(Color.Green)
            .AddColumn(new TableColumn("[green bold]ERFOLG[/]").Centered());

        table.AddRow("[green bold]✓ ERFOLGREICH AKZEPTIERT![/]");
        table.AddRow($"[grey]{DateTime.Now:HH:mm:ss}[/]");

        ctx.UpdateTarget(table);
    }
    public static Table NotificationTable()
    {
  
        // Show notification
        var notificationTable = new Table()
            .Border(TableBorder.Double)
            .BorderColor(Color.Green)
            .AddColumn(new TableColumn("[green bold]STATUS[/]").Centered());
        return notificationTable;
    }

    public static Table WriteGameFound(int acceptCount,LiveDisplayContext ctx)
    {
        var table = new Table()
            .Border(TableBorder.Double)
            .BorderColor(Color.Green)
            .AddColumn(new TableColumn("[green bold]STATUS[/]").Centered());

        table.AddRow($"[green bold]🔔 SPIEL GEFUNDEN! (#{acceptCount})[/]");
        table.AddRow($"[grey]{DateTime.Now:HH:mm:ss}[/]");
        table.AddRow($"[grey]Akzeptiere das Spiel in 2 Sekunden...[/]");
        ctx.UpdateTarget(table);

        return table;
    }

    public static Table CreateStatusTable(Lobby? lobbyDto, DateTimeOffset started, int acceptedCount)
    {
        var statusTable = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Yellow)
            .Centered()
            .AddColumn(new TableColumn("[yellow bold]STATUS[/]").Centered());

        string lobby = string.Empty;

        if (lobbyDto != null)
        {
            lobby = $"{lobbyDto.GameConfig?.GameMode} {lobbyDto.Members?.Length} Spieler";
            statusTable.AddRow($"[grey]Lobby: {lobby}[/]");
            if (lobbyDto.CanStartActivity)
            {
                statusTable.AddRow($"[grey]  Warte auf Lobby start...[/]");
            }
            else
            {
                statusTable.AddRow($"[yellow] Prüfe auf Ready Check...[/]");
            }
        }
        else
        {
            statusTable.AddRow($"[grey] Keine Lobby gefunden...[/]");
            
        }


        return statusTable;
    }
}