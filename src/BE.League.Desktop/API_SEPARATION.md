# API Separation Architecture

## Overview

The League Desktop Client integration has been separated into two distinct API clients:

```
BE.League.Desktop/
├── LeagueDesktopClient.cs         # Unified wrapper (implements ILeagueDesktopClient)
├── LiveClient/
│   └── LiveClientApi.cs           # Port 2999 - In-Game API
└── LcuClient/
    └── LcuApi.cs                  # Dynamic Port - Client/Lobby API
```

## LiveClientApi (In-Game)

**Purpose**: Real-time game data during active matches  
**Port**: `2999` (fixed)  
**Authentication**: None required  
**Availability**: Only when a game is running

### Usage

```csharp
// Direct usage
using var liveApi = new LiveClientApi();
var playerName = await liveApi.GetActivePlayerNameJsonAsync();
var gameStats = await liveApi.GetGameStatsJsonAsync();

// Via unified client
using var client = new LeagueDesktopClient();
var allData = await client.Live.GetAllGameDataJsonAsync();
```

### Available Methods

- `GetAllGameDataJsonAsync()` - Complete game state
- `GetActivePlayerJsonAsync()` - Current player data
- `GetActivePlayerNameJsonAsync()` - Player name only
- `GetActivePlayerAbilitiesJsonAsync()` - Player abilities
- `GetActivePlayerRunesJsonAsync()` - Player runes
- `GetPlayerListJsonAsync()` - All players in game
- `GetPlayerScoresJsonAsync(summonerName)` - KDA and CS
- `GetPlayerSummonerSpellsJsonAsync(summonerName)` - Summoner spells
- `GetPlayerMainRunesJsonAsync(summonerName)` - Player runes
- `GetPlayerItemsJsonAsync(summonerName)` - Player items
- `GetEventDataJsonAsync()` - Game events (kills, objectives)
- `GetGameStatsJsonAsync()` - Game metadata (time, mode, map)

## LcuApi (Client/Lobby)

**Purpose**: Client state, lobby management, matchmaking  
**Port**: Dynamic (from lockfile)  
**Authentication**: Basic Auth (username: `riot`, password from lockfile)  
**Availability**: When League Client is running

### Usage

```csharp
// Direct usage
var connection = LeagueClientConnectionInfo.GetFromRunningClient();
using var lcuApi = new LcuApi(connection);
var lobby = await lcuApi.GetLobbyJsonAsync();

// Via unified client
using var client = new LeagueDesktopClient();
if (client.Lcu != null)
{
    var champSelect = await client.Lcu.GetChampSelectSessionJsonAsync();
    await client.Lcu.AcceptReadyCheckAsync();
}
```

### Available Methods

- `GetLobbyJsonAsync()` - Current lobby state
- `GetChampSelectSessionJsonAsync()` - Champion select data
- `GetReadyCheckJsonAsync()` - Ready check state
- `AcceptReadyCheckAsync()` - Accept ready check
- `DeclineReadyCheckAsync()` - Decline ready check

## LeagueDesktopClient (Unified)

The unified client provides access to both APIs through a single instance.

### Usage

```csharp
using var client = new LeagueDesktopClient();

// Access Live Client API
var gameData = await client.Live.GetAllGameDataJsonAsync();

// Access LCU API (null if client not running)
if (client.Lcu != null)
{
    var lobby = await client.Lcu.GetLobbyJsonAsync();
}

// Or use ILeagueDesktopClient interface (backward compatible)
var playerName = await client.GetActivePlayerNameJsonAsync();
var lobbyData = await client.GetLobbyJsonAsync();
```

### Properties

- `Live` - LiveClientApi instance (always available)
- `Lcu` - LcuApi instance (null if League Client not running)

### Backward Compatibility

The `LeagueDesktopClient` still implements `ILeagueDesktopClient` and delegates all methods to the appropriate API client. This maintains backward compatibility with existing code.

## Configuration

### LeagueDesktopOptions

