# API Verwendungsbeispiele

Diese Datei enthält praktische Beispiele für die Verwendung der **BE.League.Desktop** API.

## Schnellstart

### Minimales Beispiel

```csharp
using BE.Riot.Gateways.LeagueDesktop;

var reader = new LiveClientObjectReader();
var allGameData = await reader.GetAllGameDataAsync();

if (allGameData != null)
{
    Console.WriteLine($"Spieler: {allGameData.ActivePlayer?.SummonerName}");
    Console.WriteLine($"Spielzeit: {allGameData.GameData?.GameTime}s");
}
```

## Detaillierte Beispiele

### 1. Nur aktiven Spieler abrufen

```csharp
var reader = new LiveClientObjectReader();
var activePlayer = await reader.GetActivePlayerAsync();

if (activePlayer != null)
{
    Console.WriteLine($"Name: {activePlayer.SummonerName}");
    Console.WriteLine($"Level: {activePlayer.Level}");
    Console.WriteLine($"Gold: {activePlayer.CurrentGold:F0}");
    
    // Champion-Stats
    if (activePlayer.ChampionStats != null)
    {
        var stats = activePlayer.ChampionStats;
        Console.WriteLine($"HP: {stats.CurrentHealth:F0}/{stats.MaxHealth:F0}");
        Console.WriteLine($"AD: {stats.AttackDamage:F0}");
        Console.WriteLine($"AP: {stats.AbilityPower:F0}");
    }
    
    // Runen
    if (activePlayer.FullRunes?.Keystone != null)
    {
        Console.WriteLine($"Hauptrune: {activePlayer.FullRunes.Keystone.DisplayName}");
    }
}
```

### 2. Alle Spieler im Spiel

```csharp
var reader = new LiveClientObjectReader();
var players = await reader.GetPlayerListAsync();

if (players != null)
{
    // Verbündete
    var allies = players.Where(p => p.Team == "ORDER");
    Console.WriteLine("VERBÜNDETE:");
    foreach (var player in allies)
    {
        var kda = player.Scores != null 
            ? $"{player.Scores.Kills}/{player.Scores.Deaths}/{player.Scores.Assists}" 
            : "N/A";
        Console.WriteLine($"{player.ChampionName} - {player.SummonerName} - KDA: {kda}");
    }
    
    // Gegner
    var enemies = players.Where(p => p.Team == "CHAOS");
    Console.WriteLine("\nGEGNER:");
    foreach (var player in enemies)
    {
        var status = player.IsDead ? "💀 TOT" : "✓ LEBT";
        Console.WriteLine($"{status} - {player.ChampionName} ({player.SummonerName})");
    }
}
```

### 3. Spiel-Events überwachen

```csharp
var reader = new LiveClientObjectReader();
var events = await reader.GetEventDataAsync();

if (events?.EventsList != null)
{
    // Filtere nach bestimmten Event-Typen
    var kills = events.EventsList.Where(e => e.EventName == "ChampionKill");
    var dragons = events.EventsList.Where(e => e.EventName == "DragonKill");
    var barons = events.EventsList.Where(e => e.EventName == "BaronKill");
    
    Console.WriteLine($"Champion-Kills: {kills.Count()}");
    Console.WriteLine($"Drachen: {dragons.Count()}");
    Console.WriteLine($"Barons: {barons.Count()}");
    
    // Zeige letzte Events
    var recentEvents = events.EventsList.TakeLast(5).Reverse();
    Console.WriteLine("\nLETZTE EVENTS:");
    foreach (var evt in recentEvents)
    {
        var time = TimeSpan.FromSeconds(evt.EventTime);
        Console.WriteLine($"[{time:mm\\:ss}] {evt.EventName}");
    }
}
```

### 4. Spieler-spezifische Daten

