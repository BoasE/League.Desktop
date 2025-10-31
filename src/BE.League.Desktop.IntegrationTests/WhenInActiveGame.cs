using BE.League.Desktop.Connection;

namespace BE.League.Desktop.IntegrationTests;

/// <summary>
/// Integration tests that require an active League of Legends game.
/// PREREQUISITE: Must be in an active game (not just in client lobby).
/// </summary>
public class WhenInActiveGame : IDisposable
{
    private readonly LeagueDesktopClient? _sut;
    private readonly bool _isInGame;

    public WhenInActiveGame()
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
                
                // Check if actually in game by trying to get active player name
                var playerName = _sut.GetActivePlayerNameJsonAsync().GetAwaiter().GetResult();
                _isInGame = !string.IsNullOrEmpty(playerName);
            }
        }
        catch
        {
            _isInGame = false;
        }
    }

    [Fact]
    public async Task ItCanGetActivePlayerName()
    {
        if (!_isInGame) return; // Skip if not in game
        Assert.NotNull(_sut);

        var result = await _sut.GetActivePlayerNameJsonAsync();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task ItCanGetGameStats()
    {
        if (!_isInGame) return; // Skip if not in game
        Assert.NotNull(_sut);

        var result = await _sut.GetGameStatsJsonAsync();

        Assert.NotNull(result);
        Assert.Contains("gameTime", result);
    }

    [Fact]
    public async Task ItCanGetPlayerList()
    {
        if (!_isInGame) return; // Skip if not in game
        Assert.NotNull(_sut);

        var result = await _sut.GetPlayerListJsonAsync();

        Assert.NotNull(result);
        Assert.Contains("[", result); // Should be JSON array
    }

    [Fact]
    public async Task ItCanGetActivePlayer()
    {
        if (!_isInGame) return; // Skip if not in game
        Assert.NotNull(_sut);

        var result = await _sut.GetActivePlayerJsonAsync();

        Assert.NotNull(result);
        Assert.Contains("summonerName", result);
        Assert.Contains("level", result);
    }

    [Fact]
    public async Task ItCanGetAllGameData()
    {
        if (!_isInGame) return; // Skip if not in game
        Assert.NotNull(_sut);

        var result = await _sut.GetAllGameDataJsonAsync();

        Assert.NotNull(result);
        Assert.Contains("activePlayer", result);
        Assert.Contains("gameData", result);
    }

    public void Dispose()
    {
        _sut?.Dispose();
    }
}

