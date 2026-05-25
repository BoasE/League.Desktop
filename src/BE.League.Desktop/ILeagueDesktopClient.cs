using BE.League.Desktop.GameClientApi;
using BE.League.Desktop.LeagueClientApi;

namespace BE.League.Desktop;

/// <summary>
/// Unified interface for League of Legends Desktop integration.
/// Exposes both the <see cref="GameClientApiReader"/> (in-game) and the
/// <see cref="LeagueClientApiReader"/> (desktop client / lobby).
/// </summary>
public interface ILeagueDesktopClient
{
    /// <summary>
    /// Game Client API reader — provides real-time in-game data.
    /// Available only while an active game is running on port 2999.
    /// <para>
    /// See <see href="https://developer.riotgames.com/docs/lol#game-client-api">
    /// Riot Games — Game Client API
    /// </see>
    /// </para>
    /// </summary>
    GameClientApiReader GameClient { get; }

    /// <summary>
    /// League Client API reader — provides lobby, matchmaking, and champion select data.
    /// Available while the League of Legends desktop client process is running.
    /// <para>
    /// See <see href="https://developer.riotgames.com/docs/lol#league-client-api">
    /// Riot Games — League Client API
    /// </see>
    /// </para>
    /// </summary>
    LeagueClientApiReader LeagueClient { get; }
}