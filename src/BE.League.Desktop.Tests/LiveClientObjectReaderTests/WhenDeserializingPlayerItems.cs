using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingPlayerItems : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetPlayerItemsAsync_WithValidJson_ReturnsDeserializedList()
    {
        var json = """
        [
            {
                "itemID": 3074,
                "displayName": "Ravenous Hydra",
                "count": 1,
                "canUse": true,
                "consumable": false,
                "slot": 0,
                "price": 3300
            },
            {
                "itemID": 2031,
                "displayName": "Refillable Potion",
                "count": 2,
                "canUse": true,
                "consumable": true,
                "slot": 6,
                "price": 150
            }
        ]
        """;

        A.CallTo(() => Gateway.GetPlayerItemsJsonAsync("TestPlayer", A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetPlayerItemsAsync("TestPlayer");

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(3074, result[0].ItemId);
        Assert.Equal("Ravenous Hydra", result[0].DisplayName);
        Assert.False(result[0].Consumable);
        Assert.Equal(2031, result[1].ItemId);
        Assert.True(result[1].Consumable);
    }

    [Fact]
    public async Task GetPlayerItemsAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetPlayerItemsJsonAsync("TestPlayer", A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetPlayerItemsAsync("TestPlayer");

        Assert.Null(result);
    }
}