```csharp
var reader = new LiveClientObjectReader();
string summonerName = "PlayerName";

// Scores
var scores = await reader.GetPlayerScoresAsync(summonerName);
if (scores != null)
{
    Console.WriteLine($"KDA: {scores.Kills}/{scores.Deaths}/{scores.Assists}");
    Console.WriteLine($"CS: {scores.CreepScore}");
    Console.WriteLine($"Ward Score: {scores.WardScore}");
}

// Items
var items = await reader.GetPlayerItemsAsync(summonerName);
if (items != null)
{
    Console.WriteLine("\nITEMS:");
    foreach (var item in items.Where(i => i.ItemId != 0))
    {
        Console.WriteLine($"Slot {item.Slot}: {item.DisplayName} ({item.Price}g)");
    }
}

// Beschwörerzauber
var spells = await reader.GetPlayerSummonerSpellsAsync(summonerName);
if (spells != null)
{
    Console.WriteLine("\nBESCHWÖRERZAUBER:");
    Console.WriteLine($"1: {spells.SummonerSpellOne?.DisplayName}");
    Console.WriteLine($"2: {spells.SummonerSpellTwo?.DisplayName}");
}

// Runen
var runes = await reader.GetPlayerMainRunesAsync(summonerName);
if (runes != null)
{
    Console.WriteLine("\nRUNEN:");
    Console.WriteLine($"Keystone: {runes.Keystone?.DisplayName}");
    Console.WriteLine($"Primär: {runes.PrimaryRuneTree?.DisplayName}");
    Console.WriteLine($"Sekundär: {runes.SecondaryRuneTree?.DisplayName}");
}
```

### 5. Fähigkeiten des aktiven Spielers

```csharp
var reader = new LiveClientObjectReader();
var abilities = await reader.GetActivePlayerAbilitiesAsync();

if (abilities != null)
{
    Console.WriteLine("FÄHIGKEITEN:");
    
    if (abilities.Passive != null)
        Console.WriteLine($"Passiv: {abilities.Passive.DisplayName}");
    
    if (abilities.Q != null)
        Console.WriteLine($"Q: {abilities.Q.DisplayName} (Level {abilities.Q.AbilityLevel})");
    
    if (abilities.W != null)
        Console.WriteLine($"W: {abilities.W.DisplayName} (Level {abilities.W.AbilityLevel})");
    
    if (abilities.E != null)
        Console.WriteLine($"E: {abilities.E.DisplayName} (Level {abilities.E.AbilityLevel})");
    
    if (abilities.R != null)
        Console.WriteLine($"R: {abilities.R.DisplayName} (Level {abilities.R.AbilityLevel})");
}
```

### 6. Spielstatistiken

```csharp
var reader = new LiveClientObjectReader();
var gameData = await reader.GetGameStatsAsync();

if (gameData != null)
{
    var gameTime = TimeSpan.FromSeconds(gameData.GameTime);
    
    Console.WriteLine("SPIELINFORMATIONEN:");
    Console.WriteLine($"Map: {gameData.MapName}");
    Console.WriteLine($"Modus: {gameData.GameMode}");
    Console.WriteLine($"Spielzeit: {gameTime:mm\\:ss}");
    Console.WriteLine($"Map Nummer: {gameData.MapNumber}");
}
```

### 7. Kontinuierliches Monitoring

```csharp
var reader = new LiveClientObjectReader();

Console.WriteLine("Starte Monitoring...");

while (true)
{
    try
    {
        var allGameData = await reader.GetAllGameDataAsync();
        
        if (allGameData != null)
        {
            // Zeige wichtige Informationen
            var gameTime = TimeSpan.FromSeconds(allGameData.GameData?.GameTime ?? 0);
            var playerName = allGameData.ActivePlayer?.SummonerName;
            var gold = allGameData.ActivePlayer?.CurrentGold ?? 0;
            
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {playerName} - Zeit: {gameTime:mm\\:ss} - Gold: {gold:F0}");
            
            await Task.Delay(5000); // Update alle 5 Sekunden
        }
        else
        {
            Console.Write("."); // Warten auf Spiel
            await Task.Delay(2000);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Fehler: {ex.Message}");
        await Task.Delay(2000);
    }
}
```

### 8. Mit CancellationToken

```csharp
var reader = new LiveClientObjectReader();
var cts = new CancellationTokenSource();

// Bei Strg+C abbrechen
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    while (!cts.Token.IsCancellationRequested)
    {
        var allGameData = await reader.GetAllGameDataAsync(cts.Token);
        
        if (allGameData != null)
        {
            // Verarbeite Daten...
        }
        
        await Task.Delay(5000, cts.Token);
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Abgebrochen.");
}
```

