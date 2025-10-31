using BE.League.Desktop;
using BE.League.Desktop.Models;
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public class GivenLiveClientObjectReader
{
    protected readonly ILeagueDesktopClient Gateway;
    protected readonly LiveClientObjectReader Sut;

    public GivenLiveClientObjectReader()
    {
        Gateway = A.Fake<ILeagueDesktopClient>();
        Sut = new LiveClientObjectReader(Gateway);
    }

    protected void SetupGatewayResponse<T>(Func<ILeagueDesktopClient, CancellationToken, Task<string?>> gatewayMethod, string jsonResponse)
    {
        A.CallTo(() => gatewayMethod(Gateway, A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(jsonResponse));
    }

    [Fact]
    public void ItCanBeConstructedWithGateway()
    {
        var gateway = A.Fake<ILeagueDesktopClient>();
        var sut = new LiveClientObjectReader(gateway);
        
        Assert.NotNull(sut);
    }

    [Fact]
    public void ItCanBeConstructedWithDefaultConstructor()
    {
        var sut = new LiveClientObjectReader();
        
        Assert.NotNull(sut);
    }
}

