using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingActivePlayer : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetActivePlayerAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "level": 10,
            "summonerName": "PlayerOne",
            "currentGold": 2500.75,
            "teamRelativeColors": true
        }
        """;

        A.CallTo(() => Gateway.GetActivePlayerJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetActivePlayerAsync();

        Assert.NotNull(result);
        Assert.Equal(10, result.Level);
        Assert.Equal("PlayerOne", result.SummonerName);
        Assert.Equal(2500.75f, result.CurrentGold);
        Assert.True(result.TeamRelativeColors);
    }

    [Fact]
    public async Task GetActivePlayerAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetActivePlayerJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetActivePlayerAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetActivePlayerAsync_WithInvalidJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetActivePlayerJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>("not valid json"));

        var result = await Sut.GetActivePlayerAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllGameDataAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "activePlayer": {
                "level": 18,
                "summonerName": "TestPlayer",
                "currentGold": 1500.5
            },
            "gameData": {
                "gameMode": "CLASSIC",
                "gameTime": 1200.5,
                "mapName": "Summoner's Rift",
                "mapNumber": 11
            }
        }
        """;

        A.CallTo(() => Gateway.GetAllGameDataJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetAllGameDataAsync();

        Assert.NotNull(result);
        Assert.NotNull(result.ActivePlayer);
        Assert.Equal(18, result.ActivePlayer.Level);
        Assert.Equal("TestPlayer", result.ActivePlayer.SummonerName);
        Assert.Equal(1500.5f, result.ActivePlayer.CurrentGold);
    }

    [Fact]
    public async Task GetAllGameDataAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetAllGameDataJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetAllGameDataAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllGameDataAsync_WithEmptyString_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetAllGameDataJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(""));

        var result = await Sut.GetAllGameDataAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllGameDataAsync_WithWhitespaceString_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetAllGameDataJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>("   "));

        var result = await Sut.GetAllGameDataAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllGameDataAsync_WithInvalidJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetAllGameDataJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>("{invalid json}"));

        var result = await Sut.GetAllGameDataAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllGameDataAsync_WithCancellationToken_PassesTokenToGateway()
    {
        var cts = new CancellationTokenSource();
        var json = "{}";

        A.CallTo(() => Gateway.GetAllGameDataJsonAsync(cts.Token))
            .Returns(Task.FromResult<string?>(json));

        await Sut.GetAllGameDataAsync(cts.Token);

        A.CallTo(() => Gateway.GetAllGameDataJsonAsync(cts.Token))
            .MustHaveHappenedOnceExactly();
    }
}

