using BE.League.Desktop.Connection;
using BE.League.Desktop.LiveClient;
using BE.League.Desktop.LcuClient;

namespace BE.League.Desktop;

/// <summary>
/// Unified client for League of Legends Desktop integration
/// Provides access to both Live Client Data API (in-game) and LCU API (client/lobby)
/// </summary>
public sealed class LeagueDesktopClient : ILeagueDesktopClient
{
    public LiveClientObjectReader LiveClient { get; }
    public LcuObjectReader LcuClient { get; }
    
    public LeagueDesktopClient(LiveClientObjectReader liveClient, LcuObjectReader lcuClient)
    {
        LiveClient = liveClient;
        LcuClient = lcuClient;
    }
    
    public LeagueDesktopClient(LeagueDesktopOptions? options = null)
    {
        options ??= new LeagueDesktopOptions();
        
       
        LiveClient = new LiveClientObjectReader(options);
        
        var lcuApi = new LcuApi(options.Connection, options.Timeout);
        LcuClient = new LcuObjectReader(lcuApi);
    }
}