using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingEvents : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetEventDataAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "Events": [
                {
                    "EventID": 1,
                    "EventName": "ChampionKill",
                    "EventTime": 180.5,
                    "KillerName": "Player1",
                    "VictimName": "Player2",
                    "Assisters": ["Player3"]
                },
                {
                    "EventID": 2,
                    "EventName": "DragonKill",
                    "EventTime": 300.0,
                    "KillerName": "Player1",
                    "DragonType": "Cloud"
                }
            ]
        }
        """;

        A.CallTo(() => Gateway.GetEventDataJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetEventDataAsync();

        Assert.NotNull(result);
        Assert.NotNull(result.EventsList);
        Assert.Equal(2, result.EventsList.Count);
        Assert.Equal("ChampionKill", result.EventsList[0].EventName);
        Assert.Equal("Player1", result.EventsList[0].KillerName);
        Assert.Equal("DragonKill", result.EventsList[1].EventName);
        Assert.Equal("Cloud", result.EventsList[1].DragonType);
    }

    [Fact]
    public async Task GetEventDataAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetEventDataJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetEventDataAsync();

        Assert.Null(result);
    }
}

