# League of Legends Local APIs Overview

This project integrates **both** the League of Legends **Live Client Data API** and the **League Client Update (LCU) API** to access in-game and client-side data.

---

## 🧠 1. Live Client Data API (Port 2999)

**Official Documentation:**
- [Main Documentation](https://developer.riotgames.com/docs/lol#game-client-api)
- [Swagger/OpenAPI Spec](https://static.developer.riotgames.com/docs/lol/liveclientdata_sample.json)

**Summary:**
The **Live Client Data API** runs on port **2999** and is available **only during an active game**.  
It does **not require authentication** and provides real-time game information.

**Features:**
- Player states (active player and all players)
- Champion abilities, items, and runes
- Game statistics and events
- Game score and time

**Base URL:**  
- https://127.0.0.1:2999

---

## ⚙️ 2. League Client Update (LCU) API

**Other Documentation:**
- [Hextech Docs (comprehensive)](https://hextechdocs.dev/)

**Summary:**
The **LCU API** is the internal REST API used by the League of Legends **desktop client**.  
It runs on a **dynamic port** (specified in the `lockfile`) and requires **Basic Authentication** using the token stored there.



**Key Features:**
- Lobby management (create, join, leave)
- Champion select (picks, bans, trades)
- Matchmaking (join queue, accept/decline ready check)
- Summoner data (profile, ranked stats, match history)
- Store & loot (purchases, Hextech crafting)

---
## **LCU Port Discovery via Lockfile**

### **Overview**
The LCU API uses a **dynamic port** that changes each time the League Client starts. To connect to the API, applications must read the `lockfile` created by the client at startup.

### **Lockfile Location**
The lockfile is typically located at:
- **Windows**: `C:\Riot Games\League of Legends\lockfile`
- **macOS**: `~/Library/Application Support/Riot Games/League of Legends/lockfile`
- **General**: `<LeagueInstallDir>/lockfile`

### **Lockfile Format**
The file contains a single line with colon-separated values:
ProcessName:ProcessID:Port:Password:Protocol
**Example:**
LeagueClientUx:12345:54321:a1b2c3d4e5f6:https


### **Connection Steps**
1. **Read the lockfile** from the League Client installation directory
2. **Parse the values**:
    - Port number (e.g., `54321`)
    - Authentication token/password (e.g., `a1b2c3d4e5f6`)
    - Protocol (usually `https`)
3. **Construct the base URL**: `https://127.0.0.1:<port>`
4. **Authenticate** using Basic Authentication:
    - Username: `riot`
    - Password: `<token from lockfile>`
    - Header: `Authorization: Basic <base64(riot:token)>`

### **Lifecycle**
- The lockfile is **created** when the League Client launches
- The lockfile is **deleted** when the client closes
- File presence = reliable indicator of whether the client is running
--
## 🧰 Additional Resources

- [Needlework.net](https://github.com/BlossomiShymae/Needlework.Net) – interactive browser for exploring LCU endpoints
- **Swagger UI (inside the client):**  
  - {localaddress}/swagger/v2/swagger.json
  - {localaddress}/swagger/v3/swagger.json
- **Community Discord:** [Riot Games Third Party Developer Community](https://discord.gg/riotgamesdevrel)

---

## 🎮 Integration Overview

This project uses:
- **Live Client Data API** → for real-time **in-game** data
- **LCU API** → for **client and lobby** management

Together, these provide a full spectrum of interaction from **lobby creation** to **live match monitoring**.
