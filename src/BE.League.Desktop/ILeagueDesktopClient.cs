using BE.League.Desktop.LcuClient;
using BE.League.Desktop.LiveClient;

namespace BE.League.Desktop;

public interface ILeagueDesktopClient
{
    LiveClientObjectReader LiveClient { get; }

    LcuObjectReader LcuClient { get; }
}