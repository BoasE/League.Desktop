using BE.League.Desktop;
using BE.League.Desktop.Connection;
using BE.League.Desktop.LiveClient;

namespace BE.League.Desktop.Console;

/// <summary>
/// Beispiele für die Verwendung einzelner API-Endpunkte
/// </summary>
public static class ApiExamples
{
    /// <summary>
    /// Beispiel 1: Nur aktiven Spieler abrufen
    /// </summary>
    public static async Task GetActivePlayerExample()
    {
        var reader = new LiveClientObjectReader();
        
        var activePlayer = await reader.GetActivePlayerAsync();
        
        if (activePlayer != null)
        {
            System.Console.WriteLine($"Aktiver Spieler: {activePlayer.SummonerName}");
            System.Console.WriteLine($"Level: {activePlayer.Level}");
            System.Console.WriteLine($"Gold: {activePlayer.CurrentGold:F0}");
            
            if (activePlayer.ChampionStats != null)
            {
                System.Console.WriteLine($"HP: {activePlayer.ChampionStats.CurrentHealth:F0}/{activePlayer.ChampionStats.MaxHealth:F0}");
            }
        }
    }

    /// <summary>
    /// Beispiel 2: Alle Spieler im Spiel abrufen
    /// </summary>
    public static async Task GetAllPlayersExample()
    {
        var reader = new LiveClientObjectReader();
        
        var players = await reader.GetPlayerListAsync();
        
        if (players != null)
        {
            System.Console.WriteLine($"Spieler im Spiel: {players.Count}");
            
            foreach (var player in players)
            {
                System.Console.WriteLine($"- {player.ChampionName} ({player.SummonerName}) - Team: {player.Team}");
            }
        }
    }

    /// <summary>
    /// Beispiel 3: Spiel-Events abrufen
    /// </summary>
    public static async Task GetGameEventsExample()
    {
        var reader = new LiveClientObjectReader();
        
        var events = await reader.GetEventDataAsync();
        
        if (events?.EventsList != null)
        {
            System.Console.WriteLine($"Anzahl Events: {events.EventsList.Count}");
            
            // Nur die letzten 10 Events
            var recentEvents = events.EventsList.TakeLast(10);
            
            foreach (var evt in recentEvents)
            {
                var time = TimeSpan.FromSeconds(evt.EventTime);
                System.Console.WriteLine($"[{time:mm\\:ss}] {evt.EventName}");
            }
        }
    }

    /// <summary>
    /// Beispiel 4: Spieler-spezifische Informationen abrufen
    /// </summary>
    public static async Task GetPlayerSpecificDataExample(string summonerName)
    {
        var reader = new LiveClientObjectReader();
        
        // Scores
        var scores = await reader.GetPlayerScoresAsync(summonerName);
        if (scores != null)
        {
            System.Console.WriteLine($"KDA für {summonerName}: {scores.Kills}/{scores.Deaths}/{scores.Assists}");
            System.Console.WriteLine($"CS: {scores.CreepScore}");
        }
        
        // Items
        var items = await reader.GetPlayerItemsAsync(summonerName);
        if (items != null)
        {
            System.Console.WriteLine($"\nItems von {summonerName}:");
            foreach (var item in items)
            {
                System.Console.WriteLine($"  - Slot {item.Slot}: {item.DisplayName} (ID: {item.ItemId})");
            }
        }
        
        // Summoner Spells
        var spells = await reader.GetPlayerSummonerSpellsAsync(summonerName);
        if (spells != null)
        {
            System.Console.WriteLine($"\nBeschwörerzauber von {summonerName}:");
            System.Console.WriteLine($"  - {spells.SummonerSpellOne?.DisplayName}");
            System.Console.WriteLine($"  - {spells.SummonerSpellTwo?.DisplayName}");
        }
    }

    /// <summary>
    /// Beispiel 5: Spielstatistiken abrufen
    /// </summary>
    public static async Task GetGameStatsExample()
    {
        var reader = new LiveClientObjectReader();
        
        var gameData = await reader.GetGameStatsAsync();
        
        if (gameData != null)
        {
            System.Console.WriteLine($"Map: {gameData.MapName}");
            System.Console.WriteLine($"Spielmodus: {gameData.GameMode}");
            
            var gameTime = TimeSpan.FromSeconds(gameData.GameTime);
            System.Console.WriteLine($"Spielzeit: {gameTime:mm\\:ss}");
        }
    }

    /// <summary>
    /// Beispiel 6: Fähigkeiten des aktiven Spielers abrufen
    /// </summary>
    public static async Task GetActivePlayerAbilitiesExample()
    {
        var reader = new LiveClientObjectReader();
        
        var abilities = await reader.GetActivePlayerAbilitiesAsync();
        
        if (abilities != null)
        {
            System.Console.WriteLine("Fähigkeiten:");
            System.Console.WriteLine($"  Passiv: {abilities.Passive?.DisplayName}");
            System.Console.WriteLine($"  Q: {abilities.Q?.DisplayName} (Level {abilities.Q?.AbilityLevel})");
            System.Console.WriteLine($"  W: {abilities.W?.DisplayName} (Level {abilities.W?.AbilityLevel})");
            System.Console.WriteLine($"  E: {abilities.E?.DisplayName} (Level {abilities.E?.AbilityLevel})");
            System.Console.WriteLine($"  R: {abilities.R?.DisplayName} (Level {abilities.R?.AbilityLevel})");
        }
    }

