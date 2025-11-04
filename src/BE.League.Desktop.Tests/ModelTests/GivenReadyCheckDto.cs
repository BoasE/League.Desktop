using BE.League.Desktop.Models;

namespace BE.League.Desktop.Tests.ModelTests;

public sealed class GivenReadyCheckDto
{
    [Fact]
    public void ItCanBeInstantiated()
    {
        var sut = new ReadyCheck();
        
        Assert.NotNull(sut);
    }

    [Theory]
    [InlineData("InProgress")]
    [InlineData("Everyone Accepted")]
    [InlineData("Declined")]
    public void ItCanSetState(string state)
    {
        var sut = new ReadyCheck { State = state };
        
        Assert.Equal(state, sut.State);
    }

    [Fact]
    public void ItHasNullStateByDefault()
    {
        var sut = new ReadyCheck();
        
        Assert.Null(sut.State);
    }
}

