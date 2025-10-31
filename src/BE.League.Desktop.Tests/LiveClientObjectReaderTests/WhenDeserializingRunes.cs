using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingRunes : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetActivePlayerRunesAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "keystone": {
                "displayName": "Electrocute",
                "id": 8112,
                "rawDisplayName": "perk8112"
            },
            "primaryRuneTree": {
                "displayName": "Domination",
                "id": 8100
            },
            "secondaryRuneTree": {
                "displayName": "Precision",
                "id": 8000
            }
        }
        """;

        A.CallTo(() => Gateway.GetActivePlayerRunesJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetActivePlayerRunesAsync();

        Assert.NotNull(result);
        Assert.NotNull(result.Keystone);
        Assert.Equal("Electrocute", result.Keystone.DisplayName);
        Assert.Equal(8112, result.Keystone.Id);
        Assert.NotNull(result.PrimaryRuneTree);
        Assert.Equal("Domination", result.PrimaryRuneTree.DisplayName);
    }

    [Fact]
    public async Task GetActivePlayerRunesAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetActivePlayerRunesJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetActivePlayerRunesAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetActivePlayerRunesAsync_WithInvalidJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetActivePlayerRunesJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>("{invalid}"));

        var result = await Sut.GetActivePlayerRunesAsync();

        Assert.Null(result);
    }
}

