using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingSummonerSpells : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetPlayerSummonerSpellsAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "summonerSpellOne": {
                "displayName": "Flash",
                "rawDisplayName": "SummonerFlash"
            },
            "summonerSpellTwo": {
                "displayName": "Ignite",
                "rawDisplayName": "SummonerDot"
            }
        }
        """;

        A.CallTo(() => Gateway.GetPlayerSummonerSpellsJsonAsync("TestPlayer", A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetPlayerSummonerSpellsAsync("TestPlayer");

        Assert.NotNull(result);
        Assert.NotNull(result.SummonerSpellOne);
        Assert.Equal("Flash", result.SummonerSpellOne.DisplayName);
        Assert.NotNull(result.SummonerSpellTwo);
        Assert.Equal("Ignite", result.SummonerSpellTwo.DisplayName);
    }

    [Fact]
    public async Task GetPlayerSummonerSpellsAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetPlayerSummonerSpellsJsonAsync("TestPlayer", A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetPlayerSummonerSpellsAsync("TestPlayer");

        Assert.Null(result);
    }
}

