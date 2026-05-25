# BE.League.Desktop — AI-Generated Library Structure

> **Auto-generated**: This document was created by analyzing the full source tree of the `BE.League.Desktop` solution.  
> **Purpose**: Provide LLMs and developers with a quick, authoritative map of the library — what it does, how it's layered, and how to use it.

---

## 1. What This Library Does

`BE.League.Desktop` is a C# (.NET 10) library that talks to the **two local HTTP APIs** exposed by a running League of Legends client on Windows:

| API | Official Name | Fixed Port | Auth | Available When |
|-----|---------------|-----------|------|----------------|
| **[Game Client API](https://developer.riotgames.com/docs/lol#game-client-api)** | `GameClient` | `2999` | None | An active game is running |
| **[League Client API](https://developer.riotgames.com/docs/lol#league-client-api)** | `LeagueClient` | Dynamic (from lockfile) | Basic Auth (`riot:<token>`) | The League Client process is open |

Both APIs run on `127.0.0.1` with self-signed HTTPS certificates.

---

## 2. Solution Layout

```
src/
├── BE.League.Desktop/                          # Core library (NuGet-packageable)
│   ├── Connection/                             # Lockfile discovery & connection config
│   │   ├── ClientHelper.cs                     #   Find lockfile via process / known paths / env var
│   │   ├── LeagueClientConnectionInfo.cs       #   Parse lockfile → port + token
│   │   └── LeagueDesktopOptions.cs             #   Options record (connection, base URL, timeout)
│   ├── LeagueClientApi/                        # League Client API layer (client/lobby)
│   │   ├── ILeagueClientApi.cs                 #   Interface — raw JSON methods
│   │   ├── LeagueClientApiClient.cs            #   HttpClient implementation (Basic Auth)
│   │   ├── LeagueClientApiReader.cs            #   Deserializes JSON → typed models
│   │   └── LeagueClientApiEventLoop.cs         #   Polling loop with LobbyChanged / ReadyCheckChanged events
│   ├── GameClientApi/                          # Game Client API layer (in-game)
│   │   ├── IGameClientApi.cs                   #   Interface — raw JSON methods
│   │   ├── GameClientApiClient.cs              #   HttpClient implementation (no auth, port 2999)
│   │   └── GameClientApiReader.cs              #   Deserializes JSON → typed models
│   ├── Models/                                 # All data models (shared by both APIs)
│   │   ├── LiveClientDataModels.cs             #   AllGameData, ActivePlayer, Player, Item, Scores, …
│   │   ├── LeagueClientLobbyModels.cs          #   Summoner, GameFlowSession, SearchState, …
│   │   ├── Lobby.cs                            #   Lobby
│   │   ├── LobbyMember.cs                      #   LobbyMember
│   │   ├── LobbyInvitation.cs                  #   LobbyInvitation
│   │   ├── GameConfig.cs                       #   GameConfig
│   │   ├── ChampSelectSession.cs               #   ChampSelectSession (+ nested TimerObj, ActionObj, TeamMember)
│   │   ├── ReadyCheckState.cs                  #   ReadyCheck
│   │   ├── MucJwt.cs                           #   MucJwt
│   │   ├── LiveEvent.cs                        #   LiveEvent
│   │   └── LiveWrapper.cs                      #   LiveWrapper
│   ├── ILeagueDesktopClient.cs                 # Unified interface exposing GameClient + LeagueClient
│   ├── LeagueDesktopClient.cs                  # Unified facade — creates both readers
│   └── LeagueJsonContext.cs                    # System.Text.Json source generator context
│
├── BE.League.Desktop.Tests/                    # Unit tests (xUnit + FakeItEasy)
│   ├── ModelTests/                             #   Deserialization tests per model
│   │   ├── GivenChampSelectSession.cs
│   │   ├── GivenGameConfigDto.cs
│   │   ├── GivenLiveEvent.cs
│   │   ├── GivenLiveWrapper.cs
│   │   ├── GivenLobbyDto.cs
│   │   ├── GivenLobbyInvitation.cs
│   │   ├── GivenLobbyMember.cs
│   │   ├── GivenMucJwtDto.cs
│   │   └── GivenReadyCheckDto.cs
│   └── LeagueClientApi/
│       └── GivenLeagueClientApiEventLoop.cs    #   Event loop polling tests
│
├── BE.League.Desktop.IntegrationTests/         # Integration tests (require running League Client)
│   ├── GivenLeagueDesktopClient.cs             #   Basic connectivity
│   ├── WhenInActiveGame.cs                     #   Game Client API (needs active match)
│   └── WhenInLeagueClient.cs                   #   League Client API (needs client open)
│
├── Examples/
│   ├── BE.League.Desktop.AutoAccept/           # Spectre.Console app — auto-accepts ready checks
│   │   ├── Program.cs
│   │   ├── MonitorLoop.cs                      #   Main polling loop
│   │   ├── MonitorState.cs                     #   State tracking
│   │   └── Displays.cs                         #   Spectre.Console table rendering
│   └── BE.League.Desktop.Console/              # RazorConsole experiment
│       ├── Program.cs
│       └── LobbyView.razor
│
├── LEAGUE_OF_LEGENDS_INTEGRATION.md            # Deep-dive: lockfile, endpoints, auth, troubleshooting
└── README.md                                   # Quick-start, feature list, project structure
```

---

## 3. Architecture — Layer by Layer

### 3.1 Connection Discovery (`Connection/`)

```
LeagueClient.exe starts → writes lockfile → ClientHelper reads it → LeagueClientConnectionInfo
```

| Class | Responsibility |
|-------|---------------|
| `ClientHelper` | Static helper. Finds lockfile path by: (1) scanning `LeagueClientUx`/`LeagueClient` processes, (2) checking `C:\Riot Games\…` and `D:\Riot Games\…`, (3) reading `LEAGUE_LOCKFILE` env var. Parses lockfile content (`name:pid:port:password:protocol`). |
| `LeagueClientConnectionInfo` | Holds `Port`, `Token`, `Protocol`. Factory method `GetFromRunningClient()`. Has `IsLeagueClientRunning()` and `WaitForLeagueClient(ct)`. |
| `LeagueDesktopOptions` | Configuration record: optional `Connection`, optional `LiveClientBaseUrl`, `Timeout` (default 10 s). |

### 3.2 League Client API (`LeagueClientApi/`)

Talks to the [League Client API](https://developer.riotgames.com/docs/lol#league-client-api) on a **dynamic port** with **Basic Auth**.
Community endpoint reference: [HexTech Docs](https://hextechdocs.dev/)

```
ILeagueClientApi  ←  LeagueClientApiClient (HttpClient, Basic Auth)
   ↓
LeagueClientApiReader (JSON → Lobby, ChampSelectSession, ReadyCheck)
   ↓
LeagueClientApiEventLoop (polling loop, fires LobbyChanged / ReadyCheckChanged events)
```

| Class | Layer | Description |
|-------|-------|-------------|
| `ILeagueClientApi` | Interface | 5 methods: `GetLobbyJsonAsync`, `GetChampSelectSessionJsonAsync`, `GetReadyCheckJsonAsync`, `AcceptReadyCheckAsync`, `DeclineReadyCheckAsync`. Returns raw `string?` JSON. |
| `LeagueClientApiClient` | Transport | Creates `HttpClient` with self-signed cert bypass + `Authorization: Basic …` header. Calls League Client API endpoints and returns raw JSON. Graceful null on failure. |
| `LeagueClientApiReader` | Deserialization | Wraps `ILeagueClientApi`. Deserializes via `System.Text.Json` + source-gen context into typed models (`Lobby`, `ChampSelectSession`, `ReadyCheck`). |
| `LeagueClientApiEventLoop` | Reactive | Polls `LeagueClientApiReader` every 500 ms. Fires `LobbyChanged` and `ReadyCheckChanged` events when values differ from previous iteration. |

**League Client API Endpoints Used:**

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/lol-lobby/v2/lobby` | GET | Current lobby state |
| `/lol-champ-select/v1/session` | GET | Champion select session |
| `/lol-matchmaking/v1/ready-check` | GET | Ready check state |
| `/lol-matchmaking/v1/ready-check/accept` | POST | Accept a found game |
| `/lol-matchmaking/v1/ready-check/decline` | POST | Decline a found game |

### 3.3 Game Client API (`GameClientApi/`)

Talks to the [Game Client API](https://developer.riotgames.com/docs/lol#game-client-api) on **fixed port 2999** with **no auth**.
OpenAPI spec: [liveclientdata_sample.json](https://static.developer.riotgames.com/docs/lol/liveclientdata_sample.json)

```
IGameClientApi  ←  GameClientApiClient (HttpClient, no auth, port 2999)
   ↓
GameClientApiReader (JSON → AllGameData, ActivePlayer, Player, Scores, …)
```

| Class | Layer | Description |
|-------|-------|-------------|
| `IGameClientApi` | Interface | 12 methods returning raw `string?` JSON: all-game-data, active player, player list, scores, items, events, etc. |
| `GameClientApiClient` | Transport | `HttpClient` pointed at `https://127.0.0.1:2999`. Self-signed cert bypass. No auth. |
| `GameClientApiReader` | Deserialization | Wraps `IGameClientApi`. Deserializes into typed models (`AllGameData`, `ActivePlayer`, `List<Player>`, `Scores`, `GameData`, `Event`, etc.). |

**Game Client API Endpoints Used:**

| Endpoint | Returns | Purpose |
|----------|---------|---------|
| `/liveclientdata/allgamedata` | `AllGameData` | Full game snapshot |
| `/liveclientdata/activeplayer` | `ActivePlayer` | Local player data |
| `/liveclientdata/activeplayername` | `string` | Local player name |
| `/liveclientdata/activeplayerabilities` | `Abilities` | Q/W/E/R/Passive |
| `/liveclientdata/activeplayerrunes` | `FullRunes` | Full rune config |
| `/liveclientdata/playerlist` | `List<Player>` | All 10 players |
| `/liveclientdata/playerscores?summonerName=…` | `Scores` | KDA + CS |
| `/liveclientdata/playersummonerspells?summonerName=…` | `SummonerSpells` | Flash/Ignite/etc. |
| `/liveclientdata/playermainrunes?summonerName=…` | `PlayerRunes` | Keystone + trees |
| `/liveclientdata/playeritems?summonerName=…` | `List<Item>` | Inventory |
| `/liveclientdata/eventdata` | `Event` | Kill/dragon/baron events |
| `/liveclientdata/gamestats` | `GameData` | Mode, time, map |

### 3.4 Unified Facade (`LeagueDesktopClient`)

```csharp
public sealed class LeagueDesktopClient : ILeagueDesktopClient
{
    public GameClientApiReader GameClient { get; }
    public LeagueClientApiReader LeagueClient { get; }
}
```

- The parameterless constructor auto-discovers connection info from the lockfile and creates both readers.
- The DI-friendly constructor accepts pre-built readers.
- Consumers pick `client.GameClient.*` for real-time in-game data or `client.LeagueClient.*` for lobby/client data.

### 3.5 Models (`Models/`)

All models use `System.Text.Json` attributes (`[JsonPropertyName]`) and most include `[JsonExtensionData]` for forward compatibility with unknown fields.

#### League Client API Models (used by `LeagueClientApiReader`)

| Model | File | Key Properties |
|-------|------|---------------|
| `Lobby` | `Lobby.cs` | `CanStartActivity`, `GameConfig`, `Members[]`, `Invitations`, `PartyId`, `PartyType` |
| `LobbyMember` | `LobbyMember.cs` | `SummonerId`, `SummonerName`, `IsLeader`, `Ready`, `FirstPositionPreference`, `SecondPositionPreference` |
| `LobbyInvitation` | `LobbyInvitation.cs` | `InvitationId`, `State`, `ToSummonerName` |
| `GameConfig` | `GameConfig.cs` | `GameMode`, `MapId`, `QueueId`, `MaxTeamSize`, `IsCustom` |
| `ChampSelectSession` | `ChampSelectSession.cs` | `LocalPlayerCellId`, `Actions[][]`, `MyTeam[]`, `Timer.Phase` |
| `ReadyCheck` | `ReadyCheckState.cs` | `State`, `PlayerResponse`, `Timer`, `DodgeWarning` |
| `MucJwt` | `MucJwt.cs` | `ChannelClaim`, `Domain`, `Jwt`, `TargetRegion` |
| `Summoner` | `LeagueClientLobbyModels.cs` | `SummonerId`, `DisplayName`, `Puuid`, `SummonerLevel` |
| `GameFlowSession` | `LeagueClientLobbyModels.cs` | `Phase`, `GameData`, `Map`, `GameClient` |
| `SearchState` | `LeagueClientLobbyModels.cs` | `State`, `TimeInQueue`, `EstimatedQueueTime`, `ReadyCheck` |

#### Game Client API Models (used by `GameClientApiReader`)

| Model | File | Key Properties |
|-------|------|---------------|
| `AllGameData` | `LiveClientDataModels.cs` | `ActivePlayer`, `AllPlayers[]`, `Events`, `GameData` |
| `ActivePlayer` | `LiveClientDataModels.cs` | `Abilities`, `ChampionStats`, `CurrentGold`, `Level`, `SummonerName`, `FullRunes` |
| `Player` | `LiveClientDataModels.cs` | `ChampionName`, `Team`, `IsDead`, `Level`, `Items[]`, `Scores`, `SummonerSpells` |
| `ChampionStats` | `LiveClientDataModels.cs` | `AttackDamage`, `AbilityPower`, `Armor`, `MaxHealth`, `MoveSpeed`, … (30+ stats) |
| `Abilities` | `LiveClientDataModels.cs` | `Passive`, `Q`, `W`, `E`, `R` (each an `Ability`) |
| `Item` | `LiveClientDataModels.cs` | `ItemId`, `DisplayName`, `Count`, `Price`, `Slot` |
| `Scores` | `LiveClientDataModels.cs` | `Kills`, `Deaths`, `Assists`, `CreepScore`, `WardScore` |
| `Event` / `GameEvent` | `LiveClientDataModels.cs` | `EventName`, `EventTime`, `KillerName`, `VictimName`, `DragonType`, … |
| `GameData` | `LiveClientDataModels.cs` | `GameMode`, `GameTime`, `MapName`, `MapNumber`, `MapTerrain` |
| `FullRunes` | `LiveClientDataModels.cs` | `Keystone`, `GeneralRunes[]`, `PrimaryRuneTree`, `SecondaryRuneTree` |
| `SummonerSpells` | `LiveClientDataModels.cs` | `SummonerSpellOne`, `SummonerSpellTwo` |
| `LiveEvent` | `LiveEvent.cs` | `EventID`, `EventName`, `EventTime`, `KillerName`, … |
| `LiveWrapper` | `LiveWrapper.cs` | `Events[]` (list of `LiveEvent`) |

### 3.6 JSON Source Generation (`LeagueJsonContext`)

```csharp
[JsonSerializable(typeof(AllGameData))]
[JsonSerializable(typeof(Lobby))]
// … 15+ types registered
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class LeagueJsonContext : JsonSerializerContext { }
```

Enables AOT-compatible, trimmer-safe, and faster JSON serialization. Both `LeagueClientApiReader` and `GameClientApiReader` use `LeagueJsonContext.Default` as their `TypeInfoResolver`.

---

## 4. League Client API vs. Game Client API — Side-by-Side Comparison

| Aspect | League Client API | Game Client API |
|--------|-------------------|-----------------|
| **When available** | League Client is open (lobby, menu, champ select) | Active game is running |
| **Port** | Dynamic — read from lockfile | Fixed: `2999` |
| **Authentication** | Basic Auth (`riot:<password_from_lockfile>`) | None |
| **Discovery** | Find process → read lockfile → extract port+token | Just connect to `https://127.0.0.1:2999` |
| **Data scope** | Lobby state, matchmaking, champion select, summoner info | Real-time game state: players, items, events, stats |
| **Namespace** | `BE.League.Desktop.LeagueClientApi` | `BE.League.Desktop.GameClientApi` |
| **Transport class** | `LeagueClientApiClient` | `GameClientApiClient` |
| **Reader class** | `LeagueClientApiReader` | `GameClientApiReader` |
| **Event loop** | `LeagueClientApiEventLoop` (polls every 500 ms) | None (poll manually) |
| **Key models** | `Lobby`, `ChampSelectSession`, `ReadyCheck` | `AllGameData`, `ActivePlayer`, `Player`, `Scores` |
| **Official docs** | https://developer.riotgames.com/docs/lol#league-client-api | https://developer.riotgames.com/docs/lol#game-client-api |

### When to Use Which

```
┌─────────────────────────────────────────────────────────────────┐
│  League Client Lifecycle                                        │
│                                                                 │
│  Login → [Menu] → [Lobby] → [Queue] → [ChampSelect] → [Game]  │
│           ▲         ▲         ▲           ▲              ▲      │
│           └─────────┴─────────┴───────────┘              │      │
│               League Client API available                 │      │
│                                          Game Client API        │
└─────────────────────────────────────────────────────────────────┘
```

- **Use `LeagueClient`** when you need: lobby members, queue status, champion select picks/bans, ready check accept/decline, summoner info.
- **Use `GameClient`** when you need: real-time game stats, player items/scores/KDA, game events, champion abilities.

---

## 5. Example Usages

### 5.1 Minimal — Check if League Client is Running

```csharp
using BE.League.Desktop.Connection;

bool running = LeagueClientConnectionInfo.IsLeagueClientRunning();
Console.WriteLine(running ? "League Client is running" : "Not running");
```

### 5.2 Read Lobby State (League Client API)

```csharp
using BE.League.Desktop;

var client = new LeagueDesktopClient();
var lobby = await client.LeagueClient.GetLobbyAsync();

if (lobby != null)
{
    Console.WriteLine($"Mode: {lobby.GameConfig?.GameMode}");
    Console.WriteLine($"Players: {lobby.Members.Length}");
    foreach (var m in lobby.Members)
        Console.WriteLine($"  {m.SummonerName} (Leader={m.IsLeader}, Ready={m.Ready})");
}
else
{
    Console.WriteLine("Not in a lobby.");
}
```

### 5.3 Auto-Accept Ready Check (League Client API)

```csharp
using BE.League.Desktop;

var client = new LeagueDesktopClient();

while (true)
{
    var check = await client.LeagueClient.GetReadyCheckAsync();
    if (check is { State: "InProgress" })
    {
        var accepted = await client.LeagueClient.AcceptReadyCheckAsync();
        Console.WriteLine(accepted ? "Accepted!" : "Failed to accept");
    }
    await Task.Delay(1000);
}
```

### 5.4 Monitor Champion Select (League Client API)

```csharp
using BE.League.Desktop;

var client = new LeagueDesktopClient();
var session = await client.LeagueClient.GetChampSelectSessionAsync();

if (session != null)
{
    Console.WriteLine($"Phase: {session.Timer?.Phase}");
    Console.WriteLine($"My cell: {session.LocalPlayerCellId}");
    foreach (var teammate in session.MyTeam)
        Console.WriteLine($"  Cell {teammate.CellId}: ChampId={teammate.ChampionId}");
}
```

### 5.5 Read Live Game Data (Game Client API)

```csharp
using BE.League.Desktop;

var client = new LeagueDesktopClient();
var game = await client.GameClient.GetAllGameDataAsync();

if (game != null)
{
    Console.WriteLine($"Game Time: {game.GameData?.GameTime:F0}s");
    Console.WriteLine($"You: {game.ActivePlayer?.SummonerName} (Lv{game.ActivePlayer?.Level})");

    foreach (var player in game.AllPlayers ?? [])
    {
        var s = player.Scores;
        Console.WriteLine($"  [{player.Team}] {player.ChampionName} — {s?.Kills}/{s?.Deaths}/{s?.Assists}");
    }
}
else
{
    Console.WriteLine("No active game.");
}
```

### 5.6 Get Player Items in Game (Game Client API)

```csharp
using BE.League.Desktop;

var client = new LeagueDesktopClient();
var items = await client.GameClient.GetPlayerItemsAsync("SummonerName");

if (items != null)
{
    foreach (var item in items)
        Console.WriteLine($"  Slot {item.Slot}: {item.DisplayName} x{item.Count}");
}
```

### 5.7 Use the Event Loop (League Client API)

```csharp
using BE.League.Desktop.LeagueClientApi;

var loop = new LeagueClientApiEventLoop();

loop.LobbyChanged += (_, e) =>
{
    if (e.Lobby != null)
        Console.WriteLine($"Lobby changed: {e.Lobby.GameConfig?.GameMode}, {e.Lobby.Members.Length} players");
    else
        Console.WriteLine("Left lobby");
};

loop.ReadyCheckChanged += (_, e) =>
{
    if (e.CanBeClicked)
        Console.WriteLine("Ready check available — click accept!");
};

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, ev) => { ev.Cancel = true; cts.Cancel(); };
await loop.Run(cts.Token);
```

### 5.8 Direct Raw JSON Access

```csharp
using BE.League.Desktop.LeagueClientApi;
using BE.League.Desktop.GameClientApi;

// League Client API — raw JSON
var leagueApi = new LeagueClientApiClient();
string? lobbyJson = await leagueApi.GetLobbyJsonAsync();

// Game Client API — raw JSON
using var gameApi = new GameClientApiClient();
string? gameJson = await gameApi.GetAllGameDataJsonAsync();
```

---

## 6. Testing Strategy

### Unit Tests (`BE.League.Desktop.Tests`)

- Use **FakeItEasy** to mock `ILeagueClientApi` / `IGameClientApi`.
- **ModelTests/**: Feed known JSON strings through the deserializer and assert property values.
- **LeagueClientApi/GivenLeagueClientApiEventLoop.cs**: Verifies that the polling loop fires `LobbyChanged` / `ReadyCheckChanged` events correctly.
- No network access required.

### Integration Tests (`BE.League.Desktop.IntegrationTests`)

- **Prerequisite**: League of Legends must be running on the test machine.
- `GivenLeagueDesktopClient` — verifies connection can be established.
- `WhenInLeagueClient` — queries League Client API endpoints (lobby, champ select, ready check). Skips gracefully if client is not running.
- `WhenInActiveGame` — queries Game Client API endpoints (player name, game stats, player list). Skips gracefully if no game is active.

---

## 7. Key Design Decisions

| Decision | Rationale |
|----------|-----------|
| **Two-layer API (raw JSON + typed reader)** | Allows consumers to use raw JSON for custom parsing, or typed models for convenience. Testable via interface mocking. |
| **Separate namespaces for LCU vs. LiveClient** | Clear separation of concerns — the two APIs have different auth, ports, and availability windows. |
| **`LeagueDesktopClient` as unified facade** | Single entry point for consumers who need both APIs. |
| **Source-generated JSON** (`LeagueJsonContext`) | AOT/trimming support + performance. The AutoAccept example publishes as Native AOT. |
| **`[JsonExtensionData]` on most models** | Forward compatibility — Riot can add fields without breaking deserialization. |
| **Lockfile-based discovery** | More reliable than command-line argument parsing. Works with standard and custom installs. |
| **Graceful null returns** | All API methods return `null` on failure instead of throwing, simplifying polling loops. |

---

## 8. Tech Stack

| Component | Version / Package |
|-----------|------------------|
| Target Framework | `.NET 10.0` |
| C# Language Version | `13.0` |
| JSON | `System.Text.Json` (built-in) + source generation |
| Process Discovery | `System.Management` NuGet (`10.0.0`) |
| Test Framework | xUnit `2.9.3` |
| Mocking | FakeItEasy `8.3.0` |
| Console UI (examples) | Spectre.Console `0.54.0` |
| Razor Console (examples) | RazorConsole.Core `0.1.1` |

---

## 9. How to Extend

### Add a new League Client API endpoint

1. Add method signature to `ILeagueClientApi.cs`.
2. Implement in `LeagueClientApiClient.cs` (call `GetJsonAsync` or `PostAsync`).
3. Add model class in `Models/` if needed.
4. Register model in `LeagueJsonContext.cs`.
5. Add typed method in `LeagueClientApiReader.cs`.
6. Add unit test in `BE.League.Desktop.Tests/`.

### Add a new Game Client API endpoint

1. Add method signature to `IGameClientApi.cs`.
2. Implement in `GameClientApiClient.cs`.
3. Add/reuse model class in `Models/`.
4. Register model in `LeagueJsonContext.cs`.
5. Add typed method in `GameClientApiReader.cs`.
6. Add unit test in `BE.League.Desktop.Tests/`.

### Discover new endpoints

- Use [LCU Explorer](https://github.com/HextechDocs/lcu-explorer) or [Rift Explorer](https://github.com/Pupix/rift-explorer) while the client is running.
- Use Fiddler/Charles Proxy to inspect traffic.
- Check [HexTech Docs](https://hextechdocs.dev/) for community-maintained League Client API docs.
- Official: [Riot Developer Portal — League of Legends APIs](https://developer.riotgames.com/docs/lol).

