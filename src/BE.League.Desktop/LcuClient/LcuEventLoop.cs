using BE.League.Desktop.Models;

namespace BE.League.Desktop.LcuClient;

public sealed class LobbyChangedEventArgs(Lobby? Lobby) : EventArgs;

public sealed class ReadyCheckStateChangedEventArgs(ReadyCheck? readyState)
{
    public bool CanBeClicked => readyState is { State: "InProgress" };
}

public class LcuEventLoop
{
    public event EventHandler<LobbyChangedEventArgs> LobbyChanged;
    public event EventHandler<ReadyCheckStateChangedEventArgs> ReadyCheckChanged;

    private readonly LcuObjectReader _lcu;


    public LcuEventLoop(LcuObjectReader? lcu = null)
    {
        _lcu = lcu ?? new LcuObjectReader();
    }


    public async Task Run(CancellationToken cancellationToken)
    {
        var started = new DateTimeOffset();
        var acceptCount = 0;

        Lobby? lobby = null;


        ReadyCheck? readyCheck = null;
        while (!cancellationToken.IsCancellationRequested)
        {
            lobby = await CheckLobbyChange(lobby, cancellationToken);
            readyCheck = await CheckReadyCheck(readyCheck, cancellationToken);

            await Task.Delay(500, cancellationToken);
        }
    }

    private async Task<ReadyCheck> CheckReadyCheck(ReadyCheck? previousReadyCheck, CancellationToken cancellationToken)
    {
        ReadyCheck? currentReadyCheck = await _lcu.GetReadyCheckAsync(cancellationToken);

        if (currentReadyCheck != previousReadyCheck)
        {
            this.OnReadCheckChanged(currentReadyCheck);
        }

        return currentReadyCheck;
    }

    private async Task<Lobby?> CheckLobbyChange(Lobby? previousLobby, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return null;
        }

        var currentLobby = await this._lcu.GetLobbyAsync(cancellationToken);
        if (currentLobby != previousLobby)
        {
            OnLobbyChanged(currentLobby);
        }

        previousLobby = currentLobby;
        return previousLobby;
    }


    private void OnReadCheckChanged(ReadyCheck? state)
    {
        ReadyCheckChanged?.Invoke(this, new ReadyCheckStateChangedEventArgs(state));
    }

    private void OnLobbyChanged(Lobby? currentLobby)
    {
        LobbyChanged?.Invoke(this, new LobbyChangedEventArgs(currentLobby));
    }
}