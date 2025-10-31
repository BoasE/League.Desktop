# Lobby & Ready Check Funktionen

## 🎯 Neue Funktionen

Der **BE.League.Desktop** Client wurde erweitert um:

### ✅ Lobby-Unterstützung
- Lobby-Informationen abrufen
- Champion-Select-Session überwachen
- Ready-Check-Status prüfen

### ✅ Automatisches Akzeptieren
- **Ready Check automatisch akzeptieren**, wenn ein Spiel gefunden wurde
- Ready Check ablehnen
- Kontinuierliches Monitoring

### ✅ League Client API (LCU) Integration
- Automatische Erkennung des League Client Prozesses
- Authentifizierung mit dynamischem Port und Token
- Unterstützung für alle LCU-Endpunkte

---

## 📚 API-Referenz

### ILeagueDesktopClient - Neue Methoden

```csharp
// Lobby-Informationen
Task<string?> GetLobbyJsonAsync(CancellationToken ct = default);

// Champion Select
Task<string?> GetChampSelectSessionJsonAsync(CancellationToken ct = default);

// Ready Check Status
Task<string?> GetReadyCheckJsonAsync(CancellationToken ct = default);

// Ready Check akzeptieren
Task<bool> AcceptReadyCheckAsync(CancellationToken ct = default);

// Ready Check ablehnen
Task<bool> DeclineReadyCheckAsync(CancellationToken ct = default);
```

### LiveClientObjectReader - Neue Methoden

```csharp
// Lobby-Informationen (deserialisiert)
Task<LobbyDto?> GetLobbyAsync(CancellationToken ct = default);

// Champion Select (deserialisiert)
Task<ChampSelectSession?> GetChampSelectSessionAsync(CancellationToken ct = default);

// Ready Check Status (deserialisiert)
Task<ReadyCheckDto?> GetReadyCheckAsync(CancellationToken ct = default);

// Ready Check akzeptieren
Task<bool> AcceptReadyCheckAsync(CancellationToken ct = default);

// Ready Check ablehnen
Task<bool> DeclineReadyCheckAsync(CancellationToken ct = default);
```

---

## 💡 Verwendungsbeispiele

### Beispiel 1: Lobby-Informationen abrufen

```csharp
using BE.Riot.Gateways.LeagueDesktop;

var reader = new LiveClientObjectReader();

var lobby = await reader.GetLobbyAsync();

if (lobby != null)
{
    Console.WriteLine($"Lobby mit {lobby.Members.Length} Mitgliedern");
    
    foreach (var member in lobby.Members)
    {
        Console.WriteLine($"  - {member.SummonerName}");
    }
    
    if (lobby.GameConfig?.QueueId.HasValue == true)
    {
        Console.WriteLine($"Queue ID: {lobby.GameConfig.QueueId}");
    }
}
```

### Beispiel 2: Ready Check automatisch akzeptieren

```csharp
using BE.Riot.Gateways.LeagueDesktop;

var reader = new LiveClientObjectReader();

Console.WriteLine("Warte auf Ready Check...");

while (true)
{
    var readyCheck = await reader.GetReadyCheckAsync();
    
    if (readyCheck != null && readyCheck.State == "InProgress")
    {
        Console.WriteLine("🔔 Spiel gefunden! Akzeptiere...");
        
        var accepted = await reader.AcceptReadyCheckAsync();
        
        if (accepted)
        {
            Console.WriteLine("✓ Ready Check akzeptiert!");
            break;
        }
    }
    
    await Task.Delay(2000);
}
```

### Beispiel 3: Champion Select überwachen

```csharp
using BE.Riot.Gateways.LeagueDesktop;

var reader = new LiveClientObjectReader();

var champSelect = await reader.GetChampSelectSessionAsync();

if (champSelect != null)
{
    Console.WriteLine($"Champion Select - Phase: {champSelect.Timer?.Phase}");
    Console.WriteLine($"Deine Cell ID: {champSelect.LocalPlayerCellId}");
    
    if (champSelect.MyTeam.Count > 0)
    {
        Console.WriteLine("\nDein Team:");
        
        foreach (var member in champSelect.MyTeam)
        {
            var isYou = member.CellId == champSelect.LocalPlayerCellId ? " (DU)" : "";
            
            Console.WriteLine($"  Cell {member.CellId}{isYou}: Champion ID {member.ChampionId}");
            
            if (member.ChampionPickIntent.HasValue)
            {
                Console.WriteLine($"    Intent: Champion ID {member.ChampionPickIntent.Value}");
            }
        }
    }
}
```

