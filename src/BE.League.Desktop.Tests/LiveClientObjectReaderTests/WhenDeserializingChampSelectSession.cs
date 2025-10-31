using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingChampSelectSession : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetChampSelectSessionAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "localPlayerCellId": 0,
            "mySelectionChampionId": 1,
            "myLockedChampionId": 1,
            "timer": {
                "phase": "BAN_PICK"
            },
            "myTeam": [
                {
                    "cellId": 0,
                    "championId": 1,
                    "spell1Id": 4,
                    "spell2Id": 14
                }
            ],
            "actions": []
        }
        """;

        A.CallTo(() => Gateway.GetChampSelectSessionJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetChampSelectSessionAsync();

        Assert.NotNull(result);
        Assert.Equal(0, result.LocalPlayerCellId);
        Assert.Equal(1, result.MySelectionChampionId);
        Assert.NotNull(result.Timer);
        Assert.Equal("BAN_PICK", result.Timer.Phase);
        Assert.NotEmpty(result.MyTeam);
    }

    [Fact]
    public async Task GetChampSelectSessionAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetChampSelectSessionJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetChampSelectSessionAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetChampSelectSessionAsync_WithEmptyJson_ReturnsEmptyObject()
    {
        A.CallTo(() => Gateway.GetChampSelectSessionJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>("{}"));

        var result = await Sut.GetChampSelectSessionAsync();

        Assert.NotNull(result);
        Assert.Empty(result.MyTeam);
        Assert.Empty(result.Actions);
    }
}

