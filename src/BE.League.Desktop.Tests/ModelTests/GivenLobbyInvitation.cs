using BE.League.Desktop.Models;

namespace BE.League.Desktop.Tests.ModelTests;

public sealed class GivenLobbyInvitation
{
    [Fact]
    public void ItCanBeInstantiated()
    {
        var sut = new LobbyInvitation();
        
        Assert.NotNull(sut);
    }

    [Fact]
    public void ItCanSetInvitationId()
    {
        var sut = new LobbyInvitation { InvitationId = "inv-123" };
        
        Assert.Equal("inv-123", sut.InvitationId);
    }

    [Fact]
    public void ItCanSetInvitationType()
    {
        var sut = new LobbyInvitation { InvitationType = "party" };
        
        Assert.Equal("party", sut.InvitationType);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("Accepted")]
    [InlineData("Declined")]
    public void ItCanSetState(string state)
    {
        var sut = new LobbyInvitation { State = state };
        
        Assert.Equal(state, sut.State);
    }

    [Fact]
    public void ItCanSetToSummonerId()
    {
        var sut = new LobbyInvitation { ToSummonerId = 12345L };
        
        Assert.Equal(12345L, sut.ToSummonerId);
    }

    [Fact]
    public void ItCanSetToSummonerName()
    {
        var sut = new LobbyInvitation { ToSummonerName = "Player1" };
        
        Assert.Equal("Player1", sut.ToSummonerName);
    }
}

