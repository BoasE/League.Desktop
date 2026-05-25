using System.Text.Json;
using BE.League.Desktop.Models;

namespace BE.League.Desktop.GameClientApi;

/// <summary>
/// Typed reader for the Game Client API (Live Client Data API).
/// <para>
/// Wraps <see cref="IGameClientApi"/> and deserializes raw JSON responses into
/// strongly-typed models using <c>System.Text.Json</c> with AOT-compatible source generation.
/// </para>
/// <para>
/// Official documentation:
/// <see href="https://developer.riotgames.com/docs/lol#game-client-api">
/// Riot Games — Game Client API
/// </see>
/// </para>
/// </summary>
public sealed class GameClientApiReader
{
    private readonly IGameClientApi _api;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Exposes the underlying raw JSON API for direct access.
    /// </summary>
    public IGameClientApi Api => _api;

    public GameClientApiReader(IGameClientApi? api = null)
    {
        _api = api ?? new GameClientApiClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = LeagueJsonContext.Default
        };
    }

    public GameClientApiReader(Connection.LeagueDesktopOptions options)
    {
        _api = new GameClientApiClient(
            baseUrl: options.GameClientBaseUrl,
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
            System.Diagnostics.Debug.WriteLine($"Game Client API JSON deserialization error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Full snapshot of the current game state.
    /// <para>Endpoint: <c>GET /liveclientdata/allgamedata</c></para>
    /// </summary>
    public async Task<AllGameData?> GetAllGameDataAsync(CancellationToken ct = default)
    {
        var json = await _api.GetAllGameDataJsonAsync(ct);
        return Deserialize<AllGameData>(json);
    }

    /// <summary>
    /// Data about the local active player.
    /// <para>Endpoint: <c>GET /liveclientdata/activeplayer</c></para>
    /// </summary>
    public async Task<ActivePlayer?> GetActivePlayerAsync(CancellationToken ct = default)
    {
        var json = await _api.GetActivePlayerJsonAsync(ct);
        return Deserialize<ActivePlayer>(json);
    }

    /// <summary>
    /// Riot ID (name + tagline) of the local active player.
    /// <para>Endpoint: <c>GET /liveclientdata/activeplayername</c></para>
    /// </summary>
    public async Task<string?> GetActivePlayerNameAsync(CancellationToken ct = default)
    {
        return await _api.GetActivePlayerNameJsonAsync(ct);
    }

    /// <summary>
    /// Abilities of the local active player.
    /// <para>Endpoint: <c>GET /liveclientdata/activeplayerabilities</c></para>
    /// </summary>
    public async Task<Abilities?> GetActivePlayerAbilitiesAsync(CancellationToken ct = default)
    {
        var json = await _api.GetActivePlayerAbilitiesJsonAsync(ct);
        return Deserialize<Abilities>(json);
    }

    /// <summary>
    /// Rune configuration of the local active player.
    /// <para>Endpoint: <c>GET /liveclientdata/activeplayerrunes</c></para>
    /// </summary>
    public async Task<FullRunes?> GetActivePlayerRunesAsync(CancellationToken ct = default)
    {
        var json = await _api.GetActivePlayerRunesJsonAsync(ct);
        return Deserialize<FullRunes>(json);
    }

    /// <summary>
    /// List of all players in the game.
    /// <para>Endpoint: <c>GET /liveclientdata/playerlist</c></para>
    /// </summary>
    public async Task<List<Player>?> GetPlayerListAsync(CancellationToken ct = default)
    {
        var json = await _api.GetPlayerListJsonAsync(ct);
        return Deserialize<List<Player>>(json);
    }

    /// <summary>
    /// KDA and creep score for the specified player.
    /// <para>Endpoint: <c>GET /liveclientdata/playerscores?summonerName={summonerName}</c></para>
    /// </summary>
    public async Task<Scores?> GetPlayerScoresAsync(string summonerName, CancellationToken ct = default)
    {
        var json = await _api.GetPlayerScoresJsonAsync(summonerName, ct);
        return Deserialize<Scores>(json);
    }

    /// <summary>
    /// Summoner spells for the specified player.
    /// <para>Endpoint: <c>GET /liveclientdata/playersummonerspells?summonerName={summonerName}</c></para>
    /// </summary>
    public async Task<SummonerSpells?> GetPlayerSummonerSpellsAsync(string summonerName, CancellationToken ct = default)
    {
        var json = await _api.GetPlayerSummonerSpellsJsonAsync(summonerName, ct);
        return Deserialize<SummonerSpells>(json);
    }

    /// <summary>
    /// Main rune page for the specified player.
    /// <para>Endpoint: <c>GET /liveclientdata/playermainrunes?summonerName={summonerName}</c></para>
    /// </summary>
    public async Task<PlayerRunes?> GetPlayerMainRunesAsync(string summonerName, CancellationToken ct = default)
    {
        var json = await _api.GetPlayerMainRunesJsonAsync(summonerName, ct);
        return Deserialize<PlayerRunes>(json);
    }

    /// <summary>
    /// Current inventory items of the specified player.
    /// <para>Endpoint: <c>GET /liveclientdata/playeritems?summonerName={summonerName}</c></para>
    /// </summary>
    public async Task<List<Item>?> GetPlayerItemsAsync(string summonerName, CancellationToken ct = default)
    {
        var json = await _api.GetPlayerItemsJsonAsync(summonerName, ct);
        return Deserialize<List<Item>>(json);
    }

    /// <summary>
    /// All game events so far (kills, dragon, baron, etc.).
    /// <para>Endpoint: <c>GET /liveclientdata/eventdata</c></para>
    /// </summary>
    public async Task<Event?> GetEventDataAsync(CancellationToken ct = default)
    {
        var json = await _api.GetEventDataJsonAsync(ct);
        return Deserialize<Event>(json);
    }

    /// <summary>
    /// Game metadata: mode, map, current game time.
    /// <para>Endpoint: <c>GET /liveclientdata/gamestats</c></para>
    /// </summary>
    public async Task<GameData?> GetGameStatsAsync(CancellationToken ct = default)
    {
        var json = await _api.GetGameStatsJsonAsync(ct);
        return Deserialize<GameData>(json);
    }
}

