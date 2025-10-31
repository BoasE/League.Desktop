using BE.League.Desktop;
using BE.League.Desktop.Models;
using Spectre.Console;

// Header
AnsiConsole.Clear();
AnsiConsole.Write(
    new FigletText("League Monitor")
        .Centered()
        .Color(Color.Cyan1));

AnsiConsole.Write(
    new Rule("[yellow]Live Monitor für Lobby & Game[/]")
        .RuleStyle("grey")
        .Centered());

AnsiConsole.MarkupLine("");

// Create the Live Client Reader
var reader = new LiveClientObjectReader();

AnsiConsole.Status()
    .Start("Initialisiere League Client Verbindung...", ctx =>
    {
        ctx.Spinner(Spinner.Known.Dots);
        ctx.SpinnerStyle(Style.Parse("green"));
        Thread.Sleep(1000);
    });

AnsiConsole.MarkupLine("[green]✓[/] Bereit!");
AnsiConsole.MarkupLine("");
AnsiConsole.MarkupLine("[grey]Funktionen:[/]");
AnsiConsole.MarkupLine("  • [cyan]Lobby-Überwachung[/] mit Team-Anzeige");
AnsiConsole.MarkupLine("  • [cyan]Champion-Select[/] mit Live-Picks");
AnsiConsole.MarkupLine("  • [cyan]Automatisches Ready-Check-Accept[/]");
AnsiConsole.MarkupLine("  • [cyan]Live Game Data[/]");
AnsiConsole.MarkupLine("");
AnsiConsole.MarkupLine("[grey]Drücke Strg+C zum Beenden[/]");
AnsiConsole.MarkupLine("");

// Main monitoring loop with Live Display
await AnsiConsole.Live(CreateWaitingTable())
    .StartAsync(async ctx =>
    {
        while (true)
        {
            try
            {
                // Try to get game data first
                var allGameData = await reader.GetAllGameDataAsync();

                if (allGameData != null)
                {
                    // ========== IM SPIEL ==========
                    var layout = CreateGameLayout(allGameData);
                    ctx.UpdateTarget(layout);
                    await Task.Delay(5000);
                }
                else
                {
                    // ========== NICHT IM SPIEL - PRÜFE LOBBY ==========
                    var lobby = await reader.GetLobbyAsync();

                    if (lobby != null)
                    {
                        // ========== IN LOBBY ==========
                        var champSelect = await reader.GetChampSelectSessionAsync();
                        var readyCheck = await reader.GetReadyCheckAsync();

                        // Handle auto-accept
                        if (readyCheck != null && readyCheck.State == "InProgress")
                        {
                            var accepted = await reader.AcceptReadyCheckAsync();
                            if (accepted)
                            {
                                AnsiConsole.MarkupLine("[green]✓ Ready Check akzeptiert![/]");
                            }
                        }

                        var layout = CreateLobbyLayout(lobby, champSelect, readyCheck);
                        ctx.UpdateTarget(layout);
                        await Task.Delay(2000);
                    }
                    else
                    {
                        // ========== WEDER LOBBY NOCH SPIEL ==========
                        var waitTable = CreateWaitingTable();
                        ctx.UpdateTarget(waitTable);
                        await Task.Delay(2000);
                    }
                }
            }
            catch (Exception ex)
            {
                var errorPanel = new Panel($"[red]Fehler:[/] {ex.Message}")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.Red);
                ctx.UpdateTarget(errorPanel);
                await Task.Delay(2000);
            }
        }
    });

// ========== HELPER METHODS ==========

static Table CreateWaitingTable()
{
    var table = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.Grey);

    table.AddColumn(new TableColumn("[yellow]Status[/]").Centered());
    table.AddRow($"[grey]⏳ Warte auf Lobby oder Spiel... {DateTime.Now:HH:mm:ss}[/]");

    return table;
}

