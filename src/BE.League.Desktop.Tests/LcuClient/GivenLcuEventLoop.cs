using System;
using System.Threading;
using System.Threading.Tasks;
using BE.League.Desktop.LcuClient;
using BE.League.Desktop.Models;
using FakeItEasy;
using Xunit;

namespace BE.League.Desktop.Tests.LcuClient;

public class GivenLcuEventLoop
{
    [Fact]
    public async Task Raises_LobbyChanged_once_when_lobby_exists()
    {
        // Arrange
        var api = A.Fake<ILcuApi>();
        // minimal valid lobby JSON to deserialize into Lobby instance
        A.CallTo(() => api.GetLobbyJsonAsync(A<CancellationToken>._))
            .Returns("{}");
        // ready check can be anything; it's polled in the loop
        A.CallTo(() => api.GetReadyCheckJsonAsync(A<CancellationToken>._))
            .Returns("{\"state\":\"InProgress\"}");

        var reader = new LcuObjectReader(api);
        var loop = new LcuEventLoop(reader);

        var raisedCount = 0;
        loop.LobbyChanged += (_, _) =>
        {
            raisedCount++;
        };

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(600); // allow one iteration to occur

        // Act + Assert: Run will be canceled by token (TaskCanceledException expected)
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => loop.Run(cts.Token));

        // Assert
        Assert.Equal(1, raisedCount);
    }

    [Fact]
    public async Task Does_not_raise_LobbyChanged_when_no_lobby()
    {
        // Arrange
        var api = A.Fake<ILcuApi>();
        // No lobby available
        A.CallTo(() => api.GetLobbyJsonAsync(A<CancellationToken>._))
            .Returns((string?)null);
        A.CallTo(() => api.GetReadyCheckJsonAsync(A<CancellationToken>._))
            .Returns("{\"state\":\"InProgress\"}");

        var reader = new LcuObjectReader(api);
        var loop = new LcuEventLoop(reader);

        var raisedCount = 0;
        loop.LobbyChanged += (_, _) => raisedCount++;

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(600);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => loop.Run(cts.Token));

        Assert.Equal(0, raisedCount);
    }

    [Fact]
    public async Task Does_not_raise_ReadyCheckChanged_even_when_state_changes_current_behavior()
    {
        // Arrange
        var api = A.Fake<ILcuApi>();
        // Provide lobby so the loop runs as usual
        A.CallTo(() => api.GetLobbyJsonAsync(A<CancellationToken>._))
            .Returns("{}");
        // Simulate ready-check state changes across iterations
        A.CallTo(() => api.GetReadyCheckJsonAsync(A<CancellationToken>._))
            .ReturnsNextFromSequence(
                "{\"state\":\"InProgress\"}",
                "{\"state\":\"Matched\"}",
                "{\"state\":\"InProgress\"}"); // after sequence, last value is repeated by FakeItEasy

        var reader = new LcuObjectReader(api);
        var loop = new LcuEventLoop(reader);

        var readyCheckEvents = 0;
        loop.ReadyCheckChanged += (_, _) => readyCheckEvents++;

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(1200); // allow a couple of iterations

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => loop.Run(cts.Token));

        // Current implementation never raises ReadyCheckChanged
        Assert.Equal(0, readyCheckEvents);
    }
}
