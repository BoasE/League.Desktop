# BE.League.Desktop Integration Tests

## Overview

This project contains integration tests for the BE.League.Desktop library. These tests verify that the library can successfully communicate with a running League of Legends client and/or game.

## Prerequisites

⚠️ **IMPORTANT**: These tests require League of Legends to be running on the same machine.

Different test scenarios have different requirements:

### GivenLeagueDesktopClient
- **Requirement**: League of Legends client must be running (can be in menu)
- **Tests**: Basic connection and client instantiation

### WhenInLeagueClient
- **Requirement**: League Client must be running (lobby or menu)
- **Tests**: LCU (League Client Update) API endpoints
  - Lobby status
  - Champion select session
  - Ready check status

### WhenInActiveGame
- **Requirement**: Must be in an active game (Practice Tool, Custom Game, or matchmade game)
- **Tests**: Live Client Data API endpoints
  - Active player information
  - Game statistics
  - Player list
  - All game data

## Running the Tests

### Run all integration tests:
```bash
dotnet test
```

### Run specific test class:
```bash
dotnet test --filter "FullyQualifiedName~GivenLeagueDesktopClient"
dotnet test --filter "FullyQualifiedName~WhenInLeagueClient"
dotnet test --filter "FullyQualifiedName~WhenInActiveGame"
```

## Test Behavior

- Tests automatically **skip** if the required prerequisites are not met
- No tests will fail due to League of Legends not running
- Tests use `Skip.If()` to conditionally skip based on client/game state

## Test Structure

```
BE.League.Desktop.IntegrationTests/
├── GivenLeagueDesktopClient.cs      # Basic client tests
├── WhenInLeagueClient.cs            # LCU API tests
└── WhenInActiveGame.cs              # Live game API tests
```

## Example Test Output

When League is not running:
```
[PASS] ItCanConnectToLiveClientDataApi (< 1ms)
  Note: Test skipped due to missing prerequisites (no assertions run)
```

When League is running:
```
[PASS] ItCanConnectToLiveClientDataApi (127ms)
[PASS] ItCanGetLobbyInformation (89ms)
```

## Tips for Testing

1. **For WhenInActiveGame tests**: Start a Practice Tool game for easy testing
2. **For WhenInLeagueClient tests**: Stay in the client lobby
3. **Run tests incrementally**: Start with basic tests, then move to specific scenarios

## API Endpoints Tested

### Live Client Data API (Port 2999)
- `/liveclientdata/activeplayername`
- `/liveclientdata/activeplayer`
- `/liveclientdata/allgamedata`
- `/liveclientdata/gamestats`
- `/liveclientdata/playerlist`

### LCU API (Dynamic Port)
- `/lol-lobby/v2/lobby`
- `/lol-champ-select/v1/session`
- `/lol-matchmaking/v1/ready-check`

## Troubleshooting

### All tests are skipped
- ✅ League of Legends client is not running
- ✅ Start the League client and try again

### WhenInActiveGame tests are skipped
- ✅ You're not in an active game
- ✅ Start a Practice Tool or Custom Game

### Tests fail with connection errors
- Check Windows Firewall settings
- Verify League of Legends is running with administrative privileges
- Restart the League client

## Notes

- These are **integration tests**, not unit tests
- They test real network communication with the League client
- Test execution time depends on network response times
- Tests are designed to be non-destructive (read-only operations)
# BE.League.Desktop Integration Tests

## Overview

This project contains integration tests for the BE.League.Desktop library. These tests verify that the library can successfully communicate with a running League of Legends client and/or game.

## Prerequisites

⚠️ **IMPORTANT**: These tests require League of Legends to be running on the same machine.

Different test scenarios have different requirements:

### GivenLeagueDesktopClient
- **Requirement**: League of Legends client must be running (can be in menu)
- **Tests**: Basic connection and client instantiation

### WhenInLeagueClient
- **Requirement**: League Client must be running (lobby or menu)
- **Tests**: LCU (League Client Update) API endpoints
  - Lobby status
  - Champion select session
  - Ready check status

### WhenInActiveGame
- **Requirement**: Must be in an active game (Practice Tool, Custom Game, or matchmade game)
- **Tests**: Live Client Data API endpoints
  - Active player information
  - Game statistics
  - Player list
  - All game data

## Running the Tests

### Run all integration tests:
```bash
dotnet test
```

### Run specific test class:
```bash
dotnet test --filter "FullyQualifiedName~GivenLeagueDesktopClient"
dotnet test --filter "FullyQualifiedName~WhenInLeagueClient"
dotnet test --filter "FullyQualifiedName~WhenInActiveGame"
```

## Test Behavior

- Tests automatically **skip silently** if the required prerequisites are not met (early return)
- No tests will fail due to League of Legends not running
- Tests use conditional return statements to skip when client/game is not available
- Skipped tests will show as "Passed" but perform no assertions

## Test Structure

```
BE.League.Desktop.IntegrationTests/
├── GivenLeagueDesktopClient.cs      # Basic client tests
├── WhenInLeagueClient.cs            # LCU API tests
└── WhenInActiveGame.cs              # Live game API tests
```

## Example Test Output

When League is not running:
```
[SKIPPED] ItCanConnectToLiveClientDataApi
  Reason: League of Legends is not running
```

When League is running:
```
[PASS] ItCanConnectToLiveClientDataApi (127ms)
[PASS] ItCanGetLobbyInformation (89ms)
```

## Tips for Testing

1. **For WhenInActiveGame tests**: Start a Practice Tool game for easy testing
2. **For WhenInLeagueClient tests**: Stay in the client lobby
3. **Run tests incrementally**: Start with basic tests, then move to specific scenarios

## API Endpoints Tested

### Live Client Data API (Port 2999)
- `/liveclientdata/activeplayername`
- `/liveclientdata/activeplayer`
- `/liveclientdata/allgamedata`
- `/liveclientdata/gamestats`
- `/liveclientdata/playerlist`

### LCU API (Dynamic Port)
- `/lol-lobby/v2/lobby`
- `/lol-champ-select/v1/session`
- `/lol-matchmaking/v1/ready-check`

## Troubleshooting

### All tests are skipped
- ✅ League of Legends client is not running
- ✅ Start the League client and try again

### WhenInActiveGame tests are skipped
- ✅ You're not in an active game
- ✅ Start a Practice Tool or Custom Game

### Tests fail with connection errors
- Check Windows Firewall settings
- Verify League of Legends is running with administrative privileges
- Restart the League client

## Notes

- These are **integration tests**, not unit tests
- They test real network communication with the League client
- Test execution time depends on network response times
- Tests are designed to be non-destructive (read-only operations)

