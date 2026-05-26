# BE.League.Desktop WinForms Showcase

An interactive WinForms application that demonstrates all features of the two local
League of Legends APIs through live data.

---

## What it shows

### 🟢 Dashboard Tab
Connection status for both APIs — clearly shows whether the League Client and/or
an active game is detected.

### 🖥️ League Client API Tab
> Official docs: https://developer.riotgames.com/docs/lol#league-client-api  
> Community reference: https://hextechdocs.dev/

| Feature | Endpoint |
|---------|---------|
| Lobby info (game mode, queue, map, members + positions) | `GET /lol-lobby/v2/lobby` |
| Ready check state + timer + Accept/Decline buttons | `GET /lol-matchmaking/v1/ready-check` |
| Champion select phase + team picks | `GET /lol-champ-select/v1/session` |

### 🎮 Game Client API Tab
> Official docs: https://developer.riotgames.com/docs/lol#game-client-api  
> OpenAPI spec: https://static.developer.riotgames.com/docs/lol/liveclientdata_sample.json

| Feature | Endpoint |
|---------|---------|
| Game time, mode, map + terrain | `GET /liveclientdata/gamestats` |
| Active player: level, gold, HP, mana, all champion stats | `GET /liveclientdata/activeplayer` |
| Abilities (P/Q/W/E/R) with levels | `GET /liveclientdata/activeplayer` |
| Keystone + rune trees | `GET /liveclientdata/activeplayerrunes` |
| All 10 players: champion, KDA, CS, summoner spells, keystone | `GET /liveclientdata/playerlist` |
| Live event log (kills, dragons, barons, turrets, aces) | `GET /liveclientdata/eventdata` |

### 📋 Raw JSON Tab
Select any endpoint from a dropdown (Game Client API or League Client API) and
view the raw JSON response with pretty-printing. Includes a Copy button.

---

## Prerequisites

| Scenario | Requirement |
|----------|-------------|
| League Client tab | League of Legends client is running |
| Game Client tab | An active game is in progress (Practice Tool, Custom, or matchmade) |

The app **auto-reconnects** and **auto-refreshes** every 3 seconds. No manual restart needed
when the client starts or a game begins.

---

## Running

```bash
cd src
dotnet run --project Examples/BE.League.Desktop.WinFormsShowcase
```

Or open in Rider and run the `BE.League.Desktop.WinFormsShowcase` configuration.

---

## Features

- ⏱ **Auto-refresh** every 3 seconds (toggle in toolbar)
- ⟳ **Manual refresh** button
- ✔ **Accept Ready Check** directly from the toolbar or League Client tab
- ✘ **Decline Ready Check** directly from the toolbar or League Client tab
- 🎨 **Color-coded player rows** — blue team vs. red team, dead players grayed out
- ⭐ **Lobby leader** highlighted in yellow
- 🟦 **My cell** highlighted in champion select
- 📋 **Clipboard copy** for raw JSON

