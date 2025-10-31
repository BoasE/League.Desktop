using BE.League.Desktop.Models;

namespace BE.League.Desktop.Tests.ModelTests;

public sealed class GivenLiveWrapper
{
    [Fact]
    public void ItCanBeInstantiated()
    {
        var sut = new LiveWrapper();
        
        Assert.NotNull(sut);
    }

    [Fact]
    public void ItHasEmptyEventsListByDefault()
    {
        var sut = new LiveWrapper();
        
        Assert.NotNull(sut.Events);
        Assert.Empty(sut.Events);
    }

    [Fact]
    public void ItCanSetEvents()
    {
        var events = new List<LiveEvent>
        {
            new() { EventID = 1, EventName = "GameStart" }
        };
        var sut = new LiveWrapper { Events = events };
        
        Assert.Single(sut.Events);
        Assert.Equal(1, sut.Events[0].EventID);
    }
}

