using BE.League.Desktop.LcuClient;
using BE.League.Desktop.LiveClient;

namespace BE.League.Desktop.Console;

/// <summary>
/// Beispiele für die Verwendung der Lobby- und Ready-Check-Funktionen
/// </summary>
public static class LobbyExamples
{
    /// <summary>
    /// Beispiel 1: Lobby-Informationen abrufen
    /// </summary>
    public static async Task GetLobbyExample()
    {
        var reader = new LcuClient.LcuObjectReader();


        var lobby = await reader.GetLobbyAsync();

        if (lobby != null)
        {
            System.Console.WriteLine("=== LOBBY INFORMATIONEN ===");
            System.Console.WriteLine($"Anzahl Mitglieder: {lobby.Members.Length}");

            System.Console.WriteLine("\nMitglieder:");
            foreach (var member in lobby.Members)
            {
                System.Console.WriteLine($"  - {member.SummonerName}");
            }

            if (lobby.GameConfig != null)
            {
                System.Console.WriteLine($"\nQueue ID: {lobby.GameConfig.QueueId}");
            }
        }
        else
        {
            System.Console.WriteLine("Keine Lobby aktiv oder League Client nicht gefunden");
        }
    }

    /// <summary>
    /// Beispiel 2: Champion-Select-Session abrufen
    /// </summary>
    public static async Task GetChampSelectExample()
    {
        var reader = new LcuObjectReader();

        var session = await reader.GetChampSelectSessionAsync();

        if (session != null)
        {
            System.Console.WriteLine("=== CHAMPION SELECT ===");
            System.Console.WriteLine($"Phase: {session.Timer?.Phase}");
            System.Console.WriteLine($"Lokale Spieler Cell ID: {session.LocalPlayerCellId}");

            if (session.MyTeam.Count > 0)
            {
                System.Console.WriteLine("\nMein Team:");
                foreach (var member in session.MyTeam)
                {
                    System.Console.WriteLine($"  Cell {member.CellId}: Champion {member.ChampionId}");

                    if (member.ChampionPickIntent.HasValue)
                    {
                        System.Console.WriteLine($"    Intent: Champion {member.ChampionPickIntent.Value}");
                    }
                }
            }
        }
        else
        {
            System.Console.WriteLine("Keine Champion-Select-Session aktiv");
        }
    }

    /// <summary>
    /// Beispiel 3: Ready-Check-Status abrufen
    /// </summary>
    public static async Task GetReadyCheckStatusExample()
    {
        var reader = new LcuObjectReader();

        var readyCheck = await reader.GetReadyCheckAsync();

        if (readyCheck != null)
        {
            System.Console.WriteLine("=== READY CHECK ===");
            System.Console.WriteLine($"Status: {readyCheck.State}");
        }
        else
        {
            System.Console.WriteLine("Kein Ready Check aktiv");
        }
    }

    /// <summary>
    /// Beispiel 4: Automatisch Ready Check akzeptieren
    /// </summary>
    public static async Task AutoAcceptReadyCheckExample()
    {
        var reader = new LcuObjectReader();

        System.Console.WriteLine("Warte auf Ready Check...");
        System.Console.WriteLine("(Drücke Strg+C zum Beenden)");

        while (true)
        {
            try
            {
                var readyCheck = await reader.GetReadyCheckAsync();

                if (readyCheck != null && readyCheck.State == "InProgress")
                {
                    System.Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Ready Check gefunden! Akzeptiere...");

                    var accepted = await reader.AcceptReadyCheckAsync();

                    if (accepted)
                    {
                        System.Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✓ Ready Check akzeptiert!");
                    }
                    else
                    {
                        System.Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✗ Konnte Ready Check nicht akzeptieren");
                    }

                    // Warte 5 Sekunden, bevor weiter geprüft wird
                    await Task.Delay(5000);
                }
                else
                {
                    // Überprüfe alle 2 Sekunden
                    await Task.Delay(2000);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Fehler: {ex.Message}");
                await Task.Delay(2000);
            }
        }
    }

    /// <summary>
    /// Beispiel 5: Lobby-Monitor mit kontinuierlicher Aktualisierung
    /// </summary>
    public static async Task MonitorLobbyExample(CancellationToken cancellationToken)
    {
        var reader = new LcuObjectReader();

        System.Console.WriteLine("=== LOBBY MONITOR ===");
        System.Console.WriteLine("Überwache Lobby und Ready Check...\n");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Prüfe Lobby
                var lobby = await reader.GetLobbyAsync(cancellationToken);

                if (lobby != null)
                {
                    System.Console.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss}] Lobby aktiv mit {lobby.Members.Length} Mitgliedern");

                    // Prüfe Ready Check
                    var readyCheck = await reader.GetReadyCheckAsync(cancellationToken);

                    if (readyCheck != null && readyCheck.State == "InProgress")
                    {
                        System.Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss}] 🔔 READY CHECK! Akzeptiere automatisch...");

                        var accepted = await reader.AcceptReadyCheckAsync(cancellationToken);

