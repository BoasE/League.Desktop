using BE.League.Desktop.Connection;

namespace BE.League.Desktop;

/// <summary>
/// Wrapper for live DesktopClient Data access (Default Port 2999)
/// Realtime Data while DesktopClient / game is running
/// Also supports League Client API (LCU) for lobby and matchmaking
/// </summary>
/// <see cref="https://developer.riotgames.com/docs/lol#game-client-api_live-client-data-api"/>
public sealed class LeagueDesktopClient : IDisposable, ILeagueDesktopClient
{
    private readonly HttpClient _httpClient;
    private readonly LeagueClientConnectionInfo? _lcuConnection;

    public LeagueDesktopClient(LeagueDesktopOptions? options = null)
    {
        options ??= new LeagueDesktopOptions
        {
            Connection = LeagueClientConnectionInfo.GetFromRunningClient(),
        };

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                _ = sender;
                _ = certificate;
                _ = chain;
                _ = sslPolicyErrors;
                return true; // Self-signed certificates akzeptieren
            }
        };

        string url = options.Connection.GetBaseUrl();
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(url),
            Timeout = options.Timeout
        };

   
    }

    // ========== Live Client Data API Methods ==========

    /// <summary>
    /// All data of the current game state
    /// </summary>
    public Task<string?> GetAllGameDataJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/allgamedata", ct);

    /// <summary>
    /// Data about the current active player 
    /// </summary>
    public Task<string?> GetActivePlayerJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/activeplayer", ct);

    /// <summary>
    /// Name of the current active player
    /// </summary>
    public Task<string?> GetActivePlayerNameJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/activeplayername", ct);

    /// <summary>
    /// Abilities of the current active player
    /// </summary>
    public Task<string?> GetActivePlayerAbilitiesJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/activeplayerabilities", ct);

    /// <summary>
    /// Runes of the current active player
    /// </summary>
    public Task<string?> GetActivePlayerRunesJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/activeplayerrunes", ct);

    /// <summary>
    /// List of all active PLayers
    /// </summary>
    public Task<string?> GetPlayerListJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/playerlist", ct);

    /// <summary>
    /// Current Score
    /// </summary>
    public Task<string?> GetPlayerScoresJsonAsync(string summonerName, CancellationToken ct = default)
        => GetJsonAsync($"/liveclientdata/playerscores?summonerName={Uri.EscapeDataString(summonerName)}", ct);

    /// <summary>
    /// Summoner Spells of the given player
    /// </summary>
    public Task<string?> GetPlayerSummonerSpellsJsonAsync(string summonerName, CancellationToken ct = default)
        => GetJsonAsync($"/liveclientdata/playersummonerspells?summonerName={Uri.EscapeDataString(summonerName)}", ct);

    /// <summary>
    /// Main Runes of the given player
    /// </summary>
    public Task<string?> GetPlayerMainRunesJsonAsync(string summonerName, CancellationToken ct = default)
        => GetJsonAsync($"/liveclientdata/playermainrunes?summonerName={Uri.EscapeDataString(summonerName)}", ct);

    /// <summary>
    /// Items of the given player
    /// </summary>
    public Task<string?> GetPlayerItemsJsonAsync(string summonerName, CancellationToken ct = default)
        => GetJsonAsync($"/liveclientdata/playeritems?summonerName={Uri.EscapeDataString(summonerName)}", ct);

    /// <summary>
    /// All Events that occured in the game until now
    /// </summary>
    public Task<string?> GetEventDataJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/eventdata", ct);

    /// <summary>
    /// Common gamestats of the current game
    /// </summary>
    public Task<string?> GetGameStatsJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/gamestats", ct);

    // ========== League Client API (LCU) Methods ==========

    /// <summary>
    /// Get current lobby information
    /// </summary>
    public Task<string?> GetLobbyJsonAsync(CancellationToken ct = default)
        => GetLcuJsonAsync("/lol-lobby/v2/lobby", ct);

    /// <summary>
    /// Get current champion select session
    /// </summary>
    public Task<string?> GetChampSelectSessionJsonAsync(CancellationToken ct = default)
        => GetLcuJsonAsync("/lol-champ-select/v1/session", ct);

    /// <summary>
    /// Get ready check state
    /// </summary>
    public Task<string?> GetReadyCheckJsonAsync(CancellationToken ct = default)
        => GetLcuJsonAsync("/lol-matchmaking/v1/ready-check", ct);

    /// <summary>
    /// Accept ready check (when game is found)
    /// </summary>
    public async Task<bool> AcceptReadyCheckAsync(CancellationToken ct = default)
    {
        if (_httpClient == null)
        {
            return false;
        }

        try
        {
            var response = await _httpClient.PostAsync(
                "/lol-matchmaking/v1/ready-check/accept",
                null,
                ct);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Decline ready check
    /// </summary>
    public async Task<bool> DeclineReadyCheckAsync(CancellationToken ct = default)
    {
        if (_httpClient == null)
        {
            return false;
        }

        try
        {
            var response = await _httpClient.PostAsync(
                "/lol-matchmaking/v1/ready-check/decline",
                null,
                ct);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // ========== Private Helper Methods ==========

    private async Task<string?> GetJsonAsync(string endpoint, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint, ct);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return json;
        }
        catch (HttpRequestException)
        {
            // Spiel läuft nicht oder API nicht verfügbar
            return null;
        }
        catch (TaskCanceledException)
        {
            // Timeout oder Cancellation
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Live Client Data API Error: {ex.Message}");
            return null;
        }
    }

    private async Task<string?> GetLcuJsonAsync(string endpoint, CancellationToken ct)
    {
        if (_httpClient == null)
        {
            return null;
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, ct);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return json;
        }
        catch (HttpRequestException)
        {
            // LCU nicht verfügbar
            return null;
        }
        catch (TaskCanceledException)
        {
            // Timeout oder Cancellation
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LCU API Error: {ex.Message}");
            return null;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();

    }
}