using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingLobby : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetLobbyAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "canStartActivity": true,
            "partyId": "party-123",
            "partyType": "open",
            "members": [
                {
                    "summonerId": 12345,
                    "summonerName": "Player1",
                    "isLeader": true,
                    "ready": true
                }
            ]
        }
        """;

        A.CallTo(() => Gateway.GetLobbyJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetLobbyAsync();

        Assert.NotNull(result);
        Assert.True(result.CanStartActivity);
        Assert.Equal("party-123", result.PartyId);
        Assert.NotEmpty(result.Members);
    }

    [Fact]
    public async Task GetLobbyAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetLobbyJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetLobbyAsync();

        Assert.Null(result);
    }
}