```csharp
var options = new LeagueDesktopOptions
{
    Connection = LeagueClientConnectionInfo.GetFromRunningClient(),
    LiveClientBaseUrl = "https://127.0.0.1:2999", // Optional override
    Timeout = TimeSpan.FromSeconds(10)
};

var client = new LeagueDesktopClient(options);
```

## Benefits of Separation

✅ **Separation of Concerns** - Each API has its own responsibility  
✅ **Independent Usage** - Use only the API you need  
✅ **Clearer Code** - `client.Live.*` vs `client.Lcu.*` makes API origin explicit  
✅ **Better Testing** - APIs can be tested independently  
✅ **Maintainability** - Changes to one API don't affect the other  
✅ **Flexibility** - Can initialize only the API you need

## Migration Guide

### Before (Old Structure)

```csharp
var client = new LeagueDesktopClient();
var gameData = await client.GetAllGameDataJsonAsync();
var lobby = await client.GetLobbyJsonAsync();
```

### After (New Structure - Option 1: Direct Properties)

```csharp
var client = new LeagueDesktopClient();
var gameData = await client.Live.GetAllGameDataJsonAsync();
var lobby = await client.Lcu?.GetLobbyJsonAsync();
```

### After (New Structure - Option 2: Backward Compatible)

```csharp
// No changes needed - still works!
var client = new LeagueDesktopClient();
var gameData = await client.GetAllGameDataJsonAsync();
var lobby = await client.GetLobbyJsonAsync();
```

### After (New Structure - Option 3: Separate Clients)

```csharp
// Use only what you need
using var liveClient = new LiveClientApi();
var gameData = await liveClient.GetAllGameDataJsonAsync();

// Separate instance for LCU
var connection = LeagueClientConnectionInfo.GetFromRunningClient();
using var lcuClient = new LcuApi(connection);
var lobby = await lcuClient.GetLobbyJsonAsync();
```

## Error Handling

### LiveClientApi

Returns `null` when:
- Game is not running
- API endpoint returns error
- Timeout occurs

```csharp
var playerName = await client.Live.GetActivePlayerNameJsonAsync();
if (playerName == null)
{
    // Game not running or API not available
}
```

### LcuApi

Returns `null` when:
- Endpoint not available in current state (e.g., not in lobby)
- API call fails
- Timeout occurs

Throws `InvalidOperationException` during construction if:
- League Client is not running
- Lockfile not found

```csharp
try
{
    var lcuApi = new LcuApi(); // May throw
    var lobby = await lcuApi.GetLobbyJsonAsync(); // May return null
}
catch (InvalidOperationException)
{
    // League Client not running
}
```

### Unified Client

The unified client handles LCU initialization gracefully:

```csharp
var client = new LeagueDesktopClient();

// Lcu property is null if client not running
if (client.Lcu != null)
{
    var lobby = await client.Lcu.GetLobbyJsonAsync();
}
else
{
    // League Client not running
}
```

## Testing

### Unit Tests

Both APIs can be tested independently:

```csharp
// Test LiveClientApi
var liveApi = new LiveClientApi("https://test-server:2999");
// ... test methods

// Test LcuApi
var fakeConnection = new LeagueClientConnectionInfo { ... };
var lcuApi = new LcuApi(fakeConnection);
// ... test methods
```

### Integration Tests

See `BE.League.Desktop.IntegrationTests` for examples.

## Implementation Details

### LiveClientApi
- **File**: `LiveClient/LiveClientApi.cs`
- **Dependencies**: None (just HttpClient)
- **Disposal**: Disposes HttpClient

### LcuApi
- **File**: `LcuClient/LcuApi.cs`
- **Dependencies**: LeagueClientConnectionInfo
- **Disposal**: Disposes HttpClient

### LeagueDesktopClient
- **File**: `LeagueDesktopClient.cs`
- **Dependencies**: Both LiveClientApi and LcuApi
- **Disposal**: Disposes both clients

---

**Version**: 2.0  
**Last Updated**: 2025-10-31

