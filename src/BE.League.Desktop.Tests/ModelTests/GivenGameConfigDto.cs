using BE.League.Desktop.Models;

namespace BE.League.Desktop.Tests.ModelTests;

public sealed class GivenGameConfigDto
{
    [Fact]
    public void ItCanBeInstantiated()
    {
        var sut = new GameConfig();
        
        Assert.NotNull(sut);
    }

    [Fact]
    public void ItCanSetGameMode()
    {
        var sut = new GameConfig { GameMode = "CLASSIC" };
        
        Assert.Equal("CLASSIC", sut.GameMode);
    }

    [Fact]
    public void ItCanSetMapId()
    {
        var sut = new GameConfig { MapId = 11 };
        
        Assert.Equal(11, sut.MapId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ItCanSetIsCustom(bool isCustom)
    {
        var sut = new GameConfig { IsCustom = isCustom };
        
        Assert.Equal(isCustom, sut.IsCustom);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ItCanSetIsLobbyFull(bool isLobbyFull)
    {
        var sut = new GameConfig { IsLobbyFull = isLobbyFull };
        
        Assert.Equal(isLobbyFull, sut.IsLobbyFull);
    }

    [Fact]
    public void ItCanSetMaxLobbySize()
    {
        var sut = new GameConfig { MaxLobbySize = 5 };
        
        Assert.Equal(5, sut.MaxLobbySize);
    }

    [Fact]
    public void ItCanSetQueueId()
    {
        var sut = new GameConfig { QueueId = 420 };
        
        Assert.Equal(420, sut.QueueId);
    }

    [Fact]
    public void ItCanSetPickType()
    {
        var sut = new GameConfig { PickType = "BLIND_PICK" };
        
        Assert.Equal("BLIND_PICK", sut.PickType);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ItCanSetShowPositionSelector(bool show)
    {
        var sut = new GameConfig { ShowPositionSelector = show };
        
        Assert.Equal(show, sut.ShowPositionSelector);
    }

    

    [Fact]
    public void ItHasEmptyMembersArrayByDefault()
    {
        var sut = new Lobby();
        
        Assert.NotNull(sut.Members);
        Assert.Empty(sut.Members);
    }

    [Fact]
    public void ItCanSetCanStartActivity()
    {
        var sut = new Lobby { CanStartActivity = true };
        
        Assert.True(sut.CanStartActivity);
    }

    [Fact]
    public void ItCanSetPartyId()
    {
        var sut = new Lobby { PartyId = "party-123" };
        
        Assert.Equal("party-123", sut.PartyId);
    }

    [Fact]
    public void ItCanSetPartyType()
    {
        var sut = new Lobby { PartyType = "open" };
        
        Assert.Equal("open", sut.PartyType);
    }

    [Fact]
    public void ItCanSetMembers()
    {
        var members = new[] { new LobbyMember { SummonerId = 123 } };
        var sut = new Lobby { Members = members };
        
        Assert.Single(sut.Members);
        Assert.Equal(123, sut.Members[0].SummonerId);
    }

    [Fact]
    public void ItCanSetLocalMember()
    {
        var localMember = new LobbyMember { SummonerId = 456, IsLeader = true };
        var sut = new Lobby { LocalMember = localMember };
        
        Assert.NotNull(sut.LocalMember);
        Assert.Equal(456, sut.LocalMember.SummonerId);
        Assert.True(sut.LocalMember.IsLeader);
    }
}

