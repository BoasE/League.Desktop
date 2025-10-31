namespace BE.League.Desktop.Connection;

/// <summary>
/// Reads the League Client connection information from the running process
/// </summary>
public sealed class LeagueClientConnectionInfo
{
    public string LockFilePath { get; init; }
    public string? Port { get; init; }
    public string? Token { get; init; }
    public string? Protocol { get; init; } = "https";

    public bool IsValid => !string.IsNullOrEmpty(Port) && !string.IsNullOrEmpty(Token);

    public string GetBaseUrl() => $"{Protocol}://127.0.0.1:{Port}";

    /// <summary>
    /// Find the League Client process and extract connection info from command line
    /// </summary>
    public static LeagueClientConnectionInfo? GetFromRunningClient()
    {
        var connection = ConnectionByLockFile();

        return connection;
    }


    private static LeagueClientConnectionInfo? ConnectionByLockFile()
    {
        LeagueClientConnectionInfo? connection = null;

        var path = ClientHelper.ResolveLockfilePath(null);
        var file = ClientHelper.ReadLockfile(path);
        if (file != null)
        {
            connection = new LeagueClientConnectionInfo
            {
                LockFilePath = path,
                Port = file.Port.ToString(),
                Protocol = file.Protocol,
                Token = file.Password
            };
        }

        return connection;
    }
}