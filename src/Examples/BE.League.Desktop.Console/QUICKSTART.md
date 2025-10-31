# League Client Demo - Schnellstart-Anleitung

## 🚀 Schnellstart

### 1. Projekt ausführen

```cmd
cd C:\dev\Boas\BE.LeagueClient\src\BE.League\BE.League.Desktop.Console
dotnet run
```

### 2. Was passiert?

Die Anwendung startet und wartet auf ein aktives League of Legends Spiel:

```
╔════════════════════════════════════════════════════════════════╗
║        League of Legends - Live Client Data Demo              ║
╚════════════════════════════════════════════════════════════════╝

Warte auf ein aktives League of Legends Spiel...
(Drücke Strg+C zum Beenden)
```

### 3. League of Legends starten

Starte League of Legends und beginne ein Spiel:
- **Practice Tool** (empfohlen für Tests)
- **Custom Game**
- **Normales Spiel** / **Ranked**
- **ARAM** / **Andere Modi**

⚠️ **Wichtig:** Die Live Client Data API ist nur während eines aktiven Spiels verfügbar, nicht in der Lobby!

### 4. Live-Daten anzeigen

Sobald das Spiel läuft, zeigt die Anwendung automatisch:

#### 📊 Spiel-Informationen
- Map-Name und Spielmodus
- Aktuelle Spielzeit

#### 👤 Dein Spieler
- Name und Level
- Gold und Champion-Stats (HP, AD, AP, etc.)
- Runen-Konfiguration

#### 👥 Alle Teams
- KDA für jeden Spieler
- Creep Score (CS)
- Status (lebendig/tot)

#### 📰 Letzte Events
- Kills und Multikills
- Drachen und Baron
- Turm-Zerstörungen
- Und mehr...

## 📁 Projektstruktur

```
BE.League.Desktop.Console/
│
├── Program.cs              # Hauptprogramm mit Live-Demo
├── ApiExamples.cs          # 12 Code-Beispiele für verschiedene Szenarien
├── README.md               # Vollständige Dokumentation
├── USAGE_EXAMPLES.md       # Detaillierte Code-Beispiele und Best Practices
└── QUICKSTART.md           # Diese Datei
```

## 💡 Beispiel-Szenarien

### Szenario 1: Nur Spieler-Name anzeigen

```csharp
using BE.Riot.Gateways.LeagueDesktop;

var reader = new LiveClientObjectReader();
var name = await reader.GetActivePlayerNameAsync();

Console.WriteLine($"Aktiver Spieler: {name}");
```

### Szenario 2: Alle Spieler anzeigen

```csharp
using BE.Riot.Gateways.LeagueDesktop;

var reader = new LiveClientObjectReader();
var players = await reader.GetPlayerListAsync();

if (players != null)
{
    foreach (var player in players)
    {
        Console.WriteLine($"{player.ChampionName} - {player.SummonerName}");
    }
}
```

### Szenario 3: Kontinuierliches Monitoring

Siehe `Program.cs` für ein vollständiges Beispiel mit:
- Automatischer Wartefunktion
- Live-Updates alle 5 Sekunden
- Fehlerbehandlung
- Formatierte Ausgabe

## 🎯 Weitere Beispiele

Siehe `ApiExamples.cs` für 12 vollständige Beispiele:

1. **GetActivePlayerExample()** - Nur aktiven Spieler abrufen
2. **GetAllPlayersExample()** - Alle Spieler im Spiel
3. **GetGameEventsExample()** - Spiel-Events überwachen
4. **GetPlayerSpecificDataExample()** - Spieler-spezifische Daten
5. **GetGameStatsExample()** - Spielstatistiken
6. **GetActivePlayerAbilitiesExample()** - Fähigkeiten des Spielers
7. **GetActivePlayerRunesExample()** - Runen des Spielers
8. **GetActivePlayerNameExample()** - Nur den Namen abrufen
9. **CustomOptionsExample()** - Mit benutzerdefinierten Optionen
10. **ErrorHandlingExample()** - Fehlerbehandlung
11. **ContinuousMonitoringExample()** - Monitoring mit CancellationToken
12. **CalculateTeamStatsExample()** - Team-Statistiken berechnen

### Beispiel ausführen

Du kannst die Beispiele in `Program.cs` aufrufen:

```csharp
// Füge dies zu Program.cs hinzu (vor der while-Schleife)
using BE.League.Desktop.Console.Examples;

// Beispiel aufrufen
await ApiExamples.GetActivePlayerExample();
```

## 📖 Vollständige Dokumentation

- **README.md** - Vollständige Funktionsbeschreibung
- **USAGE_EXAMPLES.md** - 12 detaillierte Code-Beispiele mit Erklärungen
- **ApiExamples.cs** - Lauffähiger Code für alle Beispiele

## 🛠️ Troubleshooting

### Problem: "Kein aktives Spiel gefunden"

**Lösung:**
- Starte League of Legends
- Beginne ein Spiel (Practice Tool ist am einfachsten)
- Die API funktioniert nur während eines Spiels

### Problem: "API nicht verfügbar"

**Lösung:**
- League of Legends muss installiert sein
- Das Spiel muss laufen
- Die API läuft auf Port 2999 (Standard)

### Problem: Build-Fehler

**Lösung:**
```cmd
dotnet restore
dotnet build
```

## 🎮 Empfohlener Workflow für Tests

1. **Öffne Visual Studio / Rider / VS Code**
2. **Starte die Console-App:**
   ```cmd
   dotnet run
   ```
3. **Starte League of Legends**
4. **Gehe ins "Practice Tool":**
   - Training → Practice Tool
   - Wähle einen Champion
   - Starte das Spiel
5. **Beobachte die Live-Daten** in der Konsole
6. **Spiele ein bisschen herum:**
   - Töte Minions → CS wird aktualisiert
   - Levele auf → Stats ändern sich
   - Kaufe Items → Gold ändert sich

## 📚 Weiterführende Links

- [Riot Live Client Data API Dokumentation](https://developer.riotgames.com/docs/lol#game-client-api_live-client-data-api)
- [BE.League.Desktop Projekt](../BE.League.Desktop/)

## 🎉 Viel Erfolg!

Die Demo ist bereit! Starte einfach ein League of Legends Spiel und beobachte die Live-Daten.

Bei Fragen siehe die ausführlichen Dokumentationen in:
- `README.md`
- `USAGE_EXAMPLES.md`
- `ApiExamples.cs`