    /// <summary>
    /// Beispiel 7: Runen des aktiven Spielers abrufen
    /// </summary>
    public static async Task GetActivePlayerRunesExample()
    {
        var reader = new LiveClientObjectReader();
        
        var runes = await reader.GetActivePlayerRunesAsync();
        
        if (runes != null)
        {
            System.Console.WriteLine("Runen-Konfiguration:");
            System.Console.WriteLine($"  Hauptrune: {runes.Keystone?.DisplayName}");
            System.Console.WriteLine($"  Primärer Baum: {runes.PrimaryRuneTree?.DisplayName}");
            System.Console.WriteLine($"  Sekundärer Baum: {runes.SecondaryRuneTree?.DisplayName}");
            
            if (runes.GeneralRunes != null)
            {
                System.Console.WriteLine($"  Weitere Runen: {runes.GeneralRunes.Count}");
            }
        }
    }

    /// <summary>
    /// Beispiel 8: Nur den Namen des aktiven Spielers abrufen
    /// </summary>
    public static async Task GetActivePlayerNameExample()
    {
        var reader = new LiveClientObjectReader();
        
        var name = await reader.GetActivePlayerNameAsync();
        
        if (!string.IsNullOrEmpty(name))
        {
            System.Console.WriteLine($"Aktiver Spieler: {name}");
        }
    }

    /// <summary>
    /// Beispiel 9: Mit benutzerdefinierten Optionen arbeiten
    /// </summary>
    public static async Task CustomOptionsExample()
    {
        // Benutzerdefinierte Optionen erstellen
        var options = new LeagueDesktopOptions
        {
            Connection = LeagueClientConnectionInfo.GetFromRunningClient(),
            Timeout = TimeSpan.FromSeconds(5) // Kürzerer Timeout
        };
        
        var reader = new LiveClientObjectReader(options);
        
        var activePlayer = await reader.GetActivePlayerAsync();
        
        if (activePlayer != null)
        {
            System.Console.WriteLine($"Spieler gefunden: {activePlayer.SummonerName}");
        }
    }

    /// <summary>
    /// Beispiel 10: Fehlerbehandlung
    /// </summary>
    public static async Task ErrorHandlingExample()
    {
        var reader = new LiveClientObjectReader();
        
        try
        {
            var allGameData = await reader.GetAllGameDataAsync();
            
            if (allGameData == null)
            {
                System.Console.WriteLine("Kein aktives Spiel gefunden oder API nicht verfügbar");
                return;
            }
            
            System.Console.WriteLine("Spieldaten erfolgreich abgerufen!");
        }
        catch (OperationCanceledException)
        {
            System.Console.WriteLine("Operation wurde abgebrochen");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Fehler beim Abrufen der Daten: {ex.Message}");
        }
    }

    /// <summary>
    /// Beispiel 11: Kontinuierliches Monitoring mit CancellationToken
    /// </summary>
    public static async Task ContinuousMonitoringExample(CancellationToken cancellationToken)
    {
        var reader = new LiveClientObjectReader();
        
        System.Console.WriteLine("Starte kontinuierliches Monitoring...");
        System.Console.WriteLine("Drücke Strg+C zum Beenden");
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var gameData = await reader.GetGameStatsAsync(cancellationToken);
                
                if (gameData != null)
                {
                    var time = TimeSpan.FromSeconds(gameData.GameTime);
                    System.Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Spielzeit: {time:mm\\:ss}");
                }
                
                await Task.Delay(5000, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                System.Console.WriteLine("Monitoring beendet");
                break;
            }
        }
    }

    /// <summary>
    /// Beispiel 12: Team-Statistiken berechnen
    /// </summary>
    public static async Task CalculateTeamStatsExample()
    {
        var reader = new LiveClientObjectReader();
        
        var players = await reader.GetPlayerListAsync();
        
        if (players != null)
        {
            var blueTeam = players.Where(p => p.Team == "ORDER").ToList();
            var redTeam = players.Where(p => p.Team == "CHAOS").ToList();
            
            // Berechne Team-Kills
            var blueKills = blueTeam.Sum(p => p.Scores?.Kills ?? 0);
            var redKills = redTeam.Sum(p => p.Scores?.Kills ?? 0);
            
            // Berechne Team-Gold (nur für Spieler, die wir detailliert abrufen können)
            System.Console.WriteLine("TEAM STATISTIKEN");
            System.Console.WriteLine($"Blaues Team - Kills: {blueKills}");
            System.Console.WriteLine($"Rotes Team - Kills: {redKills}");
            
            // Tote Spieler pro Team
            var blueDeaths = blueTeam.Count(p => p.IsDead);
            var redDeaths = redTeam.Count(p => p.IsDead);
            
            System.Console.WriteLine($"\nTote Spieler:");
            System.Console.WriteLine($"Blaues Team: {blueDeaths}/5");
            System.Console.WriteLine($"Rotes Team: {redDeaths}/5");
        }
    }
}

