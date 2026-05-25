using BE.League.Desktop.Connection;

namespace BE.League.Desktop.IntegrationTests;

/// <summary>
/// Integration tests for the League Client API (LCU).
/// PREREQUISITE: Must be in the League Client (lobby or menu).
/// <para>
/// Tests the <see href="https://developer.riotgames.com/docs/lol#league-client-api">League Client API</see>
/// on its dynamic port discovered from the lockfile.
/// </para>
/// </summary>
public class WhenInLeagueClient
{
    private readonly LeagueDesktopClient? _sut;
    private readonly bool _isClientRunning;

    public WhenInLeagueClient()
    {
        try
        {
            var connection = LeagueClientConnectionInfo.GetFromRunningClient();
            if (connection != null)
            {
                _sut = new LeagueDesktopClient(new LeagueDesktopOptions
                {
                    Connection = connection,
                    Timeout = TimeSpan.FromSeconds(5)
                });
                _isClientRunning = true;
            }
        }
        catch
        {
            _isClientRunning = false;
        }
    }

    [Fact]
    public async Task ItCanQueryLobbyStatus()
    {
        if (!_isClientRunning) return; // Skip if client not running
        Assert.NotNull(_sut);

        // Returns null if not in a lobby, but must not throw
        await _sut.LeagueClient.Api.GetLobbyJsonAsync();

        Assert.True(true, "League Client API call completed without exception");
    }

    [Fact]
    public async Task ItCanQueryChampSelectSession()
    {
        if (!_isClientRunning) return; // Skip if client not running
        Assert.NotNull(_sut);

        // Returns null if not in champ select, but must not throw
        await _sut.LeagueClient.Api.GetChampSelectSessionJsonAsync();

        Assert.True(true, "League Client API call completed without exception");
    }

    [Fact]
    public async Task ItCanQueryReadyCheckStatus()
    {
        if (!_isClientRunning) return; // Skip if client not running
        Assert.NotNull(_sut);

        // Returns null if no ready check active, but must not throw
        await _sut.LeagueClient.Api.GetReadyCheckJsonAsync();

        Assert.True(true, "League Client API call completed without exception");
    }
}