static Layout CreateLobbyLayout(Lobby lobby, ChampSelectSession? champSelect, ReadyCheckDto? readyCheck)
{
    var layout = new Layout("Root")
        .SplitRows(
            new Layout("Header"),
            new Layout("Content"));

    // Header
    var headerPanel = new Panel(
            new Markup("[cyan bold]🏠 LOBBY[/]\n[grey]League of Legends[/]"))
        .Border(BoxBorder.Double)
        .BorderColor(Color.Cyan1);
    layout["Header"].Update(headerPanel);

    // Content
    if (champSelect != null)
    {
        layout["Content"].SplitColumns(
            new Layout("Lobby"),
            new Layout("ChampSelect"));

        layout["Lobby"].Update(CreateLobbyTable(lobby));
        layout["ChampSelect"].Update(CreateChampSelectTable(champSelect));
    }
    else
    {
        layout["Content"].Update(CreateLobbyTable(lobby));
    }

    return layout;
}

static Table CreateLobbyTable(Lobby lobby)
{
    var table = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.Blue)
        .Title("[blue bold]Lobby Informationen[/]");

    table.AddColumn("[yellow]Info[/]");
    table.AddColumn("[cyan]Wert[/]");

    table.AddRow("Mitglieder", $"[white]{lobby.Members.Length}[/]");

    if (lobby.GameConfig?.QueueId.HasValue == true)
    {
        var queueName = GetQueueName(lobby.GameConfig.QueueId.Value);
        table.AddRow("Queue", $"[white]{queueName}[/]");
    }

    table.AddEmptyRow();

    var membersTable = new Table()
        .Border(TableBorder.None)
        .HideHeaders();
    membersTable.AddColumn("Name");

    foreach (var member in lobby.Members)
    {
        membersTable.AddRow($"[cyan]•[/] {member.SummonerName}");
    }

    table.AddRow(new Markup("[yellow bold]Team:[/]"), membersTable);

    return table;
}

static Table CreateChampSelectTable(ChampSelectSession session)
{
    var table = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.Green)
        .Title("[green bold]Champion Select[/]");

    table.AddColumn("[yellow]Cell[/]");
    table.AddColumn("[cyan]Champion[/]");
    table.AddColumn("[grey]Status[/]");

    var phaseText = session.Timer?.Phase ?? "Unknown";
    table.Caption($"[grey]Phase: {phaseText} | Deine Cell: {session.LocalPlayerCellId}[/]");

    foreach (var member in session.MyTeam.OrderBy(m => m.CellId))
    {
        var cellText = member.CellId == session.LocalPlayerCellId
            ? $"[green bold]👤 {member.CellId}[/]"
            : $"[grey]{member.CellId}[/]";

        string championText;
        string statusText;

        if (member.ChampionId > 0)
        {
            var champName = GetChampionName(member.ChampionId);
            championText = $"[white bold]{champName}[/]";
            statusText = "[green]✓ Gepickt[/]";
        }
        else if (member.ChampionPickIntent.HasValue && member.ChampionPickIntent.Value > 0)
        {
            var intentName = GetChampionName(member.ChampionPickIntent.Value);
            championText = $"[yellow]{intentName}[/]";
            statusText = "[yellow]🎯 Intent[/]";
        }
        else
        {
            championText = "[grey]---[/]";
            statusText = "[grey]⏳ Wartet[/]";
        }

        table.AddRow(cellText, championText, statusText);
    }

    return table;
}

static Layout CreateGameLayout(AllGameData gameData)
{
    var layout = new Layout("Root")
        .SplitRows(
            new Layout("Header").Size(3),
            new Layout("Content"));

    // Header
    var gameTime = TimeSpan.FromSeconds(gameData.GameData?.GameTime ?? 0);
    var headerPanel = new Panel(
            new Markup(
                $"[green bold]🎮 LIVE GAME[/]\n[grey]{gameData.GameData?.MapName} - {gameData.GameData?.GameMode} - {gameTime:mm\\:ss}[/]"))
        .Border(BoxBorder.Double)
        .BorderColor(Color.Green);
    layout["Header"].Update(headerPanel);

    // Content - Split in columns
    layout["Content"].SplitColumns(
        new Layout("Left"),
        new Layout("Right"));

    // Left side - Player Stats
    layout["Left"].SplitRows(
        new Layout("PlayerInfo"),
        new Layout("Teams"));

    layout["Left"]["PlayerInfo"].Update(CreatePlayerStatsTable(gameData.ActivePlayer));
    layout["Left"]["Teams"].Update(CreateTeamsTable(gameData.AllPlayers));

    // Right side - Events
    layout["Right"].Update(CreateEventsTable(gameData.Events));

    return layout;
}

