namespace BE.League.Desktop.LcuClient;

public interface ILcuApi
{
    /// <summary>
    /// Get current lobby information
    /// </summary>
    Task<string?> GetLobbyJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Get current champion select session
    /// </summary>
    Task<string?> GetChampSelectSessionJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Get ready check state
    /// </summary>
    Task<string?> GetReadyCheckJsonAsync(CancellationToken ct = default);

    /// <summary>
    /// Accept ready check (when game is found)
    /// </summary>
    Task<bool> AcceptReadyCheckAsync(CancellationToken ct = default);

    /// <summary>
    /// Decline ready check
    /// </summary>
    Task<bool> DeclineReadyCheckAsync(CancellationToken ct = default);

    void Dispose();
}