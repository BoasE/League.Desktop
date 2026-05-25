namespace BE.League.Desktop.Connection;

/// <summary>
/// Configuration options for <see cref="BE.League.Desktop.LeagueDesktopClient"/>.
/// </summary>
public sealed record LeagueDesktopOptions()
{
    /// <summary>
    /// Connection info for the League Client API (LCU).
    /// If <c>null</c>, connection is auto-discovered from the running client's lockfile.
    /// </summary>
    public LeagueClientConnectionInfo? Connection { get; init; }

    /// <summary>
    /// Optional override for the Game Client API base URL.
    /// Defaults to <c>https://127.0.0.1:2999</c> when not set.
    /// </summary>
    public string? GameClientBaseUrl { get; init; }

    /// <summary>Request timeout for all API calls. Defaults to 10 seconds.</summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);
}