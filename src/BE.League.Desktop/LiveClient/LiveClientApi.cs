namespace BE.League.Desktop.LiveClient;

/// <summary>
/// API client for League of Legends Live Client Data API (Port 2999)
/// Provides real-time game data while a match is running
/// No authentication required
/// </summary>
public class LiveClientApi : IDisposable
{
    private readonly HttpClient _httpClient;
    private const string DefaultBaseUrl = "https://127.0.0.1:2999";

    public LiveClientApi(string? baseUrl = null, TimeSpan? timeout = null)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                _ = sender;
                _ = certificate;
                _ = chain;
                _ = sslPolicyErrors;
                return true; // Accept self-signed certificates
            }
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl ?? DefaultBaseUrl),
            Timeout = timeout ?? TimeSpan.FromSeconds(5)
        };
    }

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
    /// List of all active players
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
    /// All Events that occurred in the game until now
    /// </summary>
    public Task<string?> GetEventDataJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/eventdata", ct);

    /// <summary>
    /// Common gamestats of the current game
    /// </summary>
    public Task<string?> GetGameStatsJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/gamestats", ct);

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
            // Game not running or API not available
            return null;
        }
        catch (TaskCanceledException)
        {
            // Timeout or Cancellation
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Live Client Data API Error: {ex.Message}");
            return null;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

