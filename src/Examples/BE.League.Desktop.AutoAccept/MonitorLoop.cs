using BE.League.Desktop.GameClientApi;
using BE.League.Desktop.LeagueClientApi;
using BE.League.Desktop.Models;
using Spectre.Console;

namespace BE.League.Desktop.AutoAccept;

public static class MonitorLoop
{
    private static readonly GameClientApiReader _gameClient = new();
    private static readonly LeagueClientApiReader _leagueClient = new();

    public static async Task Run(CancellationToken cancellationToken)
    {
        await MonitorLobby(cancellationToken);
    }

    private static async Task MonitorLobby(CancellationToken cancellationToken)
    {
        var started = new DateTimeOffset();
        var acceptCount = 0;

        Lobby? lobbyDto = await _leagueClient.GetLobbyAsync(cancellationToken);

        await AnsiConsole
            .Live(Displays.CreateStatusTable(lobbyDto, started, acceptCount))
            .StartAsync(async ctx =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);

                        if (key.Key == ConsoleKey.Escape)
                        {
                            return;
                        }
                    }

                    lobbyDto = await _leagueClient.GetLobbyAsync(cancellationToken);
                    ctx.UpdateTarget(Displays.CreateStatusTable(lobbyDto, started, acceptCount));

                    try
                    {
                        await ReadCheck(cancellationToken, ctx, acceptCount);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        // Ignore errors, continue checking
                    }

                    await Task.Delay(500, cancellationToken);
                }
            });
    }

    private static async Task ReadCheck(CancellationToken cancellationToken,
        LiveDisplayContext ctx, int acceptCount)
    {
        ReadyCheck? readyCheck = await _leagueClient.GetReadyCheckAsync(cancellationToken);

        if (CanClickAccept(readyCheck))
        {
            acceptCount++;

            Displays.WriteGameFound(acceptCount, ctx);

            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

            readyCheck = await _leagueClient.GetReadyCheckAsync(cancellationToken);

            if (CanClickAccept(readyCheck))
            {
                await Accept(cancellationToken, ctx);
            }
        }
    }

    private static async Task Accept(CancellationToken cancellationToken, LiveDisplayContext ctx)
    {
        Displays.WriteAcceptingNotification(ctx);

        var accepted = await _leagueClient.AcceptReadyCheckAsync(cancellationToken);

        if (accepted)
        {
            Displays.WriteSuccessNotification(ctx);
            await Task.Delay(2000, cancellationToken);
        }
        else
        {
            await Displays.WriteError(cancellationToken, ctx);
        }
    }


    private static bool CanClickAccept(ReadyCheck? readyCheck)
    {
        return readyCheck is { State: "InProgress" };
    }
}