using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingAbilities : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetActivePlayerAbilitiesAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "Q": {
                "abilityLevel": 5,
                "displayName": "Q Ability",
                "id": "Q1"
            },
            "W": {
                "abilityLevel": 3,
                "displayName": "W Ability",
                "id": "W1"
            }
        }
        """;

        A.CallTo(() => Gateway.GetActivePlayerAbilitiesJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetActivePlayerAbilitiesAsync();

        Assert.NotNull(result);
        Assert.NotNull(result.Q);
        Assert.Equal(5, result.Q.AbilityLevel);
        Assert.Equal("Q Ability", result.Q.DisplayName);
    }

    [Fact]
    public async Task GetActivePlayerAbilitiesAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetActivePlayerAbilitiesJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetActivePlayerAbilitiesAsync();

        Assert.Null(result);
    }
}

