using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingGameData : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetGameStatsAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "gameMode": "CLASSIC",
            "gameTime": 1543.5,
            "mapName": "Summoner's Rift",
            "mapNumber": 11,
            "mapTerrain": "Default"
        }
        """;

        A.CallTo(() => Gateway.GetGameStatsJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetGameStatsAsync();

        Assert.NotNull(result);
        Assert.Equal("CLASSIC", result.GameMode);
        Assert.Equal(1543.5f, result.GameTime);
        Assert.Equal("Summoner's Rift", result.MapName);
        Assert.Equal(11, result.MapNumber);
    }

    [Fact]
    public async Task GetGameStatsAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetGameStatsJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetGameStatsAsync();

        Assert.Null(result);
    }
}

