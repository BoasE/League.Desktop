# League of Legends Local APIs Overview

This project integrates **both** local APIs exposed by a running League of Legends installation:
the **Game Client API** for real-time in-game data, and the
**League Client API** for desktop client state (lobby, matchmaking, champion select).

---

## 🎮 1. Game Client API (Port 2999)

**Official Documentation:**
- [Riot Games — Game Client API](https://developer.riotgames.com/docs/lol#game-client-api)
- [OpenAPI / Swagger spec](https://static.developer.riotgames.com/docs/lol/liveclientdata_sample.json)

**Summary:**
The **Game Client API** runs on fixed port **2999** and is available **only during an active game**.
It does **not require authentication** and provides real-time game information.

**Base URL:** `https://127.0.0.1:2999`

**Features:**
- Local (active) player state — stats, gold, level, abilities, runes
- All players list — champions, items, KDA, summoner spells
- Game events — kills, dragon, baron, inhibitor, etc.
- Game metadata — mode, map name, current game time

**Key classes:**

| Class | Role |
|-------|------|
| `IGameClientApi` | Raw JSON interface |
| `GameClientApiClient` | HttpClient transport (no auth, self-signed cert bypass) |
| `GameClientApiReader` | Typed deserializer → `AllGameData`, `ActivePlayer`, etc. |

---

## 🖥️ 2. League Client API (LCU)

**Official Documentation:**
- [Riot Games — League Client API](https://developer.riotgames.com/docs/lol#league-client-api)

**Community Resources:**
- [HexTech Docs](https://hextechdocs.dev/) — comprehensive LCU endpoint reference
- [Needlework.net](https://github.com/BlossomiShymae/Needlework.Net) — interactive LCU browser
- Swagger UI (inside the running client):
  - `{localaddress}/swagger/v2/swagger.json`
  - `{localaddress}/swagger/v3/swagger.json`

**Summary:**
The **League Client API** is the internal REST API used by the League of Legends **desktop client**.
It runs on a **dynamic port** (specified in the `lockfile`) and requires **Basic Authentication**
using the token stored there.

**Authentication:** `Authorization: Basic <base64("riot:<token>")>`

**Key Features:**
- Lobby management (create, join, leave)
- Champion select (picks, bans, trades)
- Matchmaking (join queue, accept/decline ready check)
- Summoner data (profile, ranked stats, match history)
- Store & loot (purchases, Hextech crafting)

**Key classes:**

| Class | Role |
|-------|------|
| `ILeagueClientApi` | Raw JSON interface |
| `LeagueClientApiClient` | HttpClient transport (Basic Auth, dynamic port, self-signed cert bypass) |
| `LeagueClientApiReader` | Typed deserializer → `Lobby`, `ChampSelectSession`, `ReadyCheck` |
| `LeagueClientApiEventLoop` | Polling loop — fires `LobbyChanged` / `ReadyCheckChanged` events |

---

## 🔑 LCU Port Discovery via Lockfile

### Overview
The League Client API uses a **dynamic port** that changes each time the League Client starts.
Applications must read the `lockfile` created at startup to discover the port and auth token.

### Lockfile Location
- **Windows**: `C:\Riot Games\League of Legends\lockfile`
- **macOS**: `~/Library/Application Support/Riot Games/League of Legends/lockfile`
- **General**: `<LeagueInstallDir>/lockfile`

### Lockfile Format
Single line, colon-separated:
```
ProcessName:ProcessID:Port:Password:Protocol
```
**Example:**
```
LeagueClientUx:12345:54321:a1b2c3d4e5f6:https
```

### Connection Steps
1. Read the lockfile
2. Parse: **port**, **password/token**, **protocol**
3. Base URL: `https://127.0.0.1:<port>`
4. Auth header: `Authorization: Basic <base64("riot:<token>")>`

### Lifecycle
- Lockfile **created** when League Client launches
- Lockfile **deleted** when the client closes
- File presence = reliable indicator that the client is running

### Discovery in this library

```csharp
// Returns null if the client is not running
var connection = LeagueClientConnectionInfo.GetFromRunningClient();

// Poll until running
await LeagueClientConnectionInfo.WaitForLeagueClient(cancellationToken);
```

---

## 🧰 Additional Resources

- [Riot Developer Portal](https://developer.riotgames.com/docs/lol) — Official LoL docs
- [HexTech Docs](https://hextechdocs.dev/) — Community LCU endpoint reference
- [Needlework.net](https://github.com/BlossomiShymae/Needlework.Net) — Interactive LCU browser
- [Community Discord](https://discord.gg/riotgamesdevrel) — Riot Games Third Party Developer Community

---

## 🏗️ Integration Overview

| API | Use case | Namespace |
|-----|----------|-----------|
| **Game Client API** | Real-time in-game data | `BE.League.Desktop.GameClientApi` |
| **League Client API** | Lobby & client management | `BE.League.Desktop.LeagueClientApi` |

```csharp
using var client = new LeagueDesktopClient();

// In-game data (Game Client API)
var gameData = await client.GameClient.GetAllGameDataAsync();

// Lobby/client data (League Client API)
var lobby = await client.LeagueClient.GetLobbyAsync();
await client.LeagueClient.AcceptReadyCheckAsync();
```
