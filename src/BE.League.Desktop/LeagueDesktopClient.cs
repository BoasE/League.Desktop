using BE.League.Desktop.Connection;
using BE.League.Desktop.LiveClient;
using BE.League.Desktop.LcuClient;

namespace BE.League.Desktop;

/// <summary>
/// Unified client for League of Legends Desktop integration
/// Provides access to both Live Client Data API (in-game) and LCU API (client/lobby)
/// </summary>
public sealed class LeagueDesktopClient : IDisposable, ILeagueDesktopClient
{
    private readonly LiveClientApi _liveClient;
    private readonly LcuApi? _lcuClient;

    public LeagueDesktopClient(LeagueDesktopOptions? options = null)
    {
        options ??= new LeagueDesktopOptions
        {
            Connection = LeagueClientConnectionInfo.GetFromRunningClient(),
        };

        // Initialize Live Client (Port 2999 - always available if game is running)
        _liveClient = new LiveClientApi(
            options.LiveClientBaseUrl,
            options.Timeout);

        // Initialize LCU Client (dynamic port - requires lockfile)
        try
        {
            _lcuClient = new LcuApi(
                options.Connection,
                options.Timeout);
        }
        catch (InvalidOperationException)
        {
            // LCU not available (League Client not running)
            _lcuClient = null;
        }
    }

    /// <summary>
    /// Access to Live Client Data API (Port 2999 - In-Game)
    /// </summary>
    public LiveClientApi Live => _liveClient;

    /// <summary>
    /// Access to LCU API (Dynamic Port - Client/Lobby)
    /// Returns null if League Client is not running
    /// </summary>
    public LcuApi? Lcu => _lcuClient;

    // ========== Live Client Data API Methods (Delegated) ==========

    public Task<string?> GetAllGameDataJsonAsync(CancellationToken ct = default)
        => _liveClient.GetAllGameDataJsonAsync(ct);

    public Task<string?> GetActivePlayerJsonAsync(CancellationToken ct = default)
        => _liveClient.GetActivePlayerJsonAsync(ct);

    public Task<string?> GetActivePlayerNameJsonAsync(CancellationToken ct = default)
        => _liveClient.GetActivePlayerNameJsonAsync(ct);

    public Task<string?> GetActivePlayerAbilitiesJsonAsync(CancellationToken ct = default)
        => _liveClient.GetActivePlayerAbilitiesJsonAsync(ct);

    public Task<string?> GetActivePlayerRunesJsonAsync(CancellationToken ct = default)
        => _liveClient.GetActivePlayerRunesJsonAsync(ct);

    public Task<string?> GetPlayerListJsonAsync(CancellationToken ct = default)
        => _liveClient.GetPlayerListJsonAsync(ct);

    public Task<string?> GetPlayerScoresJsonAsync(string summonerName, CancellationToken ct = default)
        => _liveClient.GetPlayerScoresJsonAsync(summonerName, ct);

    public Task<string?> GetPlayerSummonerSpellsJsonAsync(string summonerName, CancellationToken ct = default)
        => _liveClient.GetPlayerSummonerSpellsJsonAsync(summonerName, ct);

    public Task<string?> GetPlayerMainRunesJsonAsync(string summonerName, CancellationToken ct = default)
        => _liveClient.GetPlayerMainRunesJsonAsync(summonerName, ct);

    public Task<string?> GetPlayerItemsJsonAsync(string summonerName, CancellationToken ct = default)
        => _liveClient.GetPlayerItemsJsonAsync(summonerName, ct);

    public Task<string?> GetEventDataJsonAsync(CancellationToken ct = default)
        => _liveClient.GetEventDataJsonAsync(ct);

    public Task<string?> GetGameStatsJsonAsync(CancellationToken ct = default)
        => _liveClient.GetGameStatsJsonAsync(ct);

    // ========== League Client API (LCU) Methods (Delegated) ==========

    public Task<string?> GetLobbyJsonAsync(CancellationToken ct = default)
        => _lcuClient?.GetLobbyJsonAsync(ct) ?? Task.FromResult<string?>(null);

    public Task<string?> GetChampSelectSessionJsonAsync(CancellationToken ct = default)
        => _lcuClient?.GetChampSelectSessionJsonAsync(ct) ?? Task.FromResult<string?>(null);

    public Task<string?> GetReadyCheckJsonAsync(CancellationToken ct = default)
        => _lcuClient?.GetReadyCheckJsonAsync(ct) ?? Task.FromResult<string?>(null);

    public Task<bool> AcceptReadyCheckAsync(CancellationToken ct = default)
        => _lcuClient?.AcceptReadyCheckAsync(ct) ?? Task.FromResult(false);

    public Task<bool> DeclineReadyCheckAsync(CancellationToken ct = default)
        => _lcuClient?.DeclineReadyCheckAsync(ct) ?? Task.FromResult(false);

    public void Dispose()
    {
        _liveClient?.Dispose();
        _lcuClient?.Dispose();
    }
}