using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingPlayerScores : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetPlayerScoresAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "kills": 5,
            "deaths": 2,
            "assists": 10,
            "creepScore": 150,
            "wardScore": 25.5
        }
        """;

        A.CallTo(() => Gateway.GetPlayerScoresJsonAsync("TestPlayer", A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetPlayerScoresAsync("TestPlayer");

        Assert.NotNull(result);
        Assert.Equal(5, result.Kills);
        Assert.Equal(2, result.Deaths);
        Assert.Equal(10, result.Assists);
        Assert.Equal(150, result.CreepScore);
        Assert.Equal(25.5f, result.WardScore);
    }

    [Fact]
    public async Task GetPlayerScoresAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetPlayerScoresJsonAsync("TestPlayer", A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetPlayerScoresAsync("TestPlayer");

        Assert.Null(result);
    }
}