### 9. Benutzerdefinierte Optionen

```csharp
var options = new LeagueDesktopOptions
{
    Url = "https://127.0.0.1:2999",
    Timeout = TimeSpan.FromSeconds(5)
};

var reader = new LiveClientObjectReader(options);
var activePlayer = await reader.GetActivePlayerAsync();

if (activePlayer != null)
{
    Console.WriteLine($"Spieler: {activePlayer.SummonerName}");
}
```

### 10. Fehlerbehandlung

```csharp
var reader = new LiveClientObjectReader();

try
{
    var allGameData = await reader.GetAllGameDataAsync();
    
    if (allGameData == null)
    {
        Console.WriteLine("Kein aktives Spiel gefunden");
        Console.WriteLine("Stelle sicher, dass:");
        Console.WriteLine("1. League of Legends läuft");
        Console.WriteLine("2. Du in einem aktiven Spiel bist");
        Console.WriteLine("3. Die Live Client Data API aktiviert ist");
        return;
    }
    
    // Verarbeite Daten...
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Netzwerkfehler: {ex.Message}");
    Console.WriteLine("Die API ist möglicherweise nicht erreichbar");
}
catch (TaskCanceledException)
{
    Console.WriteLine("Timeout - Die Anfrage hat zu lange gedauert");
}
catch (JsonException ex)
{
    Console.WriteLine($"JSON-Fehler: {ex.Message}");
    Console.WriteLine("Die API-Antwort konnte nicht deserialisiert werden");
}
catch (Exception ex)
{
    Console.WriteLine($"Unerwarteter Fehler: {ex.Message}");
}
```

### 11. Team-Statistiken berechnen

```csharp
var reader = new LiveClientObjectReader();
var players = await reader.GetPlayerListAsync();

if (players != null)
{
    var blueTeam = players.Where(p => p.Team == "ORDER").ToList();
    var redTeam = players.Where(p => p.Team == "CHAOS").ToList();
    
    // Kills
    var blueKills = blueTeam.Sum(p => p.Scores?.Kills ?? 0);
    var redKills = redTeam.Sum(p => p.Scores?.Kills ?? 0);
    
    // Creep Score
    var blueCS = blueTeam.Sum(p => p.Scores?.CreepScore ?? 0);
    var redCS = redTeam.Sum(p => p.Scores?.CreepScore ?? 0);
    
    // Tote Spieler
    var blueDeaths = blueTeam.Count(p => p.IsDead);
    var redDeaths = redTeam.Count(p => p.IsDead);
    
    Console.WriteLine("TEAM STATISTIKEN:");
    Console.WriteLine($"Blaues Team - Kills: {blueKills} | CS: {blueCS} | Tot: {blueDeaths}/5");
    Console.WriteLine($"Rotes Team  - Kills: {redKills} | CS: {redCS} | Tot: {redDeaths}/5");
}
```

### 12. Event-Typen filtern

```csharp
var reader = new LiveClientObjectReader();
var events = await reader.GetEventDataAsync();

if (events?.EventsList != null)
{
    // Kills mit Assistern
    var killsWithAssists = events.EventsList
        .Where(e => e.EventName == "ChampionKill" && e.Assisters?.Count > 0);
    
    Console.WriteLine("KILLS MIT ASSISTS:");
    foreach (var kill in killsWithAssists)
    {
        var time = TimeSpan.FromSeconds(kill.EventTime);
        var assisters = string.Join(", ", kill.Assisters ?? new List<string>());
        Console.WriteLine($"[{time:mm\\:ss}] {kill.KillerName} → {kill.VictimName} (Assist: {assisters})");
    }
    
    // Multikills
    var multikills = events.EventsList
        .Where(e => e.EventName == "Multikill" && e.KillStreak >= 2);
    
    Console.WriteLine("\nMULTIKILLS:");
    foreach (var mk in multikills)
    {
        var time = TimeSpan.FromSeconds(mk.EventTime);
        var type = mk.KillStreak switch
        {
            2 => "Double Kill",
            3 => "Triple Kill",
            4 => "Quadra Kill",
            5 => "Penta Kill",
            _ => $"{mk.KillStreak}x Kill"
        };
        Console.WriteLine($"[{time:mm\\:ss}] {type} - {mk.KillerName}");
    }
    
    // Objektive (Drachen, Baron, Türme)
    var objectives = events.EventsList
        .Where(e => e.EventName is "DragonKill" or "BaronKill" or "TurretKilled");
    
    Console.WriteLine("\nOBJEKTIVE:");
    foreach (var obj in objectives)
    {
        var time = TimeSpan.FromSeconds(obj.EventTime);
        var description = obj.EventName switch
        {
            "DragonKill" => $"Drache ({obj.DragonType})",
            "BaronKill" => "Baron Nashor",
            "TurretKilled" => $"Turm ({obj.TurretKilled})",
            _ => obj.EventName
        };
        Console.WriteLine($"[{time:mm\\:ss}] {description}");
    }
}
```

