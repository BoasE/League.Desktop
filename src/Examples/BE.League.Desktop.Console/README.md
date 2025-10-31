# League of Legends - Live Client Data Demo

Diese Konsolenanwendung demonstriert die Verwendung des **BE.League.Desktop** Clients zur Abfrage von Live-Spieldaten aus einem laufenden League of Legends Match.

## Funktionen

Die Demo zeigt folgende Live-Daten an:

### 🎮 Spiel-Informationen
- Map-Name (z.B. Summoner's Rift)
- Spielmodus
- Aktuelle Spielzeit

### 👤 Aktiver Spieler
- Beschwörername
- Champion-Level
- Aktuelles Gold
- Champion-Statistiken:
  - HP, AD, AP
  - Rüstung, Magieresistenz, Bewegungsgeschwindigkeit
  - Angriffstempo und kritische Trefferchance
- Runen-Konfiguration:
  - Hauptrune (Keystone)
  - Primärer Runenbaum
  - Sekundärer Runenbaum

### 👥 Team-Übersicht
- **Verbündete (Blau)**
  - Champion-Name
  - Beschwörername
  - KDA (Kills/Deaths/Assists)
  - CS (Creep Score)
  - Status (lebendig/tot)
  
- **Gegner (Rot)**
  - Gleiche Informationen wie bei Verbündeten

### 📰 Letzte Ereignisse
Zeigt die letzten 5 Ereignisse im Spiel:
- 💀 Champion-Kills
- 🔥 Multikills (Double, Triple, Quadra, Penta)
- 🐉 Drachen-Kills (mit Drachen-Typ)
- 👹 Baron Nashor Kills
- 🏰 Turm-Zerstörungen
- ⚔️ Inhibitor-Zerstörungen
- 🔄 Inhibitor-Respawns
- 👑 Team-Aces
- 🎮 Spielstart
- 🏁 Minions spawnen

## Voraussetzungen

- .NET 9.0
- League of Legends muss installiert sein
- Ein aktives Spiel muss laufen (Live Client Data API ist nur im Spiel verfügbar)

## Verwendung

1. **Projekt bauen:**
   ```cmd
   dotnet build
   ```

2. **Programm starten:**
   ```cmd
   dotnet run
   ```

3. **League of Legends starten:**
   - Die Anwendung wartet automatisch auf ein aktives Spiel
   - Sobald ein Spiel läuft, werden die Live-Daten angezeigt
   - Die Daten werden alle 5 Sekunden aktualisiert

4. **Beenden:**
   - Drücke `Strg+C` um die Anwendung zu beenden

## Technische Details

### API-Endpoint
Die Live Client Data API läuft standardmäßig auf:
- **URL:** `https://127.0.0.1:2999`
- **Protokoll:** HTTPS mit selbst-signiertem Zertifikat

### Verwendete Komponenten

```csharp
// LiveClientObjectReader - Hauptklasse für Datenanfragen
var reader = new LiveClientObjectReader();

// Alle Spieldaten abrufen
var allGameData = await reader.GetAllGameDataAsync();

// Enthält:
// - allGameData.GameData     - Allgemeine Spielinformationen
// - allGameData.ActivePlayer - Daten des aktiven Spielers
// - allGameData.AllPlayers   - Liste aller Spieler
// - allGameData.Events       - Spiel-Ereignisse
```

### Einzelne Datenabfragen

Der `LiveClientObjectReader` bietet auch Methoden für spezifische Daten:

```csharp
// Nur aktiver Spieler
var activePlayer = await reader.GetActivePlayerAsync();

// Nur Spielerliste
var players = await reader.GetPlayerListAsync();

// Nur Events
var events = await reader.GetEventDataAsync();

// Spieler-spezifische Daten
var scores = await reader.GetPlayerScoresAsync("SummonerName");
var items = await reader.GetPlayerItemsAsync("SummonerName");
var spells = await reader.GetPlayerSummonerSpellsAsync("SummonerName");
```

## Fehlerbehandlung

Die Anwendung behandelt folgende Szenarien:
- ❌ Kein laufendes Spiel → Wartet und versucht es erneut
- ❌ API nicht verfügbar → Wartet und versucht es erneut
- ❌ Timeout → Wartet und versucht es erneut
- ❌ Ungültige JSON-Daten → Zeigt Fehler an und wartet

## Anpassungsmöglichkeiten

### Update-Intervall ändern
```csharp
// Statt 5 Sekunden
await Task.Delay(5000);

// z.B. 1 Sekunde
await Task.Delay(1000);
```

### API-Konfiguration
```csharp
var options = new LeagueDesktopOptions
{
    Url = "https://127.0.0.1:2999",
    Timeout = TimeSpan.FromSeconds(10)
};

var reader = new LiveClientObjectReader(options);
```

## Beispiel-Output

```
╔════════════════════════════════════════════════════════════════╗
║             LIVE GAME DATA - League of Legends                 ║
╚════════════════════════════════════════════════════════════════╝

┌─── SPIEL-INFORMATIONEN ────────────────────────────────────────┐
│ Map: Summoner's Rift                                           │
│ Spielmodus: CLASSIC                                            │
│ Spielzeit: 15:42                                               │
└────────────────────────────────────────────────────────────────┘

┌─── AKTIVER SPIELER ────────────────────────────────────────────┐
│ Name: YourSummonerName                                         │
│ Level: 12                                                      │
│ Gold: 8750                                                     │
│                                                                │
│ HP: 1820/2100 │ AD: 175 │ AP: 40                              │
│ Armor: 85 │ MR: 45 │ MS: 350                                  │
│ Angriffstempo: 1.23 │ Krit-Chance: 25%                        │
│                                                                │
│ Hauptrune: Press the Attack                                   │
│ Primärer Baum: Precision                                       │
│ Sekundärer Baum: Domination                                    │
└────────────────────────────────────────────────────────────────┘

┌─── TEAM ÜBERSICHT ─────────────────────────────────────────────┐
│                                                                │
│ VERBÜNDETE (Blau):                                            │
│ ✓ Jinx         │ Player1              │ 5/2/3   │ CS:142 │
│ ✓ Thresh       │ Player2              │ 0/1/8   │ CS:45  │
│ 💀 Yasuo        │ Player3              │ 3/4/2   │ CS:128 │
│ ✓ Vi           │ Player4              │ 4/3/6   │ CS:98  │
│ ✓ Ahri         │ Player5              │ 6/2/4   │ CS:156 │
│                                                                │
│ GEGNER (Rot):                                                  │
│ ✓ Vayne        │ Enemy1               │ 4/3/2   │ CS:135 │
│ 💀 Lux          │ Enemy2               │ 1/5/6   │ CS:42  │
│ ✓ Zed          │ Enemy3               │ 5/4/3   │ CS:148 │
│ ✓ Lee Sin      │ Enemy4               │ 3/3/7   │ CS:92  │
│ ✓ Darius       │ Enemy5               │ 2/4/5   │ CS:142 │
└────────────────────────────────────────────────────────────────┘

┌─── LETZTE EREIGNISSE ──────────────────────────────────────────┐
│ [15:32] 💀 Ahri hat Lux getötet                                │
│ [15:18] 🐉 Cloud Drache getötet von Vi                         │
│ [14:55] 🔥 TRIPLE KILL für Jinx!                               │
│ [14:42] 🏰 Turm zerstört: Turret_T2_C_04                       │
│ [14:20] 💀 Yasuo hat Zed getötet                               │
└────────────────────────────────────────────────────────────────┘

Nächste Aktualisierung in 5 Sekunden...
```

## Dokumentation

Weitere Informationen zur Live Client Data API:
- [Riot Games Developer Portal](https://developer.riotgames.com/docs/lol#game-client-api_live-client-data-api)

## Lizenz

Dieses Projekt ist Teil von **BE.League** und unterliegt den gleichen Lizenzbedingungen.

