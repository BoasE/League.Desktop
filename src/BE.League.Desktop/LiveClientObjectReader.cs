﻿using BE.League.Desktop.Connection;
using BE.League.Desktop.LiveClient;
using BE.League.Desktop.LcuClient;
using BE.League.Desktop.Models;

namespace BE.League.Desktop;

/// <summary>
/// Unified object reader for League of Legends Desktop integration
/// Provides access to both Live Client Data API and LCU API readers
/// Backward compatible wrapper that delegates to specialized readers
/// </summary>
public sealed class LiveClientObjectReader
{
    private readonly LiveClient.LiveClientObjectReader _liveReader;
    private readonly LcuObjectReader? _lcuReader;

    public LiveClientObjectReader(ILeagueDesktopClient gateway)
    {
        if (gateway is LeagueDesktopClient client)
        {
            _liveReader = new LiveClient.LiveClientObjectReader(client.Live);
            _lcuReader = client.Lcu != null ? new LcuObjectReader(client.Lcu) : null;
        }
        else
        {
            // Fallback for custom implementations
            _liveReader = new LiveClient.LiveClientObjectReader(new LiveClientApi());
            try
            {
                _lcuReader = new LcuObjectReader();
            }
            catch (InvalidOperationException)
            {
                _lcuReader = null;
            }
        }
    }

    public LiveClientObjectReader(LeagueDesktopOptions? options = null)
        : this(new LeagueDesktopClient(options))
    {
    }

    public LiveClientObjectReader() : this(new LeagueDesktopClient())
    {
    }

    /// <summary>
    /// Access to Live Client Data API reader (In-Game)
    /// </summary>
    public LiveClient.LiveClientObjectReader Live => _liveReader;

    /// <summary>
    /// Access to LCU API reader (Client/Lobby)
    /// Returns null if League Client is not running
    /// </summary>
    public LcuObjectReader? Lcu => _lcuReader;

    // ========== Live Client Data API Methods (Delegated for backward compatibility) ==========

    public Task<AllGameData?> GetAllGameDataAsync(CancellationToken ct = default)
        => _liveReader.GetAllGameDataAsync(ct);

    public Task<ActivePlayer?> GetActivePlayerAsync(CancellationToken ct = default)
        => _liveReader.GetActivePlayerAsync(ct);

    public Task<string?> GetActivePlayerNameAsync(CancellationToken ct = default)
        => _liveReader.GetActivePlayerNameAsync(ct);

    public Task<Abilities?> GetActivePlayerAbilitiesAsync(CancellationToken ct = default)
        => _liveReader.GetActivePlayerAbilitiesAsync(ct);

    public Task<FullRunes?> GetActivePlayerRunesAsync(CancellationToken ct = default)
        => _liveReader.GetActivePlayerRunesAsync(ct);

    public Task<List<Player>?> GetPlayerListAsync(CancellationToken ct = default)
        => _liveReader.GetPlayerListAsync(ct);

    public Task<Scores?> GetPlayerScoresAsync(string summonerName, CancellationToken ct = default)
        => _liveReader.GetPlayerScoresAsync(summonerName, ct);

    public Task<SummonerSpells?> GetPlayerSummonerSpellsAsync(string summonerName, CancellationToken ct = default)
        => _liveReader.GetPlayerSummonerSpellsAsync(summonerName, ct);

    public Task<PlayerRunes?> GetPlayerMainRunesAsync(string summonerName, CancellationToken ct = default)
        => _liveReader.GetPlayerMainRunesAsync(summonerName, ct);

    public Task<List<Item>?> GetPlayerItemsAsync(string summonerName, CancellationToken ct = default)
        => _liveReader.GetPlayerItemsAsync(summonerName, ct);

    public Task<Event?> GetEventDataAsync(CancellationToken ct = default)
        => _liveReader.GetEventDataAsync(ct);

    public Task<GameData?> GetGameStatsAsync(CancellationToken ct = default)
        => _liveReader.GetGameStatsAsync(ct);

    // ========== League Client API (LCU) Methods (Delegated for backward compatibility) ==========

    /// <summary>
    /// Get current lobby information
    /// </summary>
    public Task<Lobby?> GetLobbyAsync(CancellationToken ct = default)
        => _lcuReader?.GetLobbyAsync(ct) ?? Task.FromResult<Lobby?>(null);

    /// <summary>
    /// Get current champion select session
    /// </summary>
    public Task<ChampSelectSession?> GetChampSelectSessionAsync(CancellationToken ct = default)
        => _lcuReader?.GetChampSelectSessionAsync(ct) ?? Task.FromResult<ChampSelectSession?>(null);

    /// <summary>
    /// Get ready check state
    /// </summary>
    public Task<ReadyCheckDto?> GetReadyCheckAsync(CancellationToken ct = default)
        => _lcuReader?.GetReadyCheckAsync(ct) ?? Task.FromResult<ReadyCheckDto?>(null);

    /// <summary>
    /// Accept ready check (when game is found)
    /// </summary>
    public Task<bool> AcceptReadyCheckAsync(CancellationToken ct = default)
        => _lcuReader?.AcceptReadyCheckAsync(ct) ?? Task.FromResult(false);

    /// <summary>
    /// Decline ready check
    /// </summary>
    public Task<bool> DeclineReadyCheckAsync(CancellationToken ct = default)
        => _lcuReader?.DeclineReadyCheckAsync(ct) ?? Task.FromResult(false);
}

