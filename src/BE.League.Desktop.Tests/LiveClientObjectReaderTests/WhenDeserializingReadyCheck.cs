using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public sealed class WhenDeserializingReadyCheck : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetReadyCheckAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "state": "InProgress"
        }
        """;

        A.CallTo(() => Gateway.GetReadyCheckJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetReadyCheckAsync();

        Assert.NotNull(result);
        Assert.Equal("InProgress", result.State);
    }

    [Fact]
    public async Task GetReadyCheckAsync_WithNullJson_ReturnsNull()
    {
        A.CallTo(() => Gateway.GetReadyCheckJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        var result = await Sut.GetReadyCheckAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task AcceptReadyCheckAsync_CallsGatewayMethod()
    {
        A.CallTo(() => Gateway.AcceptReadyCheckAsync(A<CancellationToken>._))
            .Returns(Task.FromResult(true));

        var result = await Sut.AcceptReadyCheckAsync();

        Assert.True(result);
        A.CallTo(() => Gateway.AcceptReadyCheckAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task DeclineReadyCheckAsync_CallsGatewayMethod()
    {
        A.CallTo(() => Gateway.DeclineReadyCheckAsync(A<CancellationToken>._))
            .Returns(Task.FromResult(true));

        var result = await Sut.DeclineReadyCheckAsync();

        Assert.True(result);
        A.CallTo(() => Gateway.DeclineReadyCheckAsync(A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task AcceptReadyCheckAsync_WithCancellationToken_PassesTokenToGateway()
    {
        var cts = new CancellationTokenSource();
        
        A.CallTo(() => Gateway.AcceptReadyCheckAsync(cts.Token))
            .Returns(Task.FromResult(true));

        await Sut.AcceptReadyCheckAsync(cts.Token);

        A.CallTo(() => Gateway.AcceptReadyCheckAsync(cts.Token))
            .MustHaveHappenedOnceExactly();
    }
}