### Beispiel 4: Kombiniertes Lobby & Game Monitoring

```csharp
using BE.Riot.Gateways.LeagueDesktop;

var reader = new LiveClientObjectReader();

Console.WriteLine("Überwache League of Legends...\n");

while (true)
{
    // Prüfe auf laufendes Spiel
    var gameData = await reader.GetAllGameDataAsync();
    
    if (gameData != null)
    {
        // Im Spiel
        var gameTime = TimeSpan.FromSeconds(gameData.GameData?.GameTime ?? 0);
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🎮 IM SPIEL - Zeit: {gameTime:mm\\:ss}");
    }
    else
    {
        // Nicht im Spiel - Prüfe Lobby
        var lobby = await reader.GetLobbyAsync();
        
        if (lobby != null)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🏠 In Lobby mit {lobby.Members.Length} Mitgliedern");
            
            // Prüfe Ready Check
            var readyCheck = await reader.GetReadyCheckAsync();
            
            if (readyCheck != null && readyCheck.State == "InProgress")
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🔔 READY CHECK! Akzeptiere...");
                
                var accepted = await reader.AcceptReadyCheckAsync();
                
                if (accepted)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✓ Akzeptiert!");
                }
            }
        }
        else
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ⏸️ Warte auf Lobby oder Spiel...");
        }
    }
    
    await Task.Delay(3000);
}
```

### Beispiel 5: Auto-Accept mit CancellationToken

```csharp
using BE.Riot.Gateways.LeagueDesktop;

var reader = new LiveClientObjectReader();
var cts = new CancellationTokenSource();

// Bei Strg+C abbrechen
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

Console.WriteLine("Auto-Accept aktiviert...");
Console.WriteLine("Drücke Strg+C zum Beenden\n");

try
{
    while (!cts.Token.IsCancellationRequested)
    {
        var readyCheck = await reader.GetReadyCheckAsync(cts.Token);
        
        if (readyCheck != null && readyCheck.State == "InProgress")
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🔔 Spiel gefunden!");
            
            var accepted = await reader.AcceptReadyCheckAsync(cts.Token);
            
            if (accepted)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✓ Automatisch akzeptiert!");
                
                // Warte 10 Sekunden nach Akzeptieren
                await Task.Delay(10000, cts.Token);
            }
        }
        
        await Task.Delay(1000, cts.Token);
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("\nAuto-Accept beendet");
}
```

---

## 🏗️ Technische Details

### League Client API (LCU)

Die LCU-API läuft lokal auf einem dynamischen Port mit Basic-Authentifizierung:

- **Port**: Dynamisch (wird aus dem Prozess ausgelesen)
- **Token**: Dynamisch (wird aus dem Prozess ausgelesen)
- **Protokoll**: HTTPS mit selbst-signiertem Zertifikat
- **Base URL**: `https://127.0.0.1:{port}`
- **Auth**: Basic Auth mit `riot:{token}`

### Automatische Verbindung

Der `LeagueDesktopClient` erkennt automatisch den League Client:

```csharp
// Automatische Erkennung
var client = new LeagueDesktopClient();

// LCU-Funktionen sind verfügbar, wenn der Client läuft
var lobby = await client.GetLobbyJsonAsync();
```

### LeagueClientConnectionInfo

Hilfsklasse zum Auslesen der LCU-Verbindungsdaten:

```csharp
var connectionInfo = LeagueClientConnectionInfo.GetFromRunningClient();

if (connectionInfo?.IsValid == true)
{
    Console.WriteLine($"Port: {connectionInfo.Port}");
    Console.WriteLine($"Token: {connectionInfo.Token}");
    Console.WriteLine($"Base URL: {connectionInfo.GetBaseUrl()}");
}
```

