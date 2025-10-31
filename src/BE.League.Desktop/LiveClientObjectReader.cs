using System.Text.Json;
using BE.League.Desktop.Connection;
using BE.League.Desktop.Models;

namespace BE.League.Desktop;

/// <summary>
/// Verantwortlich für die Deserialisierung von JSON-Daten in Objekt-Modelle
/// </summary>
public sealed class LiveClientObjectReader
{
    private readonly ILeagueDesktopClient _gateway;

    private readonly JsonSerializerOptions _jsonOptions;

    public LiveClientObjectReader(ILeagueDesktopClient gateway)
    {
        this._gateway = gateway;
        this._jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = LeagueJsonContext.Default
        };
    }

    public LiveClientObjectReader(LeagueDesktopOptions? options = null)
        : this(new LeagueDesktopClient(options))
    {
    }

    public LiveClientObjectReader() : this(new LeagueDesktopClient())
    {
    }


    private T? Deserialize<T>(string? json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) return null;

        try
        {
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Live Client Data API JSON Error: {ex.Message}");
            return null;
        }
    }

    // ========== Live Client Data API Methods ==========

    public async Task<AllGameData?> GetAllGameDataAsync(CancellationToken ct = default)
    {
        var json = await _gateway.GetAllGameDataJsonAsync(ct);
        return Deserialize<AllGameData>(json);
    }


    public async Task<ActivePlayer?> GetActivePlayerAsync(CancellationToken ct = default)
    {
        var json = await _gateway.GetActivePlayerJsonAsync(ct);
        return Deserialize<ActivePlayer>(json);
    }

    public async Task<string?> GetActivePlayerNameAsync(CancellationToken ct = default)
    {
        return await _gateway.GetActivePlayerNameJsonAsync(ct);
    }

    public async Task<Abilities?> GetActivePlayerAbilitiesAsync(CancellationToken ct = default)
    {
        var json = await _gateway.GetActivePlayerAbilitiesJsonAsync(ct);
        return Deserialize<Abilities>(json);
    }

    public async Task<FullRunes?> GetActivePlayerRunesAsync(CancellationToken ct = default)
    {
        var json = await _gateway.GetActivePlayerRunesJsonAsync(ct);
        return Deserialize<FullRunes>(json);
    }


    public async Task<List<Player>?> GetPlayerListAsync(CancellationToken ct = default)
    {
        var json = await _gateway.GetPlayerListJsonAsync(ct);
        return Deserialize<List<Player>>(json);
    }


    public async Task<Scores?> GetPlayerScoresAsync(string summonerName, CancellationToken ct = default)
    {
        var json = await _gateway.GetPlayerScoresJsonAsync(summonerName, ct);
        return Deserialize<Scores>(json);
    }


    public async Task<SummonerSpells?> GetPlayerSummonerSpellsAsync(string summonerName, CancellationToken ct = default)
    {
        var json = await _gateway.GetPlayerSummonerSpellsJsonAsync(summonerName, ct);
        return Deserialize<SummonerSpells>(json);
    }


    public async Task<PlayerRunes?> GetPlayerMainRunesAsync(string summonerName, CancellationToken ct = default)
    {
        var json = await _gateway.GetPlayerMainRunesJsonAsync(summonerName, ct);
        return Deserialize<PlayerRunes>(json);
    }


    public async Task<List<Item>?> GetPlayerItemsAsync(string summonerName, CancellationToken ct = default)
    {
        var json = await _gateway.GetPlayerItemsJsonAsync(summonerName, ct);
        return Deserialize<List<Item>>(json);
    }


    public async Task<Event?> GetEventDataAsync(CancellationToken ct = default)
    {
        var json = await _gateway.GetEventDataJsonAsync(ct);
        return Deserialize<Event>(json);
    }


    public async Task<GameData?> GetGameStatsAsync(CancellationToken ct = default)
    {
        var json = await _gateway.GetGameStatsJsonAsync(ct);
        return Deserialize<GameData>(json);
    }

    // ========== League Client API (LCU) Methods ==========

    /// <summary>
    /// Get current lobby information
    /// </summary>
    public async Task<Lobby?> GetLobbyAsync(CancellationToken ct = default)
    {
        var json = await _gateway.GetLobbyJsonAsync(ct);
        return Deserialize<Lobby>(json);
    }

    /// <summary>
    /// Get current champion select session
    /// </summary>
    public async Task<ChampSelectSession?> GetChampSelectSessionAsync(CancellationToken ct = default)
    {
        var json = await _gateway.GetChampSelectSessionJsonAsync(ct);
        return Deserialize<ChampSelectSession>(json);
    }

    /// <summary>
    /// Get ready check state
    /// </summary>
    public async Task<ReadyCheckDto?> GetReadyCheckAsync(CancellationToken ct = default)
    {
        var json = await _gateway.GetReadyCheckJsonAsync(ct);
        return Deserialize<ReadyCheckDto>(json);
    }

    /// <summary>
    /// Accept ready check (when game is found)
    /// </summary>
    public async Task<bool> AcceptReadyCheckAsync(CancellationToken ct = default)
    {
        return await _gateway.AcceptReadyCheckAsync(ct);
    }

    /// <summary>
    /// Decline ready check
    /// </summary>
    public async Task<bool> DeclineReadyCheckAsync(CancellationToken ct = default)
    {
        return await _gateway.DeclineReadyCheckAsync(ct);
    }
}