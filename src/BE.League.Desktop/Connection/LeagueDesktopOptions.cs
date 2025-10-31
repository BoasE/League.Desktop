namespace BE.League.Desktop.Connection;

public sealed record LeagueDesktopOptions()
{
    public LeagueClientConnectionInfo? Connection { get; init; }

    public string? LiveClientBaseUrl { get; init; }

    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(10);
}