static Panel CreatePlayerStatsTable(ActivePlayer? player)
{
    if (player == null)
        return new Panel("[grey]Keine Spielerdaten[/]");

    var grid = new Grid();
    grid.AddColumn();
    grid.AddColumn();

    grid.AddRow(
        new Markup($"[cyan bold]{player.SummonerName}[/]"),
        new Markup($"[yellow]Level {player.Level}[/]"));

    grid.AddRow(
        new Markup($"[yellow]💰[/] {player.CurrentGold:F0}g"),
        new Markup(""));

    if (player.ChampionStats != null)
    {
        var stats = player.ChampionStats;

        grid.AddEmptyRow();

        var hpBar = new BarChart()
            .Width(30)
            .Label("[green]HP[/]")
            .AddItem("", (double)stats.CurrentHealth, Color.Green)
            .AddItem("", (double)(stats.MaxHealth - stats.CurrentHealth), Color.Grey);

        grid.AddRow(new Markup($"[green]❤️ HP[/]"),
            new Markup($"[white]{stats.CurrentHealth:F0}/{stats.MaxHealth:F0}[/]"));
        grid.AddRow(new Markup($"[red]⚔️ AD[/]"), new Markup($"[white]{stats.AttackDamage:F0}[/]"));
        grid.AddRow(new Markup($"[blue]✨ AP[/]"), new Markup($"[white]{stats.AbilityPower:F0}[/]"));
        grid.AddRow(new Markup($"[yellow]🛡️ Armor[/]"), new Markup($"[white]{stats.Armor:F0}[/]"));
        grid.AddRow(new Markup($"[purple]🔮 MR[/]"), new Markup($"[white]{stats.MagicResist:F0}[/]"));
        grid.AddRow(new Markup($"[cyan]👟 MS[/]"), new Markup($"[white]{stats.MoveSpeed:F0}[/]"));
    }

    return new Panel(grid)
        .Header("[cyan bold]Dein Champion[/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Cyan1);
}

static Table CreateTeamsTable(List<Player>? players)
{
    var table = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.Blue)
        .Title("[blue bold]Team Übersicht[/]");

    table.AddColumn("[yellow]Status[/]");
    table.AddColumn("[cyan]Champion[/]");
    table.AddColumn("[white]Name[/]");
    table.AddColumn("[green]KDA[/]");
    table.AddColumn("[yellow]CS[/]");

    if (players == null || players.Count == 0)
        return table;

    var allies = players.Where(p => p.Team == "ORDER").ToList();
    var enemies = players.Where(p => p.Team == "CHAOS").ToList();

    // Allies
    table.AddRow(new Markup("[blue bold]═══ VERBÜNDETE ═══[/]"), new Markup(""), new Markup(""), new Markup(""),
        new Markup(""));

    foreach (var player in allies)
    {
        var status = player.IsDead ? "[red]💀[/]" : "[green]✓[/]";
        var kda = player.Scores != null
            ? $"[white]{player.Scores.Kills}[/]/[red]{player.Scores.Deaths}[/]/[yellow]{player.Scores.Assists}[/]"
            : "[grey]N/A[/]";
        var cs = player.Scores?.CreepScore ?? 0;

        table.AddRow(
            status,
            $"[cyan]{player.ChampionName}[/]",
            $"[white]{player.SummonerName}[/]",
            kda,
            $"[yellow]{cs}[/]");
    }

    table.AddEmptyRow();

    // Enemies
    table.AddRow(new Markup("[red bold]═══ GEGNER ═══[/]"), new Markup(""), new Markup(""), new Markup(""),
        new Markup(""));

    foreach (var player in enemies)
    {
        var status = player.IsDead ? "[grey]💀[/]" : "[red]✓[/]";
        var kda = player.Scores != null
            ? $"[white]{player.Scores.Kills}[/]/[red]{player.Scores.Deaths}[/]/[yellow]{player.Scores.Assists}[/]"
            : "[grey]N/A[/]";
        var cs = player.Scores?.CreepScore ?? 0;

        table.AddRow(
            status,
            $"[red]{player.ChampionName}[/]",
            $"[grey]{player.SummonerName}[/]",
            kda,
            $"[yellow]{cs}[/]");
    }

    return table;
}

