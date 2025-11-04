# BE.League.Desktop

This project is a personal non commercial fun/educational project. All rights any thing realted to League of Legends are owned by Riot Games.
Also this project is not affiliated with Riot Games in any way and not approved by Riot Games or anyone else.
So use at own risk..


It is a Lightweight .NET 9 client for:
- Live Client Data API (`/liveclientdata/...`) while a game is running.
- League Client API (LCU) for lobby, champ select, and matchmaking.

It runs locally, auto-discovers the League Client `lockfile`, builds the LCU base URL including Basic Auth, and exposes simple typed methods via `LiveClientObjectReader`.

All documentation is in English.

## Requirements
- .NET 9 (`net9.0`)
- Windows
- League Client installed and running

Packages are referenced via the project files; no manual installation is required to build the solution.

## Connection flow: lockfile → URL + auth

1) Resolve `lockfile`:
   - Scans running processes `LeagueClientUx`/`LeagueClient`.
   - Tries the executable directory and its parent for a file named `lockfile`.
   - Fallback paths: `C:\Riot Games\League of Legends\lockfile` and `D:\Riot Games\League of Legends\lockfile`.
   - Env override: `LEAGUE_LOCKFILE`.

2) Parse `lockfile`:
   - Format: `name:pid:port:password:protocol`.
   - Extracts `port`, `password` and `protocol` for the local LCU endpoint.

3) Build LCU base URL and Basic Auth header:
   - Base URL: `https://127.0.0.1:{port}` (protocol is typically `https`).
   - Auth header: `Authorization: Basic {Base64("riot:{password}")}`.
   - Self-signed certificates are accepted locally.

4) Clients / APIs:
   - Live Client Data API methods call `GET /liveclientdata/...` on the configured base address.
   - LCU methods call `GET/POST /lol-...` on the LCU client with Basic Auth.
   - `LiveClientObjectReader` wraps these JSON endpoints and deserializes into typed models.

Notes:
- Live Client Data API is commonly exposed on `127.0.0.1:2999` without auth. The current gateway auto-detects the base address; if you must force `2999`, supply a custom `LeagueDesktopOptions` with your own base address behavior.

## Projects and key files
- `BE.League.Desktop`
  - `Connection/ClientHelper.cs` — Finds and reads the `lockfile`.
  - `Connection/LeagueClientConnectionInfo.cs` — Holds LCU connection data (base URL, token, protocol).
  - `Connection/LeagueDesktopOptions.cs` — Options for timeouts and connection injection.
  - `LeagueDesktopClient.cs` — HTTP gateway for Live Client Data API and LCU.
  - `LiveClientObjectReader.cs` — Typed reader that deserializes JSON to models and exposes async APIs.
- `Examples/BE.League.Desktop.AutoAccept`
  - Minimal Spectre.Console-based console app that auto-accepts the ready check with a small delay, and can auto-exit.

## Build & run

You can use Rider/Visual Studio to open `BE.League.sln` and run any project, or use the .NET CLI.

### Using Rider/Visual Studio
- Open the solution `BE.League.sln`.
- Ensure the League Client is running.
- Select the desired project (e.g., `Examples/BE.League.Desktop.AutoAccept`) and run.

### Using .NET CLI
From the repository root:

```
REM Build everything
 dotnet build BE.League.sln -c Release

REM Run the AutoAccept example
 dotnet run --project Examples/BE.League.Desktop.AutoAccept/BE.League.Desktop.AutoAccept.csproj -c Release
```

## Usage (API examples)

Create a typed reader and call the desired endpoints. All methods are async and return `null` on timeout/unavailable cases.

### Get the active player
```csharp
var reader = new BE.Riot.Gateways.LeagueDesktop.LiveClientObjectReader();

var activePlayer = await reader.GetActivePlayerAsync();
if (activePlayer != null)
{
    Console.WriteLine($"Name: {activePlayer.SummonerName}");
    Console.WriteLine($"Champion: {activePlayer.ChampionName}");
    Console.WriteLine($"Gold: {activePlayer.CurrentGold:F0}");
}
```

### List all players
```csharp
var reader = new BE.Riot.Gateways.LeagueDesktop.LiveClientObjectReader();

var players = await reader.GetPlayerListAsync();
if (players != null)
{
    Console.WriteLine($"Players: {players.Count}");
    foreach (var p in players)
        Console.WriteLine($"- {p.ChampionName} ({p.SummonerName}) - Team: {p.Team}");
}
```

### Recent game events
```csharp
var reader = new BE.Riot.Gateways.LeagueDesktop.LiveClientObjectReader();

var events = await reader.GetEventDataAsync();
if (events?.EventsList != null)
{
    foreach (var evt in events.EventsList.TakeLast(10))
    {
        var t = TimeSpan.FromSeconds(evt.EventTime);
        Console.WriteLine($"[{t:mm\\:ss}] {evt.EventName}");
    }
}
```

