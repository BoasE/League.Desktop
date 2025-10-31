using BE.League.Desktop;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenGettingActivePlayerName : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetActivePlayerNameAsync_ReturnsPlayerName()
    {
        const string expectedName = "TestSummoner";
        
        A.CallTo(() => Gateway.GetActivePlayerNameJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(expectedName));

        var result = await Sut.GetActivePlayerNameAsync();

        Assert.Equal(expectedName, result);
    }

    [Fact]
    public async Task GetActivePlayerNameAsync_WithNullResponse_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetActivePlayerNameJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetActivePlayerNameAsync();

        Assert.Null(result);
    }
}

