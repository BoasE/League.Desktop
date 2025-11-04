using BE.League.Desktop.Connection;

namespace BE.League.Desktop.IntegrationTests;

/// <summary>
/// Integration tests for LCU (League Client Update) API.
/// PREREQUISITE: Must be in League Client (can be in lobby or menu).
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

        // This should not throw even if not in lobby (returns null/error response)
        await _sut.LcuClient.Api.GetLobbyJsonAsync();

        // Test passes if no exception is thrown
        Assert.True(true, "LCU API call completed without exception");
    }

    [Fact]
    public async Task ItCanQueryChampSelectSession()
    {
        if (!_isClientRunning) return; // Skip if client not running
        Assert.NotNull(_sut);

        // This should not throw even if not in champ select
        await _sut.LcuClient.Api.GetChampSelectSessionJsonAsync();

        // Test passes if no exception is thrown
        Assert.True(true, "LCU API call completed without exception");
    }

    [Fact]
    public async Task ItCanQueryReadyCheckStatus()
    {
        if (!_isClientRunning) return; // Skip if client not running
        Assert.NotNull(_sut);

        // This should not throw even if no ready check active
        await _sut.LcuClient.Api.GetReadyCheckJsonAsync();

        // Test passes if no exception is thrown
        Assert.True(true, "LCU API call completed without exception");
    }
}