### Player-specific data (scores, items, spells)
```csharp
var reader = new BE.Riot.Gateways.LeagueDesktop.LiveClientObjectReader();
string summonerName = "Your Summoner";

var scores = await reader.GetPlayerScoresAsync(summonerName);
if (scores != null)
{
    Console.WriteLine($"KDA: {scores.Kills}/{scores.Deaths}/{scores.Assists}");
    Console.WriteLine($"CS: {scores.CreepScore}");
}

var items = await reader.GetPlayerItemsAsync(summonerName);
if (items != null)
{
    foreach (var item in items)
        Console.WriteLine($"Slot {item.Slot}: {item.DisplayName} (ID: {item.ItemId})");
}

var spells = await reader.GetPlayerSummonerSpellsAsync(summonerName);
if (spells != null)
{
    Console.WriteLine(spells.SummonerSpellOne?.DisplayName);
    Console.WriteLine(spells.SummonerSpellTwo?.DisplayName);
}
```

### Game stats (map, mode, time)
```csharp
var reader = new BE.Riot.Gateways.LeagueDesktop.LiveClientObjectReader();

var game = await reader.GetGameStatsAsync();
if (game != null)
{
    Console.WriteLine($"Map: {game.MapName}");
    Console.WriteLine($"Mode: {game.GameMode}");
    Console.WriteLine($"Time: {TimeSpan.FromSeconds(game.GameTime):mm\\:ss}");
}
```

### Abilities and runes of the active player
```csharp
var reader = new BE.Riot.Gateways.LeagueDesktop.LiveClientObjectReader();

var abilities = await reader.GetActivePlayerAbilitiesAsync();
if (abilities != null)
{
    Console.WriteLine($"Passive: {abilities.Passive?.DisplayName}");
    Console.WriteLine($"Q: {abilities.Q?.DisplayName} (Lv {abilities.Q?.AbilityLevel})");
    Console.WriteLine($"W: {abilities.W?.DisplayName} (Lv {abilities.W?.AbilityLevel})");
    Console.WriteLine($"E: {abilities.E?.DisplayName} (Lv {abilities.E?.AbilityLevel})");
    Console.WriteLine($"R: {abilities.R?.DisplayName} (Lv {abilities.R?.AbilityLevel})");
}

var runes = await reader.GetActivePlayerRunesAsync();
if (runes != null)
{
    Console.WriteLine($"Keystone: {runes.Keystone?.DisplayName}");
    Console.WriteLine($"Primary: {runes.PrimaryRuneTree?.DisplayName}");
    Console.WriteLine($"Secondary: {runes.SecondaryRuneTree?.DisplayName}");
}
```

### Continuous monitoring with cancellation
```csharp
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

var reader = new BE.Riot.Gateways.LeagueDesktop.LiveClientObjectReader();
Console.WriteLine("Monitoring... press Ctrl+C to stop.");

while (!cts.IsCancellationRequested)
{
    var gs = await reader.GetGameStatsAsync(cts.Token);
    if (gs != null)
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {TimeSpan.FromSeconds(gs.GameTime):mm\\:ss}");

    await Task.Delay(5000, cts.Token);
}
```

### Accept ready check (LCU)
```csharp
var reader = new BE.Riot.Gateways.LeagueDesktop.LiveClientObjectReader();

var ready = await reader.GetReadyCheckAsync();
if (ready != null)
{
    bool accepted = await reader.AcceptReadyCheckAsync();
    Console.WriteLine($"Ready check accepted: {accepted}");
}
```

### Custom options (timeout, injected connection)
```csharp
var options = new BE.League.Desktop.LeagueDesktopOptions
{
    Connection = BE.League.Desktop.LeagueClientConnectionInfo.GetFromRunningClient(),
    Timeout = TimeSpan.FromSeconds(5)
};

var reader = new BE.Riot.Gateways.LeagueDesktop.LiveClientObjectReader(options);
var me = await reader.GetActivePlayerAsync();
Console.WriteLine(me?.SummonerName ?? "N/A");
```

## Examples: Auto-accept console

The sample console app under `Examples/BE.League.Desktop.AutoAccept` uses Spectre.Console to:
- Watch for ready checks in a live display,
- Optionally wait a configurable delay before accepting,
- Optionally exit automatically after acceptance.

Run it from your IDE or via the .NET CLI as shown above.

## Error handling
- All gateway calls return `null` on network errors, timeouts, or when the API is not available.
- Wrap long-running loops with a `CancellationToken`.
- LCU calls require the League Client to be running and unlocked.

## Security
- Communication is local to `127.0.0.1`.
- Self-signed certificates are accepted to simplify local HTTPS to the LCU.
- Credentials are read from the `lockfile` and used only for Basic Auth to the local LCU.

## Notes
- Target framework: `net9.0`.
- If you want to force the classic Live Client Data API port `2999`, provide a custom gateway or `LeagueDesktopOptions` that points the `_httpClient` to that port.

## Documentation

- 📚 [LCU&Live Api](src/BE.League.Desktop/API_SEPARATION.md) - Sepearted Api Classes
- 📚 [API References](src/BE.League.Desktop/ApiReferences.md) - Complete API documentation for LCU and Live Client APIs