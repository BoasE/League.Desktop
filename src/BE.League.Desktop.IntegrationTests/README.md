# BE.League.Desktop Integration Tests

## Overview

Integration tests for the BE.League.Desktop library. These tests verify live communication
with a running League of Legends client and/or game.

## Prerequisites

⚠️ **IMPORTANT**: These tests require League of Legends to be running on the same machine.

| Test Class | Prerequisite | What it tests |
|------------|-------------|---------------|
| `GivenLeagueDesktopClient` | League Client running (any state) | Basic connectivity |
| `WhenInLeagueClient` | League Client running (lobby or menu) | [League Client API](https://developer.riotgames.com/docs/lol#league-client-api) endpoints |
| `WhenInActiveGame` | Active game running | [Game Client API](https://developer.riotgames.com/docs/lol#game-client-api) endpoints |

## Running the Tests

```bash
# Run all integration tests
dotnet test

# Run a specific class
dotnet test --filter "FullyQualifiedName~GivenLeagueDesktopClient"
dotnet test --filter "FullyQualifiedName~WhenInLeagueClient"
dotnet test --filter "FullyQualifiedName~WhenInActiveGame"
```

## Test Behavior

- Tests **silently skip** (early return) when prerequisites are not met
- No test fails just because League is not running
- Skipped tests show as "Passed" but perform no assertions

## Test Structure

```
BE.League.Desktop.IntegrationTests/
├── GivenLeagueDesktopClient.cs   # Basic connection tests
├── WhenInLeagueClient.cs         # League Client API (lobby, champ select, ready check)
└── WhenInActiveGame.cs           # Game Client API (player data, game stats)
```

## API Endpoints Tested

### Game Client API (Port 2999)
> Official docs: https://developer.riotgames.com/docs/lol#game-client-api

- `GET /liveclientdata/activeplayername`
- `GET /liveclientdata/activeplayer`
- `GET /liveclientdata/allgamedata`
- `GET /liveclientdata/gamestats`
- `GET /liveclientdata/playerlist`

### League Client API (Dynamic Port)
> Official docs: https://developer.riotgames.com/docs/lol#league-client-api

- `GET /lol-lobby/v2/lobby`
- `GET /lol-champ-select/v1/session`
- `GET /lol-matchmaking/v1/ready-check`

## Tips for Testing

1. **For `WhenInActiveGame`**: Start a Practice Tool game for easy testing
2. **For `WhenInLeagueClient`**: Stay in the client lobby or main menu
3. **Run incrementally**: Start with `GivenLeagueDesktopClient`, then move to specific scenarios

## Troubleshooting

| Symptom | Likely Cause |
|---------|-------------|
| All tests pass without assertions | League of Legends is not running |
| `WhenInActiveGame` tests skip | Not in an active game — start Practice Tool |
| Connection errors | Check Windows Firewall; try running as administrator |

## Notes

- These are **integration tests**, not unit tests
- All operations are **read-only** (non-destructive)
- Test duration depends on network response times from the local client
