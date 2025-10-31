using BE.League.Desktop.Models;

namespace BE.League.Desktop.Tests.ModelTests;

public sealed class GivenChampSelectSession
{
    [Fact]
    public void ItCanBeInstantiated()
    {
        var sut = new ChampSelectSession();
        
        Assert.NotNull(sut);
    }

    [Fact]
    public void ItHasEmptyActionsListByDefault()
    {
        var sut = new ChampSelectSession();
        
        Assert.NotNull(sut.Actions);
        Assert.Empty(sut.Actions);
    }

    [Fact]
    public void ItHasEmptyMyTeamListByDefault()
    {
        var sut = new ChampSelectSession();
        
        Assert.NotNull(sut.MyTeam);
        Assert.Empty(sut.MyTeam);
    }

    [Fact]
    public void ItCanSetLocalPlayerCellId()
    {
        var sut = new ChampSelectSession { LocalPlayerCellId = 5 };
        
        Assert.Equal(5, sut.LocalPlayerCellId);
    }

    [Fact]
    public void ItCanSetMySelectionChampionId()
    {
        var sut = new ChampSelectSession { MySelectionChampionId = 103 };
        
        Assert.Equal(103, sut.MySelectionChampionId);
    }
}

