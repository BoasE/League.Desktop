using BE.League.Desktop.LcuClient;
using BE.League.Desktop.LiveClient;
using BE.League.Desktop.Models;
using Spectre.Console;

namespace BE.League.Desktop.AutoAccept;

public static class MonitorLoop
{
    private static readonly LiveClientObjectReader _reader = new();
    private static readonly LcuObjectReader _lcu = new();

    public static async Task Run(CancellationToken cancellationToken)
    {
        await MonitorLobby(cancellationToken);
    }

    private static async Task MonitorLobby(CancellationToken cancellationToken)
    {
        var started = new DateTimeOffset();
        var acceptCount = 0;

        Lobby? lobbyDto = await _lcu.GetLobbyAsync(cancellationToken);

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

                    lobbyDto = await _lcu.GetLobbyAsync(cancellationToken);
                    ctx.UpdateTarget(Displays.CreateStatusTable(lobbyDto, started, acceptCount));

                    try
                    {
                        await ReadCheck(_reader, cancellationToken, ctx, acceptCount);
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

    private static async Task ReadCheck(LiveClientObjectReader reader, CancellationToken cancellationToken,
        LiveDisplayContext ctx, int acceptCount)
    {
        ReadyCheck? readyCheck;
        readyCheck = await _lcu.GetReadyCheckAsync(cancellationToken);

        if (CanClickAccept(readyCheck))
        {
            acceptCount++;

            Displays.WriteGameFound(acceptCount, ctx);


            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

            readyCheck = await _lcu.GetReadyCheckAsync(cancellationToken);

            if (CanClickAccept(readyCheck))
            {
                await Accept(reader, cancellationToken, ctx);
            }
        }
    }


    private static async Task Accept(LiveClientObjectReader reader, CancellationToken cancellationToken,
        LiveDisplayContext ctx)
    {
        Displays.WriteAcceptingNotification(ctx);

        var accepted = await _lcu.AcceptReadyCheckAsync(cancellationToken);

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