# API Architecture

## Overview

The League Desktop Client integration is built around the two official local APIs exposed by
League of Legends. Each API has its own dedicated client stack.

```
BE.League.Desktop/
├── LeagueDesktopClient.cs          # Unified facade (implements ILeagueDesktopClient)
├── LeagueClientApi/                # League Client API — desktop client/lobby data
│   ├── ILeagueClientApi.cs         #   Raw JSON interface
│   ├── LeagueClientApiClient.cs    #   HttpClient implementation (Basic Auth, dynamic port)
│   ├── LeagueClientApiReader.cs    #   Deserializer → typed models
│   └── LeagueClientApiEventLoop.cs #   Polling loop with LobbyChanged/ReadyCheckChanged events
└── GameClientApi/                  # Game Client API — real-time in-game data
    ├── IGameClientApi.cs           #   Raw JSON interface
    ├── GameClientApiClient.cs      #   HttpClient implementation (no auth, port 2999)
    └── GameClientApiReader.cs      #   Deserializer → typed models
```

---

## GameClientApiClient (In-Game)

**Official docs**: https://developer.riotgames.com/docs/lol#game-client-api
**OpenAPI spec**: https://static.developer.riotgames.com/docs/lol/liveclientdata_sample.json
**Purpose**: Real-time game data during active matches
**Port**: `2999` (fixed)
**Authentication**: None required
**Availability**: Only when a game is running

### Usage

```csharp
// Direct usage
using var gameApi = new GameClientApiClient();
var playerName = await gameApi.GetActivePlayerNameJsonAsync();

// Via typed reader
var reader = new GameClientApiReader();
var allData = await reader.GetAllGameDataAsync();

// Via unified facade
using var client = new LeagueDesktopClient();
var allData = await client.GameClient.GetAllGameDataAsync();
```

### Available Methods on `GameClientApiReader`

| Method | Endpoint | Returns |
|--------|----------|---------|
| `GetAllGameDataAsync()` | `GET /liveclientdata/allgamedata` | `AllGameData` |
| `GetActivePlayerAsync()` | `GET /liveclientdata/activeplayer` | `ActivePlayer` |
| `GetActivePlayerNameAsync()` | `GET /liveclientdata/activeplayername` | `string` |
| `GetActivePlayerAbilitiesAsync()` | `GET /liveclientdata/activeplayerabilities` | `Abilities` |
| `GetActivePlayerRunesAsync()` | `GET /liveclientdata/activeplayerrunes` | `FullRunes` |
| `GetPlayerListAsync()` | `GET /liveclientdata/playerlist` | `List<Player>` |
| `GetPlayerScoresAsync(name)` | `GET /liveclientdata/playerscores?summonerName=…` | `Scores` |
| `GetPlayerSummonerSpellsAsync(name)` | `GET /liveclientdata/playersummonerspells?summonerName=…` | `SummonerSpells` |
| `GetPlayerMainRunesAsync(name)` | `GET /liveclientdata/playermainrunes?summonerName=…` | `PlayerRunes` |
| `GetPlayerItemsAsync(name)` | `GET /liveclientdata/playeritems?summonerName=…` | `List<Item>` |
| `GetEventDataAsync()` | `GET /liveclientdata/eventdata` | `Event` |
| `GetGameStatsAsync()` | `GET /liveclientdata/gamestats` | `GameData` |

---

## LeagueClientApiClient (Desktop Client / Lobby)

**Official docs**: https://developer.riotgames.com/docs/lol#league-client-api
**Community reference**: https://hextechdocs.dev/
**Purpose**: Client state, lobby management, matchmaking
**Port**: Dynamic (from lockfile)
**Authentication**: `Authorization: Basic riot:<token>` (token from lockfile)
**Availability**: While the League Client process is running

### Usage

```csharp
// Direct usage
var connection = LeagueClientConnectionInfo.GetFromRunningClient();
using var leagueApi = new LeagueClientApiClient(connection);
var lobbyJson = await leagueApi.GetLobbyJsonAsync();

// Via typed reader
var reader = new LeagueClientApiReader();
var lobby = await reader.GetLobbyAsync();

// Via unified facade
using var client = new LeagueDesktopClient();
var session = await client.LeagueClient.GetChampSelectSessionAsync();
await client.LeagueClient.AcceptReadyCheckAsync();
```

