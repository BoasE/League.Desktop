namespace BE.League.Desktop.LiveClient;

public interface ILiveClientApi
{
    /// <summary>
    /// All data of the current game state
    /// </summary>
    Task<string?> GetAllGameDataJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Data about the current active player 
    /// </summary>
    Task<string?> GetActivePlayerJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Name of the current active player
    /// </summary>
    Task<string?> GetActivePlayerNameJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Abilities of the current active player
    /// </summary>
    Task<string?> GetActivePlayerAbilitiesJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Runes of the current active player
    /// </summary>
    Task<string?> GetActivePlayerRunesJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// List of all active players
    /// </summary>
    Task<string?> GetPlayerListJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Current Score
    /// </summary>
    Task<string?> GetPlayerScoresJsonAsync(string summonerName, CancellationToken ct = default);

    /// <summary>
    /// Summoner Spells of the given player
    /// </summary>
    Task<string?> GetPlayerSummonerSpellsJsonAsync(string summonerName, CancellationToken ct = default);

    /// <summary>
    /// Main Runes of the given player
    /// </summary>
    Task<string?> GetPlayerMainRunesJsonAsync(string summonerName, CancellationToken ct = default);

    /// <summary>
    /// Items of the given player
    /// </summary>
    Task<string?> GetPlayerItemsJsonAsync(string summonerName, CancellationToken ct = default);

    /// <summary>
    /// All Events that occurred in the game until now
    /// </summary>
    Task<string?> GetEventDataJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Common gamestats of the current game
    /// </summary>
    Task<string?> GetGameStatsJsonAsync(CancellationToken ct = default);

    void Dispose();
}