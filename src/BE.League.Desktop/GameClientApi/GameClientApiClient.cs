namespace BE.League.Desktop.GameClientApi;

/// <summary>
/// HTTP client for the Game Client API (Live Client Data API).
/// <para>
/// Connects to <c>https://127.0.0.1:2999</c> while a League of Legends game is active.
/// No authentication is required. Accepts the game client's self-signed HTTPS certificate.
/// </para>
/// <para>
/// This client returns <c>null</c> for all methods when no game is running —
/// it never throws on network errors.
/// </para>
/// <para>
/// Official documentation:
/// <see href="https://developer.riotgames.com/docs/lol#game-client-api">
/// Riot Games — Game Client API
/// </see>
/// </para>
/// <para>
/// OpenAPI specification:
/// <see href="https://static.developer.riotgames.com/docs/lol/liveclientdata_sample.json">
/// Live Client Data API — Swagger sample
/// </see>
/// </para>
/// </summary>
public sealed class GameClientApiClient : IDisposable, IGameClientApi
{
    private readonly HttpClient _httpClient;

    /// <summary>Default base URL for the Game Client API.</summary>
    private const string DefaultBaseUrl = "https://127.0.0.1:2999";

    public GameClientApiClient(string? baseUrl = null, TimeSpan? timeout = null)
    {
        var handler = new HttpClientHandler
        {
            // The game client uses a self-signed certificate; bypass validation for localhost.
            ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                _ = sender;
                _ = certificate;
                _ = chain;
                _ = sslPolicyErrors;
                return true;
            }
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl ?? DefaultBaseUrl),
            Timeout = timeout ?? TimeSpan.FromSeconds(5)
        };
    }

    /// <summary>
    /// Full snapshot of the current game state (players, events, game data).
    /// <para>Endpoint: <c>GET /liveclientdata/allgamedata</c></para>
    /// </summary>
    public Task<string?> GetAllGameDataJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/allgamedata", ct);

    /// <summary>
    /// Data about the local active player (stats, gold, level, runes, abilities).
    /// <para>Endpoint: <c>GET /liveclientdata/activeplayer</c></para>
    /// </summary>
    public Task<string?> GetActivePlayerJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/activeplayer", ct);

    /// <summary>
    /// Riot ID (name + tagline) of the local active player.
    /// <para>Endpoint: <c>GET /liveclientdata/activeplayername</c></para>
    /// </summary>
    public Task<string?> GetActivePlayerNameJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/activeplayername", ct);

    /// <summary>
    /// Abilities (Q/W/E/R/Passive) of the local active player.
    /// <para>Endpoint: <c>GET /liveclientdata/activeplayerabilities</c></para>
    /// </summary>
    public Task<string?> GetActivePlayerAbilitiesJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/activeplayerabilities", ct);

    /// <summary>
    /// Rune configuration of the local active player.
    /// <para>Endpoint: <c>GET /liveclientdata/activeplayerrunes</c></para>
    /// </summary>
    public Task<string?> GetActivePlayerRunesJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/activeplayerrunes", ct);

    /// <summary>
    /// List of all players in the current game (both teams).
    /// <para>Endpoint: <c>GET /liveclientdata/playerlist</c></para>
    /// </summary>
    public Task<string?> GetPlayerListJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/playerlist", ct);

    /// <summary>
    /// KDA and creep score for the specified player.
    /// <para>Endpoint: <c>GET /liveclientdata/playerscores?summonerName={summonerName}</c></para>
    /// </summary>
    public Task<string?> GetPlayerScoresJsonAsync(string summonerName, CancellationToken ct = default)
        => GetJsonAsync($"/liveclientdata/playerscores?summonerName={Uri.EscapeDataString(summonerName)}", ct);

    /// <summary>
    /// Summoner spells for the specified player.
    /// <para>Endpoint: <c>GET /liveclientdata/playersummonerspells?summonerName={summonerName}</c></para>
    /// </summary>
    public Task<string?> GetPlayerSummonerSpellsJsonAsync(string summonerName, CancellationToken ct = default)
        => GetJsonAsync($"/liveclientdata/playersummonerspells?summonerName={Uri.EscapeDataString(summonerName)}", ct);

    /// <summary>
    /// Main rune page for the specified player.
    /// <para>Endpoint: <c>GET /liveclientdata/playermainrunes?summonerName={summonerName}</c></para>
    /// </summary>
    public Task<string?> GetPlayerMainRunesJsonAsync(string summonerName, CancellationToken ct = default)
        => GetJsonAsync($"/liveclientdata/playermainrunes?summonerName={Uri.EscapeDataString(summonerName)}", ct);

    /// <summary>
    /// Current inventory items of the specified player.
    /// <para>Endpoint: <c>GET /liveclientdata/playeritems?summonerName={summonerName}</c></para>
    /// </summary>
    public Task<string?> GetPlayerItemsJsonAsync(string summonerName, CancellationToken ct = default)
        => GetJsonAsync($"/liveclientdata/playeritems?summonerName={Uri.EscapeDataString(summonerName)}", ct);

    /// <summary>
    /// All game events that have occurred so far (kills, objectives, etc.).
    /// <para>Endpoint: <c>GET /liveclientdata/eventdata</c></para>
    /// </summary>
    public Task<string?> GetEventDataJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/liveclientdata/eventdata", ct);

    /// <summary>
    /// Game metadata: mode, map, current game time.
    /// <para>Endpoint: <c>GET /liveclientdata/gamestats</c></para>
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
            // Game not running or API not yet available
            return null;
        }
        catch (TaskCanceledException)
        {
            // Timeout or cancellation requested
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Game Client API error: {ex.Message}");
            return null;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

