using BE.League.Desktop.Connection;
using BE.League.Desktop.GameClientApi;
using BE.League.Desktop.LeagueClientApi;

namespace BE.League.Desktop;

/// <summary>
/// Unified facade for League of Legends Desktop integration.
/// <para>
/// Provides access to both local APIs exposed by the League of Legends client:
/// </para>
/// <list type="bullet">
///   <item>
///     <see cref="GameClient"/> — the
///     <see href="https://developer.riotgames.com/docs/lol#game-client-api">Game Client API</see>
///     (port 2999, in-game real-time data, no auth required)
///   </item>
///   <item>
///     <see cref="LeagueClient"/> — the
///     <see href="https://developer.riotgames.com/docs/lol#league-client-api">League Client API</see>
///     (dynamic port from lockfile, lobby/matchmaking/champ-select, Basic Auth required)
///   </item>
/// </list>
/// </summary>
public sealed class LeagueDesktopClient : ILeagueDesktopClient
{
    /// <summary>
    /// Game Client API reader — real-time in-game data (port 2999).
    /// </summary>
    public GameClientApiReader GameClient { get; }

    /// <summary>
    /// League Client API reader — lobby, matchmaking, and champion select data.
    /// </summary>
    public LeagueClientApiReader LeagueClient { get; }

    /// <summary>
    /// DI-friendly constructor — accepts pre-built readers for both APIs.
    /// </summary>
    public LeagueDesktopClient(GameClientApiReader gameClient, LeagueClientApiReader leagueClient)
    {
        GameClient = gameClient;
        LeagueClient = leagueClient;
    }

    /// <summary>
    /// Convention constructor — auto-discovers the League Client connection from the lockfile
    /// and creates both API readers from <paramref name="options"/>.
    /// </summary>
    public LeagueDesktopClient(LeagueDesktopOptions? options = null)
    {
        options ??= new LeagueDesktopOptions();

        GameClient = new GameClientApiReader(options);

        var lcuApi = new LeagueClientApiClient(options.Connection, options.Timeout);
        LeagueClient = new LeagueClientApiReader(lcuApi);
    }
}