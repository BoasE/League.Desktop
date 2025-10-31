using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingPlayerRunes : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetPlayerMainRunesAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "keystone": {
                "displayName": "Press the Attack",
                "id": 8005
            },
            "primaryRuneTree": {
                "displayName": "Precision",
                "id": 8000
            },
            "secondaryRuneTree": {
                "displayName": "Resolve",
                "id": 8400
            }
        }
        """;

        A.CallTo(() => Gateway.GetPlayerMainRunesJsonAsync("TestPlayer", A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetPlayerMainRunesAsync("TestPlayer");

        Assert.NotNull(result);
        Assert.NotNull(result.Keystone);
        Assert.Equal("Press the Attack", result.Keystone.DisplayName);
        Assert.Equal(8005, result.Keystone.Id);
        Assert.NotNull(result.PrimaryRuneTree);
        Assert.Equal("Precision", result.PrimaryRuneTree.DisplayName);
    }

    [Fact]
    public async Task GetPlayerMainRunesAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetPlayerMainRunesJsonAsync("TestPlayer", A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetPlayerMainRunesAsync("TestPlayer");

        Assert.Null(result);
    }
}