### Available Methods on `LeagueClientApiReader`

| Method | Endpoint | Returns |
|--------|----------|---------|
| `GetLobbyAsync()` | `GET /lol-lobby/v2/lobby` | `Lobby` |
| `GetChampSelectSessionAsync()` | `GET /lol-champ-select/v1/session` | `ChampSelectSession` |
| `GetReadyCheckAsync()` | `GET /lol-matchmaking/v1/ready-check` | `ReadyCheck` |
| `AcceptReadyCheckAsync()` | `POST /lol-matchmaking/v1/ready-check/accept` | `bool` |
| `DeclineReadyCheckAsync()` | `POST /lol-matchmaking/v1/ready-check/decline` | `bool` |

---

## LeagueDesktopClient (Unified Facade)

```csharp
using var client = new LeagueDesktopClient();

// Game Client API (in-game, port 2999)
var gameData = await client.GameClient.GetAllGameDataAsync();

// League Client API (lobby/client data, dynamic port)
var lobby = await client.LeagueClient.GetLobbyAsync();
```

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `GameClient` | `GameClientApiReader` | Game Client API — real-time in-game data |
| `LeagueClient` | `LeagueClientApiReader` | League Client API — lobby, matchmaking, champ select |

---

## Side-by-Side Comparison

| Aspect | League Client API | Game Client API |
|--------|-------------------|-----------------|
| **When available** | League Client is running | Active game is running |
| **Port** | Dynamic (from lockfile) | Fixed: `2999` |
| **Authentication** | Basic Auth `riot:<token>` | None |
| **Namespace** | `BE.League.Desktop.LeagueClientApi` | `BE.League.Desktop.GameClientApi` |
| **Transport class** | `LeagueClientApiClient` | `GameClientApiClient` |
| **Reader class** | `LeagueClientApiReader` | `GameClientApiReader` |
| **Event loop** | `LeagueClientApiEventLoop` (500 ms polling) | _(poll manually)_ |
| **Key models** | `Lobby`, `ChampSelectSession`, `ReadyCheck` | `AllGameData`, `ActivePlayer`, `Player`, `Scores` |

---

## Configuration

```csharp
var options = new LeagueDesktopOptions
{
    Connection = LeagueClientConnectionInfo.GetFromRunningClient(),
    GameClientBaseUrl = "https://127.0.0.1:2999", // optional override
    Timeout = TimeSpan.FromSeconds(10)
};
var client = new LeagueDesktopClient(options);
```

---

## Event Loop (League Client API)

```csharp
using BE.League.Desktop.LeagueClientApi;

var loop = new LeagueClientApiEventLoop();

loop.LobbyChanged += (_, e) =>
{
    if (e.Lobby != null)
        Console.WriteLine($"Lobby: {e.Lobby.GameConfig?.GameMode}, {e.Lobby.Members.Length} players");
    else
        Console.WriteLine("Left lobby");
};

loop.ReadyCheckChanged += (_, e) =>
{
    if (e.CanBeClicked)
        Console.WriteLine("Ready check available!");
};

using var cts = new CancellationTokenSource();
await loop.Run(cts.Token);
```

---

## How to Extend

### Add a new League Client API endpoint

1. Add method signature to `ILeagueClientApi.cs`
2. Implement in `LeagueClientApiClient.cs` (call `GetJsonAsync` or `PostAsync`)
3. Add model in `Models/` if needed; register in `LeagueJsonContext.cs`
4. Add typed method in `LeagueClientApiReader.cs`
5. Add unit test in `BE.League.Desktop.Tests/`

### Add a new Game Client API endpoint

1. Add method signature to `IGameClientApi.cs`
2. Implement in `GameClientApiClient.cs`
3. Add/reuse model in `Models/`; register in `LeagueJsonContext.cs`
4. Add typed method in `GameClientApiReader.cs`
5. Add unit test in `BE.League.Desktop.Tests/`

---

**Version**: 3.0
**Last Updated**: 2026-05-25