### Unterstützte Endpunkte

#### Lobby
- `/lol-lobby/v2/lobby` - Lobby-Informationen

#### Champion Select
- `/lol-champ-select/v1/session` - Champion-Select-Session

#### Matchmaking
- `/lol-matchmaking/v1/ready-check` - Ready-Check-Status
- `/lol-matchmaking/v1/ready-check/accept` - Ready Check akzeptieren (POST)
- `/lol-matchmaking/v1/ready-check/decline` - Ready Check ablehnen (POST)

---

## 📦 Datenmodelle

### LobbyDto

```csharp
public sealed class LobbyDto
{
    public LobbyMember[] Members { get; set; }
    public GameConfigDto? GameConfig { get; set; }
}
```

### LobbyMember

```csharp
public sealed class LobbyMember
{
    public string? SummonerName { get; set; }
}
```

### GameConfigDto

```csharp
public sealed class GameConfigDto
{
    public int? QueueId { get; set; }
}
```

### ReadyCheckDto

```csharp
public sealed class ReadyCheckDto
{
    public string? State { get; set; }
}
```

**Mögliche States:**
- `"InProgress"` - Ready Check läuft
- `"EveryoneReady"` - Alle haben akzeptiert
- `"StrangerNotReady"` - Jemand hat nicht akzeptiert
- `null` - Kein Ready Check aktiv

### ChampSelectSession

```csharp
public sealed class ChampSelectSession
{
    public int LocalPlayerCellId { get; set; }
    public ChampSelectTimer? Timer { get; set; }
    public List<ChampSelectMember> MyTeam { get; set; }
    public List<ChampSelectMember> TheirTeam { get; set; }
}
```

---

## 🎮 Queue IDs

Häufige Queue IDs:

| Queue ID | Beschreibung |
|----------|--------------|
| 420 | Ranked Solo/Duo |
| 440 | Ranked Flex |
| 450 | ARAM |
| 400 | Normal Draft |
| 430 | Normal Blind |
| 490 | Normal Quickplay |
| 700 | Clash |
| 900 | ARURF |
| 1020 | One for All |
| 1300 | Nexus Blitz |
| 1400 | Ultimate Spellbook |

---

## ⚠️ Wichtige Hinweise

### Voraussetzungen

1. **League of Legends muss laufen**
   - Der League Client (nicht das Spiel) muss gestartet sein
   - LCU-API ist nur verfügbar, wenn der Client läuft

2. **Windows-Only**
   - Die automatische Prozess-Erkennung funktioniert nur unter Windows
   - Verwendet `System.Management` für WMI-Abfragen

3. **Berechtigungen**
   - Keine Administrator-Rechte erforderlich
   - Funktioniert mit normalen Benutzerrechten

### Fehlerbehandlung

- **Methoden geben `null` zurück**, wenn:
  - League Client nicht läuft
  - Keine Lobby/Champion Select aktiv ist
  - API nicht erreichbar ist

- **`AcceptReadyCheckAsync()` gibt `false` zurück**, wenn:
  - League Client nicht läuft
  - Kein Ready Check aktiv ist
  - Request fehlschlägt

### Performance

- **LCU-API ist schneller als Live Client Data API**
  - Empfohlenes Update-Intervall: 1-2 Sekunden
  - Ready Check sollte häufiger geprüft werden (500-1000ms)

---

## 📖 Weitere Beispiele

Siehe auch:
- `LobbyExamples.cs` - 8 vollständige Code-Beispiele
- `ApiExamples.cs` - 12 Beispiele für Live Client Data API
- `Program.cs` - Live Game Monitoring Demo

---

## 🔗 Ressourcen

- [Riot LCU API Dokumentation](https://developer.riotgames.com/docs/lol)
- [Rift Explorer](https://github.com/Pupix/rift-explorer) - LCU API Explorer
- [lcu-driver](https://github.com/sousa-andre/lcu-driver) - Python LCU Bibliothek

---

**Erstellt am**: 2025-10-30  
**Version**: 1.0  
**Status**: ✅ Produktionsbereit

