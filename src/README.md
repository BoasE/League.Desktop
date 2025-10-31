# BE.League.Desktop

A C# library for integrating with the League of Legends Desktop Client, providing access to both the Live Client Data API (in-game) and the League Client Update (LCU) API (client/lobby).

## Quick Start

```csharp
using BE.League.Desktop;

// Create reader
var reader = new LiveClientObjectReader();

// Get in-game data
var gameData = await reader.GetAllGameDataAsync();
if (gameData != null)
{
    Console.WriteLine($"Game time: {gameData.GameData?.GameTime}");
    Console.WriteLine($"Player: {gameData.ActivePlayer?.SummonerName}");
}

// Get lobby data
var lobby = await reader.GetLobbyAsync();
if (lobby != null)
{
    Console.WriteLine($"Players in lobby: {lobby.Members.Length}");
}
```

## Documentation

📖 **[Complete Integration Guide](LEAGUE_OF_LEGENDS_INTEGRATION.md)** - Comprehensive documentation covering:
- Architecture overview
- Process & file system integration
- API endpoints (Live Client Data & LCU)
- Authentication & security
- Connection discovery algorithm
- Data models
- Best practices
- Troubleshooting
- Research resources

## Prerequisites

- **.NET 9.0** or higher
- **League of Legends Client** must be running for integration tests
- **Windows** (League Client is Windows-only)

## Project Structure

```
src/
├── BE.League.Desktop/                    # Main library
│   ├── Connection/                       # Lockfile discovery & connection
│   ├── Models/                          # Data models
│   ├── ILeagueDesktopClient.cs          # Interface
│   ├── LeagueDesktopClient.cs           # HTTP client
│   └── LiveClientObjectReader.cs        # Typed API wrapper
├── BE.League.Desktop.Tests/             # Unit tests
│   ├── LiveClientObjectReaderTests/     # Reader tests
│   └── ModelTests/                      # Model tests
├── BE.League.Desktop.IntegrationTests/  # Integration tests
│   ├── GivenLeagueDesktopClient.cs     # Basic tests
│   ├── WhenInActiveGame.cs             # Live API tests
│   └── WhenInLeagueClient.cs           # LCU API tests
└── Examples/                            # Example applications
    ├── BE.League.Desktop.Console/       # Console examples
    └── BE.League.Desktop.AutoAccept/    # Auto-accept example
```

## Key Features

✅ **Live Client Data API** (Port 2999)
- Real-time game data during active matches
- Player stats, items, abilities
- Game events (kills, objectives)
- No authentication required

✅ **League Client Update API** (Dynamic Port)
- Lobby state and management
- Champion select session
- Ready check accept/decline
- Summoner information
- Basic Auth with lockfile credentials

✅ **Type-Safe Models**
- Strongly-typed C# models
- JSON serialization with System.Text.Json
- Source-generated for performance

✅ **Automatic Connection Discovery**
- Finds League Client process
- Reads lockfile for credentials
- Handles dynamic ports

## Examples

### Get Active Player Name
```csharp
var reader = new LiveClientObjectReader();
var name = await reader.GetActivePlayerNameAsync();
Console.WriteLine($"Playing as: {name}");
```

### Accept Ready Check
```csharp
var reader = new LiveClientObjectReader();
var success = await reader.AcceptReadyCheckAsync();
```

### Get Lobby Members
```csharp
var reader = new LiveClientObjectReader();
var lobby = await reader.GetLobbyAsync();

if (lobby != null)
{
    foreach (var member in lobby.Members)
    {
        Console.WriteLine($"{member.SummonerName} - Ready: {member.Ready}");
    }
}
```

## Testing

### Unit Tests
```bash
cd src/BE.League.Desktop.Tests
dotnet test
```

### Integration Tests
**Prerequisite**: League of Legends must be running

```bash
cd src/BE.League.Desktop.IntegrationTests
dotnet test
```

Integration tests automatically skip if League is not running.

## API Coverage

### Live Client Data API ✅
- `/liveclientdata/allgamedata`
- `/liveclientdata/activeplayer`
- `/liveclientdata/activeplayername`
- `/liveclientdata/playerlist`
- `/liveclientdata/gamestats`
- `/liveclientdata/eventdata`
- `/liveclientdata/playerscores`
- `/liveclientdata/playersummonerspells`
- `/liveclientdata/playeritems`
- `/liveclientdata/activeplayerrunes`
- `/liveclientdata/activeplayerabilities`

### LCU API ✅
- `/lol-lobby/v2/lobby`
- `/lol-champ-select/v1/session`
- `/lol-matchmaking/v1/ready-check`
- `/lol-matchmaking/v1/ready-check/accept`
- `/lol-matchmaking/v1/ready-check/decline`

## Architecture

```
┌─────────────────────────────────────────────────────┐
│          League of Legends Client Process          │
├─────────────────────────────────────────────────────┤
│  ┌───────────────────┐    ┌──────────────────────┐ │
│  │   LCU API         │    │  Live Client Data    │ │
│  │   (Dynamic Port)  │    │  API (Port 2999)     │ │
│  │   HTTPS + Auth    │    │  HTTPS (No Auth)     │ │
│  └───────────────────┘    └──────────────────────┘ │
└───────────┬─────────────────────────┬───────────────┘
            │                         │
            ▼                         ▼
    ┌───────────────┐         ┌──────────────┐
    │  BE.League.   │         │  BE.League.  │
    │   Desktop     │◄────────┤   Desktop    │
    │   Client      │         │   Reader     │
    └───────────────┘         └──────────────┘
            │
            ▼
    ┌───────────────┐
    │ Your          │
    │ Application   │
    └───────────────┘
```

## Requirements

- **.NET 9.0 SDK**
- **System.Management** NuGet package (for process discovery)
- **League of Legends** installed and running

## License

See LICENSE file for details.

## Resources

- 📖 [Integration Guide](LEAGUE_OF_LEGENDS_INTEGRATION.md)
- 🔧 [Test Documentation](BE.League.Desktop.Tests/TEST_STRUCTURE_DOCUMENTATION.md)
- 🎮 [Riot Developer Portal](https://developer.riotgames.com/)
- 📚 [HextechDocs](https://www.hextechdocs.dev/)

## Contributing

Contributions are welcome! Please:
1. Read the [Integration Guide](LEAGUE_OF_LEGENDS_INTEGRATION.md)
2. Add tests for new features
3. Update documentation
4. Test with actual League Client

---

**Note**: This is an unofficial library and is not endorsed by Riot Games. Use at your own risk.
 dazu 