## Tipps und Best Practices

### 1. Null-Checks sind wichtig

Die API gibt `null` zurück, wenn kein Spiel läuft oder die API nicht verfügbar ist:

```csharp
var data = await reader.GetAllGameDataAsync();
if (data == null)
{
    // Kein Spiel läuft
    return;
}

// Prüfe auch verschachtelte Objekte
if (data.ActivePlayer?.ChampionStats != null)
{
    // Verwende ChampionStats
}
```

### 2. Update-Intervalle

Für verschiedene Datentypen empfohlene Update-Intervalle:

- **Spiel-Events**: 2-3 Sekunden (neue Events werden häufiger generiert)
- **Spieler-Stats**: 3-5 Sekunden (Stats ändern sich kontinuierlich)
- **Allgemeine Spielinformationen**: 5-10 Sekunden (ändern sich weniger)
- **Runen/Items**: 10-30 Sekunden (ändern sich selten während des Spiels)

```csharp
// Häufig für Events
var events = await reader.GetEventDataAsync();
await Task.Delay(2000);

// Mittel für Stats
var player = await reader.GetActivePlayerAsync();
await Task.Delay(5000);
```

### 3. Verwende spezifische Endpunkte

Wenn du nur bestimmte Daten benötigst, verwende die spezifischen Endpunkte statt `GetAllGameDataAsync()`:

```csharp
// ❌ Nicht optimal - holt alle Daten
var allData = await reader.GetAllGameDataAsync();
var playerName = allData?.ActivePlayer?.SummonerName;

// ✅ Besser - holt nur den Namen
var playerName = await reader.GetActivePlayerNameAsync();
```

### 4. Async/Await richtig verwenden

```csharp
// ✅ Richtig - await verwenden
var data = await reader.GetAllGameDataAsync();

// ❌ Falsch - blockiert den Thread
var data = reader.GetAllGameDataAsync().Result;
```

### 5. CancellationToken für lange Operationen

```csharp
var cts = new CancellationTokenSource();

// Bei Bedarf abbrechen
cts.CancelAfter(TimeSpan.FromMinutes(5));

var data = await reader.GetAllGameDataAsync(cts.Token);
```

## Häufige Probleme

### Problem: API gibt null zurück

**Lösung:**
1. Stelle sicher, dass League of Legends läuft
2. Du musst in einem **aktiven Spiel** sein (nicht in der Lobby)
3. Die API ist nur während eines Spiels verfügbar (Practice Tool, Custom Game, etc.)

### Problem: Timeout-Fehler

**Lösung:**
```csharp
var options = new LeagueDesktopOptions
{
    Timeout = TimeSpan.FromSeconds(30) // Längerer Timeout
};
var reader = new LiveClientObjectReader(options);
```

### Problem: JSON-Fehler

**Lösung:**
- Die API-Struktur kann sich ändern
- Verwende `ExtensionData` für unbekannte Properties
- Aktualisiere die Bibliothek auf die neueste Version

## Weitere Ressourcen

- [Riot Games Developer Portal](https://developer.riotgames.com/docs/lol#game-client-api_live-client-data-api)
- [Projekt README](README.md)
- [API Examples Source Code](ApiExamples.cs)