static Panel CreateEventsTable(Event? events)
{
    var table = new Table()
        .Border(TableBorder.None)
        .HideHeaders();

    table.AddColumn("Event");

    if (events?.EventsList == null || events.EventsList.Count == 0)
    {
        table.AddRow("[grey]Keine Events[/]");
    }
    else
    {
        var recentEvents = events.EventsList.TakeLast(8).Reverse().ToList();

        foreach (var evt in recentEvents)
        {
            var time = TimeSpan.FromSeconds(evt.EventTime);
            var description = GetEventDescriptionMarkup(evt);

            if (!string.IsNullOrEmpty(description))
            {
                table.AddRow($"[grey]{time:mm\\:ss}[/] {description}");
            }
        }
    }

    return new Panel(table)
        .Header("[yellow bold]Letzte Events[/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Yellow);
}

static string GetEventDescriptionMarkup(GameEvent evt)
{
    return evt.EventName switch
    {
        "ChampionKill" => $"[red]💀[/] {evt.KillerName} → {evt.VictimName}",
        "Multikill" when evt.KillStreak >= 5 => $"[red bold]🔥 PENTA KILL[/] {evt.KillerName}!",
        "Multikill" when evt.KillStreak == 4 => $"[red bold]🔥 QUADRA KILL[/] {evt.KillerName}!",
        "Multikill" when evt.KillStreak == 3 => $"[yellow bold]🔥 TRIPLE KILL[/] {evt.KillerName}!",
        "Multikill" when evt.KillStreak == 2 => $"[yellow]🔥 DOUBLE KILL[/] {evt.KillerName}!",
        "DragonKill" => $"[blue]🐉 {evt.DragonType}[/] Drache → {evt.KillerName}",
        "BaronKill" => $"[purple bold]👹 Baron[/] → {evt.KillerName}",
        "TurretKilled" => $"[grey]🏰 Turm zerstört[/]",
        "InhibKilled" => $"[red]⚔️ Inhibitor zerstört[/]",
        "InhibRespawned" => $"[green]🔄 Inhibitor respawned[/]",
        "Ace" => $"[red bold]👑 ACE[/] Team {evt.AcingTeam}",
        "GameStart" => "[green]🎮 Spiel gestartet[/]",
        "MinionsSpawning" => "[cyan]🏁 Minions spawnen[/]",
        _ => $"[grey]{evt.EventName}[/]"
    };
}

// ...existing code...

static string GetQueueName(int queueId)
{
    return queueId switch
    {
        420 => "Ranked Solo/Duo",
        440 => "Ranked Flex",
        450 => "ARAM",
        400 => "Normal Draft",
        430 => "Normal Blind",
        490 => "Normal Quickplay",
        700 => "Clash",
        830 => "Intro Co-op vs. AI",
        840 => "Beginner Co-op vs. AI",
        850 => "Intermediate Co-op vs. AI",
        900 => "ARURF",
        1020 => "One for All",
        1300 => "Nexus Blitz",
        1400 => "Ultimate Spellbook",
        1900 => "URF",
        _ => $"Queue ID: {queueId}"
    };
}

static string GetChampionName(int championId)
{
    return championId switch
    {
        1 => "Annie", 2 => "Olaf", 3 => "Galio", 4 => "Twisted Fate", 5 => "Xin Zhao",
        6 => "Urgot", 7 => "LeBlanc", 8 => "Vladimir", 9 => "Fiddlesticks", 10 => "Kayle",
        11 => "Master Yi", 12 => "Alistar", 13 => "Ryze", 14 => "Sion", 15 => "Sivir",
        16 => "Soraka", 17 => "Teemo", 18 => "Tristana", 19 => "Warwick", 20 => "Nunu",
        21 => "Miss Fortune", 22 => "Ashe", 23 => "Tryndamere", 24 => "Jax", 25 => "Morgana",
        26 => "Zilean", 27 => "Singed", 28 => "Evelynn", 29 => "Twitch", 30 => "Karthus",
        31 => "Cho'Gath", 32 => "Amumu", 33 => "Rammus", 34 => "Anivia", 35 => "Shaco",
        36 => "Dr. Mundo", 37 => "Sona", 38 => "Kassadin", 39 => "Irelia", 40 => "Janna",
        41 => "Gangplank", 42 => "Corki", 43 => "Karma", 44 => "Taric", 45 => "Veigar",
        48 => "Trundle", 50 => "Swain", 51 => "Caitlyn", 53 => "Blitzcrank", 54 => "Malphite",
        55 => "Katarina", 56 => "Nocturne", 57 => "Maokai", 58 => "Renekton", 59 => "Jarvan IV",
        60 => "Elise", 61 => "Orianna", 62 => "Wukong", 63 => "Brand", 64 => "Lee Sin",
        67 => "Vayne", 68 => "Rumble", 69 => "Cassiopeia", 72 => "Skarner", 74 => "Heimerdinger",
        75 => "Nasus", 76 => "Nidalee", 77 => "Udyr", 78 => "Poppy", 79 => "Gragas",
        80 => "Pantheon", 81 => "Ezreal", 82 => "Mordekaiser", 83 => "Yorick", 84 => "Akali",
        85 => "Kennen", 86 => "Garen", 89 => "Leona", 90 => "Malzahar", 91 => "Talon",
        92 => "Riven", 96 => "Kog'Maw", 98 => "Shen", 99 => "Lux", 101 => "Xerath",
        102 => "Shyvana", 103 => "Ahri", 104 => "Graves", 105 => "Fizz", 106 => "Volibear",
        107 => "Rengar", 110 => "Varus", 111 => "Nautilus", 112 => "Viktor", 113 => "Sejuani",
        114 => "Fiora", 115 => "Ziggs", 117 => "Lulu", 119 => "Draven", 120 => "Hecarim",
        121 => "Kha'Zix", 122 => "Darius", 126 => "Jayce", 127 => "Lissandra", 131 => "Diana",
        133 => "Quinn", 134 => "Syndra", 136 => "Aurelion Sol", 141 => "Kayn", 142 => "Zoe",
        143 => "Zyra", 145 => "Kai'Sa", 147 => "Seraphine", 150 => "Gnar", 154 => "Zac",
        157 => "Yasuo", 161 => "Vel'Koz", 163 => "Taliyah", 164 => "Camille", 166 => "Akshan",
        200 => "Bel'Veth", 201 => "Braum", 202 => "Jhin", 203 => "Kindred", 221 => "Zeri",
        222 => "Jinx", 223 => "Tahm Kench", 234 => "Viego", 235 => "Senna", 236 => "Lucian",
        238 => "Zed", 240 => "Kled", 245 => "Ekko", 246 => "Qiyana", 254 => "Vi",
        266 => "Aatrox", 267 => "Nami", 268 => "Azir", 350 => "Yuumi", 360 => "Samira",
        412 => "Thresh", 420 => "Illaoi", 421 => "Rek'Sai", 427 => "Ivern", 429 => "Kalista",
        432 => "Bard", 497 => "Rakan", 498 => "Xayah", 516 => "Ornn", 517 => "Sylas",
        518 => "Neeko", 523 => "Aphelios", 526 => "Rell", 555 => "Pyke", 711 => "Vex",
        777 => "Yone", 875 => "Sett", 876 => "Lillia", 887 => "Gwen", 888 => "Renata Glasc",
        895 => "Nilah", 897 => "K'Sante", 902 => "Milio", 910 => "Hwei", 950 => "Naafiri",
        _ => $"Champion #{championId}"
    };
}

Console.WriteLine();


Console.WriteLine("Überwache League of Legends Client...");
Console.WriteLine("Funktionen:");
Console.WriteLine("  • Lobby-Überwachung mit Team-Anzeige");
Console.WriteLine("  • Champion-Select mit Picks");
Console.WriteLine("  • Automatisches Ready-Check-Accept");
Console.WriteLine("  • Live Game Data");
Console.WriteLine();
Console.WriteLine("(Drücke Strg+C zum Beenden)");
Console.WriteLine();

// Main monitoring loop
while (true)
{
    try
    {
        // Try to get game data first
        var allGameData = await reader.GetAllGameDataAsync();

        if (allGameData != null)
        {
            // ========== IM SPIEL ==========
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║             🎮 LIVE GAME - League of Legends                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            DisplayGameInfo(allGameData.GameData);
            Console.WriteLine();

            DisplayActivePlayer(allGameData.ActivePlayer);
            Console.WriteLine();

            DisplayTeamOverview(allGameData.AllPlayers);
            Console.WriteLine();

            DisplayRecentEvents(allGameData.Events);
            Console.WriteLine();

            Console.WriteLine("Nächste Aktualisierung in 5 Sekunden...");
            await Task.Delay(5000);
        }
        else
        {
            // ========== NICHT IM SPIEL - PRÜFE LOBBY ==========
            var lobby = await reader.GetLobbyAsync();

            if (lobby != null)
            {
                // ========== IN LOBBY ==========
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║             🏠 LOBBY - League of Legends                       ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
                Console.WriteLine();

                DisplayLobby(lobby);
                Console.WriteLine();

                // Check for champion select
                var champSelect = await reader.GetChampSelectSessionAsync();
                if (champSelect != null)
                {
                    DisplayChampSelect(champSelect);
                    Console.WriteLine();
                }

                // Check for ready check and auto-accept
                var readyCheck = await reader.GetReadyCheckAsync();
                if (readyCheck != null)
                {
                    await HandleReadyCheck(reader, readyCheck);
                    Console.WriteLine();
                }

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Nächste Aktualisierung in 1 Sekunden...");
                await Task.Delay(800);
            }
            else
            {
                // ========== WEDER LOBBY NOCH SPIEL ==========
                Console.Write($"\r[{DateTime.Now:HH:mm:ss}] Warte auf Lobby oder Spiel...    ");
                await Task.Delay(2000);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] Fehler: {ex.Message}");
        await Task.Delay(2000);
    }
}

// Helper Methods

static void DisplayGameInfo(GameData? gameData)
{
    if (gameData == null) return;

    Console.WriteLine("┌─── SPIEL-INFORMATIONEN ────────────────────────────────────────┐");
    Console.WriteLine($"│ Map: {gameData.MapName,-54} │");
    Console.WriteLine($"│ Spielmodus: {gameData.GameMode,-46} │");

    var gameTime = TimeSpan.FromSeconds(gameData.GameTime);
    Console.WriteLine($"│ Spielzeit: {gameTime:mm\\:ss}                                              │");
    Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
}

static void DisplayActivePlayer(ActivePlayer? player)
{
    if (player == null) return;

    Console.WriteLine("┌─── AKTIVER SPIELER ────────────────────────────────────────────┐");
    Console.WriteLine($"│ Name: {player.SummonerName,-51} │");
    Console.WriteLine($"│ Level: {player.Level,-50} │");
    Console.WriteLine($"│ Gold: {player.CurrentGold:F0}                                                  │");
    Console.WriteLine("│                                                                │");

    if (player.ChampionStats != null)
    {
        var stats = player.ChampionStats;
        Console.WriteLine(
            $"│ HP: {stats.CurrentHealth:F0}/{stats.MaxHealth:F0} │ AD: {stats.AttackDamage:F0} │ AP: {stats.AbilityPower:F0}            │");
        Console.WriteLine(
            $"│ Armor: {stats.Armor:F0} │ MR: {stats.MagicResist:F0} │ MS: {stats.MoveSpeed:F0}               │");
        Console.WriteLine(
            $"│ Angriffstempo: {stats.AttackSpeed:F2} │ Krit-Chance: {stats.CritChance * 100:F0}%          │");
    }

    if (player.FullRunes?.Keystone != null)
    {
        Console.WriteLine("│                                                                │");
        Console.WriteLine($"│ Hauptrune: {player.FullRunes.Keystone.DisplayName,-44} │");
        Console.WriteLine($"│ Primärer Baum: {player.FullRunes.PrimaryRuneTree?.DisplayName,-39} │");
        Console.WriteLine($"│ Sekundärer Baum: {player.FullRunes.SecondaryRuneTree?.DisplayName,-37} │");
    }

    Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
}

static void DisplayTeamOverview(List<Player>? players)
{
    if (players == null || players.Count == 0) return;

    var allies = players.Where(p => p.Team == "ORDER").ToList();
    var enemies = players.Where(p => p.Team == "CHAOS").ToList();

    Console.WriteLine("┌─── TEAM ÜBERSICHT ─────────────────────────────────────────────┐");
    Console.WriteLine("│                                                                │");
    Console.WriteLine("│ VERBÜNDETE (Blau):                                            │");
    foreach (var player in allies)
    {
        var status = player.IsDead ? "💀" : "✓";
        var kda = player.Scores != null
            ? $"{player.Scores.Kills}/{player.Scores.Deaths}/{player.Scores.Assists}"
            : "N/A";
        var cs = player.Scores?.CreepScore ?? 0;

        Console.WriteLine(
            $"│ {status} {player.ChampionName,-12} │ {player.SummonerName,-20} │ {kda,-7} │ CS:{cs,-3} │");
    }

    Console.WriteLine("│                                                                │");
    Console.WriteLine("│ GEGNER (Rot):                                                  │");
    foreach (var player in enemies)
    {
        var status = player.IsDead ? "💀" : "✓";
        var kda = player.Scores != null
            ? $"{player.Scores.Kills}/{player.Scores.Deaths}/{player.Scores.Assists}"
            : "N/A";
        var cs = player.Scores?.CreepScore ?? 0;

        Console.WriteLine(
            $"│ {status} {player.ChampionName,-12} │ {player.SummonerName,-20} │ {kda,-7} │ CS:{cs,-3} │");
    }

    Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
}

static void DisplayRecentEvents(Event? events)
{
    if (events?.EventsList == null || events.EventsList.Count == 0) return;

    // Get the last 5 events
    var recentEvents = events.EventsList.TakeLast(5).Reverse().ToList();

    Console.WriteLine("┌─── LETZTE EREIGNISSE ──────────────────────────────────────────┐");

    foreach (var evt in recentEvents)
    {
        var time = TimeSpan.FromSeconds(evt.EventTime);
        var description = GetEventDescription(evt);

        if (!string.IsNullOrEmpty(description))
        {
            Console.WriteLine($"│ [{time:mm\\:ss}] {description,-53} │");
        }
    }

    Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
}

static string GetEventDescription(GameEvent evt)
{
    return evt.EventName switch
    {
        "ChampionKill" => $"💀 {evt.KillerName} hat {evt.VictimName} getötet",
        "Multikill" when evt.KillStreak >= 5 => $"🔥 PENTA KILL für {evt.KillerName}!",
        "Multikill" when evt.KillStreak == 4 => $"🔥 QUADRA KILL für {evt.KillerName}!",
        "Multikill" when evt.KillStreak == 3 => $"🔥 TRIPLE KILL für {evt.KillerName}!",
        "Multikill" when evt.KillStreak == 2 => $"🔥 DOUBLE KILL für {evt.KillerName}!",
        "DragonKill" => $"🐉 {evt.DragonType} Drache getötet von {evt.KillerName}",
        "BaronKill" => $"👹 Baron Nashor getötet von {evt.KillerName}",
        "TurretKilled" => $"🏰 Turm zerstört: {evt.TurretKilled}",
        "InhibKilled" => $"⚔️ Inhibitor zerstört: {evt.InhibKilled}",
        "InhibRespawned" => $"🔄 Inhibitor wiederbelebt: {evt.InhibRespawned}",
        "Ace" => $"👑 ACE! Team {evt.AcingTeam}",
        "GameStart" => "🎮 Spiel gestartet",
        "MinionsSpawning" => "🏁 Minions spawnen",
        _ => evt.EventName ?? "Unbekanntes Event"
    };
}

static void DisplayLobby(Lobby lobby)
{
    Console.WriteLine("┌─── LOBBY INFORMATIONEN ────────────────────────────────────────┐");
    Console.WriteLine($"│ Mitglieder: {lobby.Members.Length,-49} │");

    if (lobby.GameConfig?.QueueId.HasValue == true)
    {
        var queueName = GetQueueName(lobby.GameConfig.QueueId.Value);
        Console.WriteLine($"│ Queue: {queueName,-52} │");
    }

    Console.WriteLine("│                                                                │");
    Console.WriteLine("│ Team:                                                          │");

    foreach (var member in lobby.Members)
    {
        Console.WriteLine($"│   • {member.SummonerName,-57} │");
    }

    Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
}

static void DisplayChampSelect(ChampSelectSession session)
{
    Console.WriteLine("┌─── CHAMPION SELECT ────────────────────────────────────────────┐");
    Console.WriteLine($"│ Phase: {session.Timer?.Phase,-52} │");
    Console.WriteLine($"│ Deine Cell ID: {session.LocalPlayerCellId,-46} │");
    Console.WriteLine("│                                                                │");

    if (session.MyTeam.Count > 0)
    {
        Console.WriteLine("│ Team:                                                          │");
        Console.WriteLine("│                                                                │");

        foreach (var member in session.MyTeam)
        {
            var isYou = member.CellId == session.LocalPlayerCellId ? "👤 DU" : "     ";

            if (member.ChampionId > 0)
            {
                var champName = GetChampionName(member.ChampionId);
                var paddedName = champName.PadRight(43);
                Console.WriteLine($"│ {isYou} Cell {member.CellId}: ✓ {paddedName} │");
            }
            else if (member.ChampionPickIntent.HasValue && member.ChampionPickIntent.Value > 0)
            {
                var intentName = GetChampionName(member.ChampionPickIntent.Value);
                var paddedIntent = intentName.PadRight(35);
                Console.WriteLine($"│ {isYou} Cell {member.CellId}: 🎯 Intent: {paddedIntent} │");
            }
            else
            {
                Console.WriteLine($"│ {isYou} Cell {member.CellId}: ⏳ Noch nicht gewählt                      │");
            }
        }
    }

    Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
}

static async Task HandleReadyCheck(LiveClientObjectReader reader, ReadyCheckDto readyCheck)
{
    if (readyCheck.State == "InProgress")
    {
        Console.WriteLine("┌─── READY CHECK ────────────────────────────────────────────────┐");
        Console.WriteLine("│ 🔔 SPIEL GEFUNDEN!                                             │");
        Console.WriteLine("│                                                                │");
        Console.WriteLine("│ Akzeptiere automatisch...                                      │");

        var accepted = await reader.AcceptReadyCheckAsync();

        if (accepted)
        {
            Console.WriteLine("│ ✓ Ready Check erfolgreich akzeptiert!                          │");
        }
        else
        {
            Console.WriteLine("│ ✗ Konnte Ready Check nicht akzeptieren                         │");
        }

        Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
    }
    else if (readyCheck.State == "EveryoneReady")
    {
        Console.WriteLine("┌─── READY CHECK ────────────────────────────────────────────────┐");
        Console.WriteLine("│ ✓ Alle bereit! Spiel startet...                                │");
        Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
    }
}
