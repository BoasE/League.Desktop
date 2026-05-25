namespace BE.League.Desktop.GameClientApi;

/// <summary>
/// Interface for the Game Client API (Live Client Data API).
/// <para>
/// The Game Client API runs on <b>fixed port 2999</b> and is available <b>only during an active game</b>.
/// No authentication is required. The API provides real-time game state including player stats,
/// items, abilities, events, and match data.
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
public interface IGameClientApi
{
    /// <summary>
    /// Full snapshot of the current game state (players, events, game data).
    /// <para>Endpoint: <c>GET /liveclientdata/allgamedata</c></para>
    /// </summary>
    Task<string?> GetAllGameDataJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Data about the local active player (stats, gold, level, runes, abilities).
    /// <para>Endpoint: <c>GET /liveclientdata/activeplayer</c></para>
    /// </summary>
    Task<string?> GetActivePlayerJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Riot ID (name + tagline) of the local active player.
    /// <para>Endpoint: <c>GET /liveclientdata/activeplayername</c></para>
    /// </summary>
    Task<string?> GetActivePlayerNameJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Abilities (Q/W/E/R/Passive) of the local active player.
    /// <para>Endpoint: <c>GET /liveclientdata/activeplayerabilities</c></para>
    /// </summary>
    Task<string?> GetActivePlayerAbilitiesJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Rune configuration of the local active player.
    /// <para>Endpoint: <c>GET /liveclientdata/activeplayerrunes</c></para>
    /// </summary>
    Task<string?> GetActivePlayerRunesJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// List of all players in the current game (both teams).
    /// <para>Endpoint: <c>GET /liveclientdata/playerlist</c></para>
    /// </summary>
    Task<string?> GetPlayerListJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// KDA and creep score for the specified player.
    /// <para>Endpoint: <c>GET /liveclientdata/playerscores?summonerName={summonerName}</c></para>
    /// </summary>
    Task<string?> GetPlayerScoresJsonAsync(string summonerName, CancellationToken ct = default);

    /// <summary>
    /// Summoner spells (e.g. Flash, Ignite) for the specified player.
    /// <para>Endpoint: <c>GET /liveclientdata/playersummonerspells?summonerName={summonerName}</c></para>
    /// </summary>
    Task<string?> GetPlayerSummonerSpellsJsonAsync(string summonerName, CancellationToken ct = default);

    /// <summary>
    /// Main rune page (keystone + secondary tree) for the specified player.
    /// <para>Endpoint: <c>GET /liveclientdata/playermainrunes?summonerName={summonerName}</c></para>
    /// </summary>
    Task<string?> GetPlayerMainRunesJsonAsync(string summonerName, CancellationToken ct = default);

    /// <summary>
    /// Current inventory items of the specified player.
    /// <para>Endpoint: <c>GET /liveclientdata/playeritems?summonerName={summonerName}</c></para>
    /// </summary>
    Task<string?> GetPlayerItemsJsonAsync(string summonerName, CancellationToken ct = default);

    /// <summary>
    /// All game events that have occurred so far (kills, objectives, etc.).
    /// <para>Endpoint: <c>GET /liveclientdata/eventdata</c></para>
    /// </summary>
    Task<string?> GetEventDataJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Game metadata: mode, map, current game time.
    /// <para>Endpoint: <c>GET /liveclientdata/gamestats</c></para>
    /// </summary>
    Task<string?> GetGameStatsJsonAsync(CancellationToken ct = default);

    void Dispose();
}

