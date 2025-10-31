using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingPlayerList : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetPlayerListAsync_WithValidJson_ReturnsDeserializedList()
    {
        var json = """
        [
            {
                "championName": "Ahri",
                "summonerName": "Player1",
                "level": 10,
                "isBot": false,
                "isDead": false,
                "team": "ORDER"
            },
            {
                "championName": "Yasuo",
                "summonerName": "Player2",
                "level": 12,
                "isBot": false,
                "isDead": true,
                "team": "CHAOS"
            }
        ]
        """;

        A.CallTo(() => Gateway.GetPlayerListJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetPlayerListAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Ahri", result[0].ChampionName);
        Assert.Equal("Player1", result[0].SummonerName);
        Assert.False(result[0].IsDead);
        Assert.Equal("Yasuo", result[1].ChampionName);
        Assert.True(result[1].IsDead);
    }

    [Fact]
    public async Task GetPlayerListAsync_WithEmptyArray_ReturnsEmptyList()
    {
        A.CallTo(() => Gateway.GetPlayerListJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>("[]"));

        var result = await Sut.GetPlayerListAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPlayerListAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetPlayerListJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetPlayerListAsync();

        Assert.Null(result);
    }
}

