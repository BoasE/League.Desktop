using System.Text.Json;
using BE.League.Desktop.Connection;
using BE.League.Desktop.Models;

namespace BE.League.Desktop.LiveClient;

/// <summary>
/// Object reader for Live Client Data API (Port 2999)
/// Deserializes JSON responses into strongly-typed models
/// </summary>
public sealed class LiveClientObjectReader
{
    private readonly ILiveClientApi _api;
    private readonly JsonSerializerOptions _jsonOptions;

    public ILiveClientApi Api => _api;
    
    public LiveClientObjectReader(ILiveClientApi? api = null)
    {
        _api = api ?? new LiveClientApi();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = LeagueJsonContext.Default
        };
    }

    public LiveClientObjectReader(LeagueDesktopOptions options)
    {
        _api = new LiveClientApi(
            baseUrl: options.LiveClientBaseUrl,
            timeout: options.Timeout);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = LeagueJsonContext.Default
        };
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

    public async Task<AllGameData?> GetAllGameDataAsync(CancellationToken ct = default)
    {
        var json = await _api.GetAllGameDataJsonAsync(ct);
        return Deserialize<AllGameData>(json);
    }

    public async Task<ActivePlayer?> GetActivePlayerAsync(CancellationToken ct = default)
    {
        var json = await _api.GetActivePlayerJsonAsync(ct);
        return Deserialize<ActivePlayer>(json);
    }

    public async Task<string?> GetActivePlayerNameAsync(CancellationToken ct = default)
    {
        return await _api.GetActivePlayerNameJsonAsync(ct);
    }

    public async Task<Abilities?> GetActivePlayerAbilitiesAsync(CancellationToken ct = default)
    {
        var json = await _api.GetActivePlayerAbilitiesJsonAsync(ct);
        return Deserialize<Abilities>(json);
    }

    public async Task<FullRunes?> GetActivePlayerRunesAsync(CancellationToken ct = default)
    {
        var json = await _api.GetActivePlayerRunesJsonAsync(ct);
        return Deserialize<FullRunes>(json);
    }

    public async Task<List<Player>?> GetPlayerListAsync(CancellationToken ct = default)
    {
        var json = await _api.GetPlayerListJsonAsync(ct);
        return Deserialize<List<Player>>(json);
    }

    public async Task<Scores?> GetPlayerScoresAsync(string summonerName, CancellationToken ct = default)
    {
        var json = await _api.GetPlayerScoresJsonAsync(summonerName, ct);
        return Deserialize<Scores>(json);
    }

    public async Task<SummonerSpells?> GetPlayerSummonerSpellsAsync(string summonerName, CancellationToken ct = default)
    {
        var json = await _api.GetPlayerSummonerSpellsJsonAsync(summonerName, ct);
        return Deserialize<SummonerSpells>(json);
    }

    public async Task<PlayerRunes?> GetPlayerMainRunesAsync(string summonerName, CancellationToken ct = default)
    {
        var json = await _api.GetPlayerMainRunesJsonAsync(summonerName, ct);
        return Deserialize<PlayerRunes>(json);
    }

    public async Task<List<Item>?> GetPlayerItemsAsync(string summonerName, CancellationToken ct = default)
    {
        var json = await _api.GetPlayerItemsJsonAsync(summonerName, ct);
        return Deserialize<List<Item>>(json);
    }

    public async Task<Event?> GetEventDataAsync(CancellationToken ct = default)
    {
        var json = await _api.GetEventDataJsonAsync(ct);
        return Deserialize<Event>(json);
    }

    public async Task<GameData?> GetGameStatsAsync(CancellationToken ct = default)
    {
        var json = await _api.GetGameStatsJsonAsync(ct);
        return Deserialize<GameData>(json);
    }
}