using BE.League.Desktop.Connection;

namespace BE.League.Desktop.IntegrationTests;

/// <summary>
/// Integration tests for LeagueDesktopClient.
/// PREREQUISITE: League of Legends must be running on the same machine.
/// </summary>
public class GivenLeagueDesktopClient
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
    public async Task ItCanConnectToGameClientApi()
    {
        Assert.NotNull(_sut);

        // Try to get active player name — returns null when not in a game, but must not throw
        await _sut.GameClient.Api.GetActivePlayerNameJsonAsync();

        Assert.True(true, "Connection to Game Client API successful");
    }

    [Fact]
    public async Task ItCanConnectToLeagueClientApi()
    {
        Assert.NotNull(_sut);

        // Try to get lobby information — returns null when not in a lobby, but must not throw
        await _sut.LeagueClient.Api.GetLobbyJsonAsync();

        Assert.True(true, "Connection to League Client API successful");
    }
}