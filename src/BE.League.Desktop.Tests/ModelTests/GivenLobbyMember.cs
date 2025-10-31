using BE.League.Desktop.Models;

namespace BE.League.Desktop.Tests.ModelTests;

public sealed class GivenLobbyMember
{
    [Fact]
    public void ItCanBeInstantiated()
    {
        var sut = new LobbyMember();
        
        Assert.NotNull(sut);
    }

    [Fact]
    public void ItCanSetSummonerId()
    {
        var sut = new LobbyMember { SummonerId = 12345L };
        
        Assert.Equal(12345L, sut.SummonerId);
    }

    [Fact]
    public void ItCanSetSummonerName()
    {
        var sut = new LobbyMember { SummonerName = "TestPlayer" };
        
        Assert.Equal("TestPlayer", sut.SummonerName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ItCanSetIsLeader(bool isLeader)
    {
        var sut = new LobbyMember { IsLeader = isLeader };
        
        Assert.Equal(isLeader, sut.IsLeader);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ItCanSetReady(bool ready)
    {
        var sut = new LobbyMember { Ready = ready };
        
        Assert.Equal(ready, sut.Ready);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ItCanSetIsBot(bool isBot)
    {
        var sut = new LobbyMember { IsBot = isBot };
        
        Assert.Equal(isBot, sut.IsBot);
    }

    [Fact]
    public void ItCanSetPuuid()
    {
        var puuid = "puuid-123-456-789";
        var sut = new LobbyMember { Puuid = puuid };
        
        Assert.Equal(puuid, sut.Puuid);
    }

    [Fact]
    public void ItCanSetPositionPreferences()
    {
        var sut = new LobbyMember 
        { 
            FirstPositionPreference = "TOP",
            SecondPositionPreference = "JUNGLE"
        };
        
        Assert.Equal("TOP", sut.FirstPositionPreference);
        Assert.Equal("JUNGLE", sut.SecondPositionPreference);
    }

    [Fact]
    public void ItCanSetAutoFillProperties()
    {
        var sut = new LobbyMember 
        { 
            AutoFillEligible = true,
            AutoFillProtectedForPromos = true
        };
        
        Assert.True(sut.AutoFillEligible);
        Assert.True(sut.AutoFillProtectedForPromos);
    }

    [Fact]
    public void ItCanSetBotProperties()
    {
        var sut = new LobbyMember 
        { 
            IsBot = true,
            BotChampionId = 103,
            BotDifficulty = "MEDIUM",
            BotPosition = "TOP"
        };
        
        Assert.True(sut.IsBot);
        Assert.Equal(103, sut.BotChampionId);
        Assert.Equal("MEDIUM", sut.BotDifficulty);
        Assert.Equal("TOP", sut.BotPosition);
    }
}

