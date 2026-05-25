namespace BE.League.Desktop.LeagueClientApi;

/// <summary>
/// Interface for the League Client API (LCU).
/// <para>
/// The League Client API is the internal REST API built into the League of Legends desktop client.
/// It runs on a <b>dynamic port</b> discovered via the lockfile and requires
/// <c>Authorization: Basic riot:&lt;token&gt;</c> authentication.
/// It is available whenever the League Client process is running.
/// </para>
/// <para>
/// Official documentation:
/// <see href="https://developer.riotgames.com/docs/lol#league-client-api">
/// Riot Games — League Client API
/// </see>
/// </para>
/// <para>
/// Community documentation:
/// <see href="https://hextechdocs.dev/">HexTech Docs — LCU API reference</see>
/// </para>
/// </summary>
public interface ILeagueClientApi
{
    /// <summary>
    /// Get current lobby information.
    /// <para>Endpoint: <c>GET /lol-lobby/v2/lobby</c></para>
    /// </summary>
    Task<string?> GetLobbyJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Get the current champion select session.
    /// <para>Endpoint: <c>GET /lol-champ-select/v1/session</c></para>
    /// </summary>
    Task<string?> GetChampSelectSessionJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Get the current ready-check state.
    /// <para>Endpoint: <c>GET /lol-matchmaking/v1/ready-check</c></para>
    /// </summary>
    Task<string?> GetReadyCheckJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Accept a ready check (when a game has been found).
    /// <para>Endpoint: <c>POST /lol-matchmaking/v1/ready-check/accept</c></para>
    /// </summary>
    Task<bool> AcceptReadyCheckAsync(CancellationToken ct = default);

    /// <summary>
    /// Decline a ready check.
    /// <para>Endpoint: <c>POST /lol-matchmaking/v1/ready-check/decline</c></para>
    /// </summary>
    Task<bool> DeclineReadyCheckAsync(CancellationToken ct = default);

    void Dispose();
}

