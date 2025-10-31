using BE.League.Desktop.Connection;

namespace BE.League.Desktop.IntegrationTests;

/// <summary>
/// Integration tests for LeagueDesktopClient.
/// PREREQUISITE: League of Legends must be running on the same machine.
/// </summary>
public class GivenLeagueDesktopClient : IDisposable
{
    private readonly LeagueDesktopClient? _sut;
    private readonly bool _isLeagueRunning;

    public GivenLeagueDesktopClient()
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
                _isLeagueRunning = true;
            }
        }
        catch
        {
            _isLeagueRunning = false;
        }
    }

    [Fact]
    public void ItRequiresLeagueOfLegendsToBeRunning()
    {

        
        Assert.True(_isLeagueRunning);
    }

    [Fact]
    public void ItCanBeInstantiatedWhenLeagueIsRunning()
    {


        Assert.NotNull(_sut);
    }

    [Fact]
    public async Task ItCanConnectToLiveClientDataApi()
    {
        Assert.NotNull(_sut);

        // Try to get active player name - this should work if a game is running
        // or return null if in lobby
        await _sut.GetActivePlayerNameJsonAsync();

        // We just verify that the call doesn't throw an exception
        // Result can be null if not in game
        Assert.True(true, "Connection to Live Client Data API successful");
    }

    [Fact]
    public async Task ItCanGetLobbyInformation()
    {

        Assert.NotNull(_sut);

        // Try to get lobby information via LCU API
        await _sut.GetLobbyJsonAsync();

        // Result can be null if not in lobby, but the call should not throw
        Assert.True(true, "Connection to LCU API successful");
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}