                        if (accepted)
                        {
                            System.Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✓ Ready Check akzeptiert!");
                        }
                    }
                }

                await Task.Delay(2000, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Fehler: {ex.Message}");
                await Task.Delay(2000, cancellationToken);
            }
        }

        System.Console.WriteLine("Monitor beendet");
    }

    /// <summary>
    /// Beispiel 6: Vollständige Lobby-Details anzeigen
    /// </summary>
    public static async Task ShowLobbyDetailsExample()
    {
        var reader = new LcuObjectReader();

        System.Console.WriteLine("=== VOLLSTÄNDIGE LOBBY-DETAILS ===\n");

        // Lobby-Informationen
        var lobby = await reader.GetLobbyAsync();

        if (lobby != null)
        {
            System.Console.WriteLine("┌─── LOBBY ──────────────────────────────────────────┐");
            System.Console.WriteLine($"│ Mitglieder: {lobby.Members.Length,-37} │");

            if (lobby.GameConfig?.QueueId.HasValue == true)
            {
                System.Console.WriteLine($"│ Queue ID: {lobby.GameConfig.QueueId.Value,-39} │");
            }

            System.Console.WriteLine("│                                                    │");
            System.Console.WriteLine("│ Mitglieder:                                        │");

            foreach (var member in lobby.Members)
            {
                System.Console.WriteLine($"│   • {member.SummonerName,-45} │");
            }

            System.Console.WriteLine("└────────────────────────────────────────────────────┘");
        }
        else
        {
            System.Console.WriteLine("Keine Lobby gefunden");
            return;
        }

        System.Console.WriteLine();

        // Champion Select (falls aktiv)
        var champSelect = await reader.GetChampSelectSessionAsync();

        if (champSelect != null)
        {
            System.Console.WriteLine("┌─── CHAMPION SELECT ────────────────────────────────┐");
            System.Console.WriteLine($"│ Phase: {champSelect.Timer?.Phase,-41} │");
            System.Console.WriteLine($"│ Lokale Cell ID: {champSelect.LocalPlayerCellId,-34} │");
            System.Console.WriteLine("│                                                    │");

            if (champSelect.MyTeam.Count > 0)
            {
                System.Console.WriteLine("│ Team:                                              │");

                foreach (var member in champSelect.MyTeam)
                {
                    var isLocal = member.CellId == champSelect.LocalPlayerCellId ? "👤" : "  ";
                    System.Console.WriteLine(
                        $"│ {isLocal} Cell {member.CellId}: Champion ID {member.ChampionId,-20} │");
                }
            }

            System.Console.WriteLine("└────────────────────────────────────────────────────┘");
        }

        System.Console.WriteLine();

        // Ready Check (falls aktiv)
        var readyCheck = await reader.GetReadyCheckAsync();

        if (readyCheck != null)
        {
            System.Console.WriteLine("┌─── READY CHECK ────────────────────────────────────┐");
            System.Console.WriteLine($"│ Status: {readyCheck.State,-40} │");
            System.Console.WriteLine("└────────────────────────────────────────────────────┘");
        }
    }

    /// <summary>
    /// Beispiel 7: Ready Check Decline (ablehnen)
    /// </summary>
    public static async Task DeclineReadyCheckExample()
    {
        var reader = new LcuObjectReader();

        var readyCheck = await reader.GetReadyCheckAsync();

        if (readyCheck != null && readyCheck.State == "InProgress")
        {
            System.Console.WriteLine("Ready Check aktiv. Lehne ab...");

            var declined = await reader.DeclineReadyCheckAsync();

            if (declined)
            {
                System.Console.WriteLine("✓ Ready Check abgelehnt");
            }
            else
            {
                System.Console.WriteLine("✗ Konnte Ready Check nicht ablehnen");
            }
        }
        else
        {
            System.Console.WriteLine("Kein aktiver Ready Check zum Ablehnen");
        }
    }

    /// <summary>
    /// Beispiel 8: Kombiniertes Monitoring (Lobby + Game)
    /// </summary>
    public static async Task CombinedMonitorExample(CancellationToken cancellationToken)
    {
        var reader = new LcuObjectReader();
        var live = new LiveClientObjectReader();

        System.Console.WriteLine("=== KOMBINIERTES MONITORING ===");
        System.Console.WriteLine("Überwache Lobby, Ready Check und Live Game...\n");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Prüfe auf laufendes Spiel
                var gameData = await live.GetAllGameDataAsync(cancellationToken);

                if (gameData != null)
                {
                    // Im Spiel
                    var gameTime = TimeSpan.FromSeconds(gameData.GameData?.GameTime ?? 0);
                    var playerName = gameData.ActivePlayer?.SummonerName;

                    System.Console.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss}] 🎮 IM SPIEL - {playerName} - Zeit: {gameTime:mm\\:ss}");
                }
                else
                {
                    // Nicht im Spiel - Prüfe Lobby
                    var lobby = await reader.GetLobbyAsync(cancellationToken);

                    if (lobby != null)
                    {
                        System.Console.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss}] 🏠 In Lobby mit {lobby.Members.Length} Mitgliedern");

                        // Prüfe Ready Check
                        var readyCheck = await reader.GetReadyCheckAsync(cancellationToken);

                        if (readyCheck != null && readyCheck.State == "InProgress")
                        {
                            System.Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 🔔 READY CHECK! Akzeptiere...");

                            var accepted = await reader.AcceptReadyCheckAsync(cancellationToken);

                            if (accepted)
                            {
                                System.Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ✓ Akzeptiert!");
                            }
                        }
                    }
                    else
                    {
                        System.Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ⏸️ Warte auf Lobby oder Spiel...");
                    }
                }

                await Task.Delay(3000, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ❌ Fehler: {ex.Message}");
                await Task.Delay(3000, cancellationToken);
            }
        }

        System.Console.WriteLine("\nMonitoring beendet");
    }
}