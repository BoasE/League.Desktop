using BE.League.Desktop.Models;

namespace BE.League.Desktop.Tests.ModelTests;

public sealed class GivenReadyCheckDto
{
    [Fact]
    public void ItCanBeInstantiated()
    {
        var sut = new ReadyCheckDto();
        
        Assert.NotNull(sut);
    }

    [Theory]
    [InlineData("InProgress")]
    [InlineData("Everyone Accepted")]
    [InlineData("Declined")]
    public void ItCanSetState(string state)
    {
        var sut = new ReadyCheckDto { State = state };
        
        Assert.Equal(state, sut.State);
    }

    [Fact]
    public void ItHasNullStateByDefault()
    {
        var sut = new ReadyCheckDto();
        
        Assert.Null(sut.State);
    }
}

