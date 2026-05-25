using BE.League.Desktop.Models;

namespace BE.League.Desktop.LeagueClientApi;

/// <summary>
/// Event arguments raised when the lobby state changes.
/// </summary>
public sealed class LobbyChangedEventArgs(Lobby? Lobby) : EventArgs;

/// <summary>
/// Event arguments raised when the ready-check state changes.
/// </summary>
public sealed class ReadyCheckStateChangedEventArgs(ReadyCheck? readyState)
{
    /// <summary>
    /// Returns <c>true</c> when the ready check is in progress and can be accepted/declined.
    /// </summary>
    public bool CanBeClicked => readyState is { State: "InProgress" };
}

/// <summary>
/// Polling event loop for the League Client API (LCU).
/// <para>
/// Polls <see cref="LeagueClientApiReader"/> every 500 ms and raises
/// <see cref="LobbyChanged"/> and <see cref="ReadyCheckChanged"/> events
/// whenever the state changes from the previous poll.
/// </para>
/// <para>
/// Run until cancelled via a <see cref="CancellationToken"/>.
/// </para>
/// <para>
/// Official documentation:
/// <see href="https://developer.riotgames.com/docs/lol#league-client-api">
/// Riot Games — League Client API
/// </see>
/// </para>
/// </summary>
public class LeagueClientApiEventLoop
{
    /// <summary>Raised when the lobby state changes (joined, left, or updated).</summary>
    public event EventHandler<LobbyChangedEventArgs> LobbyChanged;

    /// <summary>Raised when the ready-check state changes.</summary>
    public event EventHandler<ReadyCheckStateChangedEventArgs> ReadyCheckChanged;

    private readonly LeagueClientApiReader _leagueClient;

    public LeagueClientApiEventLoop(LeagueClientApiReader? leagueClient = null)
    {
        _leagueClient = leagueClient ?? new LeagueClientApiReader();
    }

    /// <summary>
    /// Starts the polling loop. Runs until <paramref name="cancellationToken"/> is cancelled.
    /// </summary>
    public async Task Run(CancellationToken cancellationToken)
    {
        Lobby? lobby = null;
        ReadyCheck? readyCheck = null;

        while (!cancellationToken.IsCancellationRequested)
        {
            lobby = await CheckLobbyChange(lobby, cancellationToken);
            readyCheck = await CheckReadyCheck(readyCheck, cancellationToken);

            await Task.Delay(500, cancellationToken);
        }
    }

    private async Task<ReadyCheck?> CheckReadyCheck(ReadyCheck? previousReadyCheck, CancellationToken cancellationToken)
    {
        ReadyCheck? currentReadyCheck = await _leagueClient.GetReadyCheckAsync(cancellationToken);

        if (currentReadyCheck != previousReadyCheck)
        {
            OnReadyCheckChanged(currentReadyCheck);
        }

        return currentReadyCheck;
    }

    private async Task<Lobby?> CheckLobbyChange(Lobby? previousLobby, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return null;
        }

        var currentLobby = await _leagueClient.GetLobbyAsync(cancellationToken);

        if (currentLobby != previousLobby)
        {
            OnLobbyChanged(currentLobby);
        }

        return currentLobby;
    }

    private void OnReadyCheckChanged(ReadyCheck? state)
    {
        ReadyCheckChanged?.Invoke(this, new ReadyCheckStateChangedEventArgs(state));
    }

    private void OnLobbyChanged(Lobby? currentLobby)
    {
        LobbyChanged?.Invoke(this, new LobbyChangedEventArgs(currentLobby));
    }
}

