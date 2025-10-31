namespace BE.League.Desktop.Connection;

public sealed record LeagueDesktopOptions()
{
    public required LeagueClientConnectionInfo Connection { get; init; }

    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);
}