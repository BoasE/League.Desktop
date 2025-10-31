# League of Legends Desktop Client Integration

## Overview

This document describes how to integrate with the League of Legends Desktop Client at the file, process, and API level. It serves as a comprehensive reference for understanding the integration points and for providing context to LLMs when researching additional functionality.

**Last Updated**: 2025-10-31  
**Target Platform**: Windows  
**League Client Version**: Modern League Client (2016+)

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Process & File System Integration](#process--file-system-integration)
3. [API Endpoints](#api-endpoints)
4. [Authentication & Security](#authentication--security)
5. [Connection Discovery](#connection-discovery)
6. [Data Models](#data-models)
7. [Best Practices](#best-practices)
8. [Troubleshooting](#troubleshooting)
9. [Research Resources](#research-resources)

---

## Architecture Overview

League of Legends provides **two separate APIs** for integration:

### 1. Live Client Data API (In-Game)
- **Port**: `2999` (fixed)
- **Protocol**: HTTP/HTTPS
- **Authentication**: None required
- **Availability**: Only when a game is actively running
- **Purpose**: Real-time game data during an active match
- **Base URL**: `https://127.0.0.1:2999`

### 2. League Client Update (LCU) API (Client)
- **Port**: Dynamic (assigned at client startup)
- **Protocol**: HTTPS
- **Authentication**: Basic Auth required (username + password)
- **Availability**: When League Client is running (lobby, menu, etc.)
- **Purpose**: Client state, lobby management, champion select, etc.
- **Base URL**: `https://127.0.0.1:{dynamic-port}`

```
┌─────────────────────────────────────────────────────┐
│          League of Legends Client Process          │
│                 (LeagueClient.exe)                  │
├─────────────────────────────────────────────────────┤
│                                                     │
│  ┌───────────────────┐    ┌──────────────────────┐ │
│  │   LCU API         │    │  Live Client Data    │ │
│  │   (Dynamic Port)  │    │  API (Port 2999)     │ │
│  │   HTTPS + Auth    │    │  HTTPS (No Auth)     │ │
│  └───────────────────┘    └──────────────────────┘ │
│           │                         │               │
└───────────┼─────────────────────────┼───────────────┘
            │                         │
            │                         │
            ▼                         ▼
    ┌───────────────┐         ┌──────────────┐
    │ Lobby/Client  │         │  Live Game   │
    │    State      │         │     Data     │
    └───────────────┘         └──────────────┘
```

---

## Process & File System Integration

### Process Discovery

The League Client runs as one of these processes:
- `LeagueClientUx.exe` (modern client)
- `LeagueClient.exe` (fallback)

**Location**: Typically installed at:
- `C:\Riot Games\League of Legends\`
- `D:\Riot Games\League of Legends\` (alternative drive)

### The Lockfile

The **lockfile** is the key to LCU API integration. It contains connection credentials.

**File Path**: `{League Install Directory}/lockfile`

**Example**: `C:\Riot Games\League of Legends\lockfile`

#### Lockfile Format

The lockfile is a colon-separated text file with 5 fields:

```
{process-name}:{process-id}:{port}:{password}:{protocol}
```

**Example Content**:
```
LeagueClientUx:12345:54321:abcdefghijklmnopqrstuvwxyz:https
```

**Fields**:
1. **Process Name**: `LeagueClientUx` or `LeagueClient`
2. **Process ID**: PID of the running client process
3. **Port**: Dynamic port number for LCU API (e.g., `54321`)
4. **Password**: Authentication token (base64-encoded)
5. **Protocol**: `https` (always HTTPS)

#### Lockfile Characteristics

- **Created**: When League Client starts
- **Deleted**: When League Client closes
- **Permissions**: Readable while client is running
- **File Sharing**: Must be opened with `FileShare.ReadWrite` flag
- **Dynamic**: Port and password change on each client restart

### Discovery Algorithm

Our implementation uses this strategy:

1. **Process-based discovery** (primary):
   - Find `LeagueClientUx` or `LeagueClient` process
   - Get the process's executable path
   - Look for `lockfile` in the executable directory
   - If not found, check parent directory

2. **Known locations** (fallback):
   - Check `C:\Riot Games\League of Legends\lockfile`
   - Check `D:\Riot Games\League of Legends\lockfile`

3. **Environment variable** (override):
   - Check `LEAGUE_LOCKFILE` environment variable
   - Useful for custom installations

**Implementation**:
```csharp
// See: BE.League.Desktop/Connection/ClientHelper.cs
public static string? ResolveLockfilePath(string? overridePath = null)
{
    // 1. Try override path
    if (!string.IsNullOrWhiteSpace(overridePath) && File.Exists(overridePath))
        return overridePath;

    // 2. Find via process
    var candidates = new[] { "LeagueClientUx", "LeagueClient" };
    foreach (var process in Process.GetProcesses()
                 .Where(p => candidates.Contains(p.ProcessName)))
    {
        var exe = process.MainModule?.FileName;
        var dir = Path.GetDirectoryName(exe);
        var lockfilePath = Path.Combine(dir, "lockfile");
        if (File.Exists(lockfilePath)) return lockfilePath;
    }

    // 3. Try known locations
    var knownPaths = new[] {
        @"C:\Riot Games\League of Legends\lockfile",
        @"D:\Riot Games\League of Legends\lockfile"
    };
    
    // 4. Check environment variable
    var env = Environment.GetEnvironmentVariable("LEAGUE_LOCKFILE");
    if (!string.IsNullOrWhiteSpace(env) && File.Exists(env))
        return env;
    
    return null;
}
```

### Reading the Lockfile

The lockfile must be read with proper file sharing flags because the League Client keeps it open:

```csharp
using var fs = new FileStream(path, 
    FileMode.Open, 
    FileAccess.Read, 
    FileShare.ReadWrite); // Critical: Allow shared access

using var sr = new StreamReader(fs, Encoding.UTF8);
var content = sr.ReadToEnd().Trim();
var parts = content.Split(':');

// Parse fields
var processName = parts[0];
var processId = int.Parse(parts[1]);
var port = int.Parse(parts[2]);
var password = parts[3];
var protocol = parts[4];
```

---

## API Endpoints

### Live Client Data API (Port 2999)

This API provides real-time game data when a match is active.

**Base URL**: `https://127.0.0.1:2999`

#### Key Endpoints

| Endpoint | Description | Availability |
|----------|-------------|--------------|
| `/liveclientdata/allgamedata` | Complete game state | In active game |
| `/liveclientdata/activeplayer` | Current player info | In active game |
| `/liveclientdata/activeplayername` | Current player name | In active game |
| `/liveclientdata/playerlist` | All players in game | In active game |
| `/liveclientdata/gamestats` | Game metadata (time, mode, map) | In active game |
| `/liveclientdata/eventdata` | Game events (kills, objectives) | In active game |
| `/liveclientdata/playerscores?summonerName={name}` | KDA, CS for player | In active game |
| `/liveclientdata/playersummonerspells?summonerName={name}` | Summoner spells | In active game |
| `/liveclientdata/playeritems?summonerName={name}` | Items for player | In active game |
| `/liveclientdata/activeplayerrunes` | Active player runes | In active game |
| `/liveclientdata/activeplayerabilities` | Active player abilities | In active game |

**Authentication**: None required

**Example Request**:
```http
GET https://127.0.0.1:2999/liveclientdata/activeplayer
```

**Example Response**:
```json
{
  "abilities": { ... },
  "championStats": {
    "abilityPower": 0,
    "armor": 33,
    "attackDamage": 64,
    "attackSpeed": 0.625,
    "currentHealth": 580,
    "maxHealth": 580
  },
  "currentGold": 500,
  "level": 1,
  "summonerName": "PlayerName"
}
```

### League Client Update (LCU) API

This API provides access to client state, lobby, matchmaking, and champion select.

**Base URL**: `https://127.0.0.1:{port}` (port from lockfile)

#### Key Endpoints

| Endpoint | Method | Description | Availability |
|----------|--------|-------------|--------------|
| `/lol-summoner/v1/current-summoner` | GET | Current summoner info | Always |
| `/lol-lobby/v2/lobby` | GET | Current lobby state | In lobby |
| `/lol-champ-select/v1/session` | GET | Champion select session | In champ select |
| `/lol-matchmaking/v1/ready-check` | GET | Ready check state | When found |
| `/lol-matchmaking/v1/ready-check/accept` | POST | Accept ready check | When found |
| `/lol-matchmaking/v1/ready-check/decline` | POST | Decline ready check | When found |
| `/lol-gameflow/v1/session` | GET | Game flow state | Always |

**Authentication**: Basic Auth with username `riot` and password from lockfile

**Example Request**:
```http
GET https://127.0.0.1:54321/lol-lobby/v2/lobby
Authorization: Basic cmlvdDphYmNkZWZnaGlqa2xtbm9wcXJzdHV2d3h5eg==
```

**Example Response**:
```json
{
  "canStartActivity": true,
  "gameConfig": {
    "gameMode": "CLASSIC",
    "mapId": 11,
    "queueId": 420
  },
  "members": [
    {
      "summonerId": 12345,
      "summonerName": "PlayerName",
      "isLeader": true,
      "ready": true
    }
  ],
  "partyId": "party-uuid",
  "partyType": "open"
}
```

---

## Authentication & Security

### Live Client Data API
- **No authentication required**
- Self-signed SSL certificate (must be accepted)
- Only accessible from localhost (`127.0.0.1`)

### LCU API
- **Basic Authentication required**
- Username: `riot` (always)
- Password: From lockfile (changes per session)
- Self-signed SSL certificate (must be accepted)
- Only accessible from localhost (`127.0.0.1`)

### SSL Certificate Handling

Both APIs use self-signed certificates. Your HTTP client must be configured to accept them:

```csharp
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = 
        (sender, certificate, chain, sslPolicyErrors) => true
};

var client = new HttpClient(handler)
{
    BaseAddress = new Uri(baseUrl)
};
```

### Basic Auth Implementation

For LCU API:

```csharp
var username = "riot";
var password = passwordFromLockfile; // e.g., "abcdefghijklmnopqrstuvwxyz"

var credentials = Convert.ToBase64String(
    Encoding.ASCII.GetBytes($"{username}:{password}")
);

client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Basic", credentials);
```

---

## Connection Discovery

### Complete Connection Flow

1. **Find the lockfile**
   ```csharp
   var lockfilePath = ClientHelper.ResolveLockfilePath();
   if (lockfilePath == null) 
   {
       // League Client is not running
       return null;
   }
   ```

2. **Read lockfile contents**
   ```csharp
   var lockInfo = ClientHelper.ReadLockfile(lockfilePath);
   // lockInfo.Port = 54321
   // lockInfo.Password = "abcdefg..."
   // lockInfo.Protocol = "https"
   ```

3. **Build connection info**
   ```csharp
   var connection = new LeagueClientConnectionInfo
   {
       Port = lockInfo.Port.ToString(),
       Token = lockInfo.Password,
       Protocol = lockInfo.Protocol
   };
   
   var baseUrl = connection.GetBaseUrl();
   // Returns: "https://127.0.0.1:54321"
   ```

4. **Create authenticated HTTP client**
   ```csharp
   var handler = new HttpClientHandler
   {
       ServerCertificateCustomValidationCallback = (_, _, _, _) => true
   };
   
   var client = new HttpClient(handler)
   {
       BaseAddress = new Uri(baseUrl)
   };
   
   var credentials = Convert.ToBase64String(
       Encoding.ASCII.GetBytes($"riot:{connection.Token}")
   );
   
   client.DefaultRequestHeaders.Authorization = 
       new AuthenticationHeaderValue("Basic", credentials);
   ```

5. **Make API calls**
   ```csharp
   var response = await client.GetStringAsync("/lol-lobby/v2/lobby");
   ```

### Error Handling

Common scenarios to handle:

- **Lockfile not found**: League Client not running
- **Connection refused**: Client just started/stopped
- **404 Not Found**: Endpoint not available in current state
- **401 Unauthorized**: Wrong credentials (shouldn't happen with lockfile)
- **Timeout**: Client not responding

---

## Data Models

### Core Model Categories

Our implementation uses strongly-typed models:

#### 1. Live Game Data Models
Located in: `BE.League.Desktop/Models/LiveClientDataModels.cs`

**Key Classes**:
- `AllGameData` - Complete game state
- `ActivePlayer` - Current player (you)
- `Player` - Any player in the game
- `GameData` - Game metadata (mode, time, map)
- `Event` / `GameEvent` - Game events (kills, objectives)
- `ChampionStats` - Stats (HP, AD, AP, armor, etc.)
- `Abilities` - Champion abilities (Q, W, E, R, Passive)
- `FullRunes` - Complete rune setup
- `Item` - Item in inventory
- `Scores` - KDA and CS
- `SummonerSpells` - Summoner spells (Flash, Ignite, etc.)

#### 2. Client/Lobby Models
Located in: `BE.League.Desktop/Models/`

**Key Classes**:
- `Lobby` - Lobby state
- `LobbyMember` - Player in lobby
- `LobbyInvitation` - Pending invitations
- `GameConfigDto` - Game configuration
- `ChampSelectSession` - Champion select state
- `ReadyCheckDto` - Ready check state
- `MucJwtDto` - Multi-user chat JWT

**From LeagueClientLobbyModels.cs**:
- `Summoner` - Summoner profile
- `GameFlowSession` - Complete game flow state
- `SearchState` - Matchmaking search state

#### 3. Nested Models

Many models contain nested structures. Example:

```csharp
public class ChampSelectSession
{
    public TimerObj? Timer { get; set; }
    public int LocalPlayerCellId { get; set; }
    public List<List<ActionObj>> Actions { get; set; }
    public List<TeamMember> MyTeam { get; set; }
    
    public class TimerObj 
    { 
        public string? Phase { get; set; } 
    }
    
    public class ActionObj 
    { 
        public int ActorCellId { get; set; }
        public bool IsInProgress { get; set; }
        public string? Type { get; set; }
        public int ChampionId { get; set; }
    }
    
    public class TeamMember 
    {
        public int CellId { get; set; }
        public int ChampionId { get; set; }
        public int? ChampionPickIntent { get; set; }
    }
}
```

### JSON Serialization

All models use `System.Text.Json` with these attributes:

```csharp
[JsonPropertyName("summonerName")]
public string? SummonerName { get; set; }

[JsonExtensionData]
public Dictionary<string, JsonElement>? ExtensionData { get; set; }
```

**JsonExtensionData** captures unknown properties for forward compatibility.

### JSON Context

We use source generation for performance:

```csharp
[JsonSerializable(typeof(AllGameData))]
[JsonSerializable(typeof(ActivePlayer))]
[JsonSerializable(typeof(Lobby))]
// ... more types
public partial class LeagueJsonContext : JsonSerializerContext { }
```

Usage:
```csharp
var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    TypeInfoResolver = LeagueJsonContext.Default
};

var data = JsonSerializer.Deserialize<AllGameData>(json, options);
```

---

## Best Practices

### 1. Connection Management

✅ **DO**:
- Check if lockfile exists before attempting connection
- Handle connection failures gracefully
- Cache connection info (don't re-read lockfile on every request)
- Dispose HttpClient properly
- Use `FileShare.ReadWrite` when reading lockfile

❌ **DON'T**:
- Assume League Client is always running
- Hardcode port numbers (they're dynamic)
- Ignore SSL certificate validation (self-signed certs are expected)
- Poll lockfile continuously (expensive)

### 2. API Usage

✅ **DO**:
- Check API availability before use (404 = not available in current state)
- Use appropriate timeouts (5-10 seconds recommended)
- Handle null responses (many endpoints return null when not applicable)
- Respect API rate limits (though not officially documented)

❌ **DON'T**:
- Poll endpoints more frequently than needed
- Expect real-time updates (you must poll)
- Assume endpoints are always available

### 3. State Management

The client goes through various states:

```
None → Login → Lobby → ChampSelect → InGame → PostGame → Lobby
```

Different APIs are available in different states:

- **Lobby**: `/lol-lobby/v2/lobby`
- **ChampSelect**: `/lol-champ-select/v1/session`
- **InGame**: All Live Client Data API endpoints
- **Always**: `/lol-summoner/v1/current-summoner`

### 4. Error Handling Pattern

```csharp
try
{
    var connection = LeagueClientConnectionInfo.GetFromRunningClient();
    if (connection == null)
    {
        // League not running
        return null;
    }
    
    var client = CreateAuthenticatedClient(connection);
    var response = await client.GetStringAsync(endpoint);
    
    if (string.IsNullOrEmpty(response))
    {
        // No data available in current state
        return null;
    }
    
    return JsonSerializer.Deserialize<T>(response);
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    // Endpoint not available in current state
    return null;
}
catch (JsonException ex)
{
    // Invalid JSON response
    Log.Error("JSON parsing error", ex);
    return null;
}
catch (Exception ex)
{
    // Other errors
    Log.Error("Unexpected error", ex);
    return null;
}
```

### 5. Threading

Both APIs are safe for concurrent access, but:

✅ **DO**:
- Use async/await for all API calls
- Use CancellationToken for long-running operations
- One HttpClient per connection (reuse it)

❌ **DON'T**:
- Create new HttpClient for each request
- Block threads waiting for responses
- Share HttpClient across different connections (different ports/auth)

---

## Troubleshooting

### Common Issues

#### 1. "Lockfile not found"
**Cause**: League Client is not running  
**Solution**: Start League Client, wait for full startup

#### 2. "Connection refused" or timeout
**Causes**:
- Client is starting up (wait a few seconds)
- Client just closed (lockfile may still exist briefly)
- Firewall blocking localhost connections

**Solution**: 
- Add retry logic with delays
- Check if process is actually running
- Verify Windows Firewall settings

#### 3. "404 Not Found" on API calls
**Cause**: Endpoint not available in current client state  
**Example**: `/lol-lobby/v2/lobby` returns 404 when not in lobby

**Solution**: This is expected behavior, handle gracefully

#### 4. "SSL/TLS error"
**Cause**: Not accepting self-signed certificate  
**Solution**: Use `ServerCertificateCustomValidationCallback`

#### 5. Empty or null responses
**Cause**: Data not available in current game state  
**Example**: `GetActivePlayerNameJsonAsync()` returns null when not in game

**Solution**: This is expected, not an error

### Debugging Tips

1. **Check Process**:
   ```csharp
   var processes = Process.GetProcesses()
       .Where(p => p.ProcessName.Contains("League"))
       .ToList();
   ```

2. **Verify Lockfile**:
   ```csharp
   var path = ClientHelper.ResolveLockfilePath();
   if (path != null)
   {
       var content = File.ReadAllText(path);
       Console.WriteLine($"Lockfile: {content}");
   }
   ```

3. **Test Connection**:
   ```csharp
   var connection = LeagueClientConnectionInfo.GetFromRunningClient();
   Console.WriteLine($"Port: {connection?.Port}");
   Console.WriteLine($"Token: {connection?.Token}");
   Console.WriteLine($"Valid: {connection?.IsValid}");
   ```

4. **Manual API Test**:
   Use tools like Postman or curl:
   ```bash
   curl -k -u riot:PASSWORD https://127.0.0.1:PORT/lol-summoner/v1/current-summoner
   ```

### Performance Considerations

- **Lockfile Discovery**: ~10-50ms (process enumeration)
- **API Calls**: ~5-50ms per request
- **JSON Deserialization**: ~1-10ms depending on size

**Optimization**:
- Cache connection info (valid for client session)
- Reuse HttpClient instances
- Use source-generated JSON serialization
- Batch related API calls

---

## Research Resources

### Official Documentation

1. **Riot Developer Portal**
   - URL: https://developer.riotgames.com/
   - Docs: https://developer.riotgames.com/docs/lol
   - Live Client Data API: https://developer.riotgames.com/docs/lol#game-client-api_live-client-data-api

### Community Resources

2. **LCU Explorer**
   - Tool to explore LCU API endpoints interactively
   - GitHub: https://github.com/HextechDocs/lcu-explorer
   - Useful for discovering undocumented endpoints

3. **Rift Explorer**
   - Alternative LCU API explorer
   - Better UI, real-time updates
   - GitHub: https://github.com/Pupix/rift-explorer

4. **League Client Update (LCU) Documentation**
   - Community-maintained docs
   - GitHub: https://github.com/CommunityDragon/Docs

5. **HextechDocs**
   - Comprehensive LCU API documentation
   - Website: https://www.hextechdocs.dev/
   - GitHub: https://github.com/HextechDocs

### Example Projects

6. **League Sharp**
   - C# wrapper for League APIs
   - GitHub: https://github.com/bryanhitc/league-sharp

7. **LCU Connector (Python)**
   - Python library for LCU connection
   - GitHub: https://github.com/Her0x27/lcu-connector

### Research with LLMs

When asking LLMs about League integration, provide:

1. **This document** - For understanding the architecture
2. **Specific endpoint** - What you're trying to access
3. **Current state** - What client/game state you're in
4. **Error messages** - Exact error text
5. **Code snippet** - Relevant code attempting the integration

**Example prompt**:
```
I'm integrating with the League of Legends client using the LCU API.

Context:
- Using C# with HttpClient
- Have successfully read lockfile and authenticated
- Client is running, in lobby state

Issue:
- Calling /lol-champ-select/v1/session returns 404
- Expected to get champion select data

Question:
Why does this endpoint return 404 in lobby state?

[Attach: LEAGUE_OF_LEGENDS_INTEGRATION.md]
```

### Endpoint Discovery

To find new endpoints:

1. **Use LCU Explorer**:
   - Start League Client
   - Open LCU Explorer
   - Browse available endpoints by category

2. **Use Fiddler/Charles Proxy**:
   - Configure as HTTPS proxy
   - Add League Client certificate trust
   - Observe actual API calls made by client

3. **Check HextechDocs**:
   - Most complete community documentation
   - Includes request/response examples
   - Categorized by feature

### Data Model Discovery

When encountering unknown JSON structure:

1. **Capture raw JSON** - Log the full response
2. **Use online JSON-to-C# converters** - Generate initial models
3. **Add JsonExtensionData** - Capture unknown fields
4. **Iterate** - Add properties as needed

**Tools**:
- json2csharp.com
- quicktype.io
- Visual Studio's "Paste JSON as Classes"

---

## Implementation Reference

### Our Implementation Structure

```
BE.League.Desktop/
├── Connection/
│   ├── ClientHelper.cs              # Lockfile discovery and reading
│   ├── LeagueClientConnectionInfo.cs # Connection info model
│   └── LeagueDesktopOptions.cs      # Configuration options
├── Models/
│   ├── LiveClientDataModels.cs      # Live game data models
│   ├── LeagueClientLobbyModels.cs   # Client/lobby models
│   ├── ChampSelectSession.cs        # Champion select
│   ├── Lobby.cs                     # Lobby state
│   ├── LobbyMember.cs              # Lobby member
│   └── ... (other models)
├── ILeagueDesktopClient.cs         # Interface definition
├── LeagueDesktopClient.cs          # HTTP client implementation
├── LiveClientObjectReader.cs        # Deserializer wrapper
└── LeagueJsonContext.cs            # JSON source generation

BE.League.Desktop.Tests/
├── LiveClientObjectReaderTests/     # Unit tests for reader
└── ModelTests/                      # Unit tests for models

BE.League.Desktop.IntegrationTests/
├── GivenLeagueDesktopClient.cs     # Basic integration tests
├── WhenInActiveGame.cs             # Live API integration tests
└── WhenInLeagueClient.cs           # LCU API integration tests
```

### Key Classes

**LeagueDesktopClient**: Low-level HTTP client
- Handles authentication
- Makes raw API calls
- Returns JSON strings

**LiveClientObjectReader**: High-level wrapper
- Deserializes JSON to models
- Handles errors gracefully
- Type-safe API

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-10-31 | Initial documentation |

---

## Contributing

When extending this library or documentation:

1. **Test with actual client**: Don't rely on documentation alone
2. **Handle all states**: Test in lobby, in game, and between states
3. **Document discoveries**: Update this file with new findings
4. **Add integration tests**: Verify against real client
5. **Update models**: Keep JSON models in sync with API changes

---

**For questions or additional research needs, provide**:
- This complete document
- Specific use case or problem
- Current client/game state
- Any error messages or unexpected behavior
```

