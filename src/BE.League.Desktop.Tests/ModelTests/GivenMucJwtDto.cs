using BE.League.Desktop.Models;

namespace BE.League.Desktop.Tests.ModelTests;

public sealed class GivenMucJwtDto
{
    [Fact]
    public void ItCanBeInstantiated()
    {
        var sut = new MucJwt();
        
        Assert.NotNull(sut);
    }

    [Fact]
    public void ItCanSetChannelClaim()
    {
        var sut = new MucJwt { ChannelClaim = "channel-123" };
        
        Assert.Equal("channel-123", sut.ChannelClaim);
    }

    [Fact]
    public void ItCanSetDomain()
    {
        var sut = new MucJwt { Domain = "euw.pvp.net" };
        
        Assert.Equal("euw.pvp.net", sut.Domain);
    }

    [Fact]
    public void ItCanSetJwt()
    {
        var jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
        var sut = new MucJwt { Jwt = jwt };
        
        Assert.Equal(jwt, sut.Jwt);
    }

    [Fact]
    public void ItCanSetTargetRegion()
    {
        var sut = new MucJwt { TargetRegion = "EUW1" };
        
        Assert.Equal("EUW1", sut.TargetRegion);
    }


    [Fact]
    public void ItCanSetEventId()
    {
        var sut = new LiveEvent { EventID = 42 };
        
        Assert.Equal(42, sut.EventID);
    }

    [Fact]
    public void ItCanSetEventName()
    {
        var sut = new LiveEvent { EventName = "ChampionKill" };
        
        Assert.Equal("ChampionKill", sut.EventName);
    }

    [Fact]
    public void ItCanSetEventTime()
    {
        var sut = new LiveEvent { EventTime = 123.5f };
        
        Assert.Equal(123.5f, sut.EventTime);
    }

    [Fact]
    public void ItCanSetKillerName()
    {
        var sut = new LiveEvent { KillerName = "Player1" };
        
        Assert.Equal("Player1", sut.KillerName);
    }

    [Fact]
    public void ItCanSetVictimName()
    {
        var sut = new LiveEvent { VictimName = "Player2" };
        
        Assert.Equal("Player2", sut.VictimName);
    }

    [Fact]
    public void ItCanSetDragonType()
    {
        var sut = new LiveEvent { DragonType = "Cloud" };
        
        Assert.Equal("Cloud", sut.DragonType);
    }

    [Fact]
    public void ItCanSetAssisters()
    {
        var assisters = new List<string> { "Player3", "Player4" };
        var sut = new LiveEvent { Assisters = assisters };
        
        Assert.Equal(assisters, sut.Assisters);
    }
}

