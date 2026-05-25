using System.Text.Json;
using BE.League.Desktop.Connection;
using BE.League.Desktop.Models;

namespace BE.League.Desktop.LeagueClientApi;

/// <summary>
/// Typed reader for the League Client API (LCU).
/// <para>
/// Wraps <see cref="ILeagueClientApi"/> and deserializes raw JSON responses into
/// strongly-typed models using <c>System.Text.Json</c> with AOT-compatible source generation.
/// </para>
/// <para>
/// Official documentation:
/// <see href="https://developer.riotgames.com/docs/lol#league-client-api">
/// Riot Games — League Client API
/// </see>
/// </para>
/// </summary>
public sealed class LeagueClientApiReader
{
    private readonly ILeagueClientApi _api;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Exposes the underlying raw JSON API for direct access.
    /// </summary>
    public ILeagueClientApi Api => _api;

    public LeagueClientApiReader(ILeagueClientApi? api = null, LeagueClientConnectionInfo? connectionInfo = null)
    {
        _api = api ?? new LeagueClientApiClient(connectionInfo);
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = LeagueJsonContext.Default
        };
    }

    private T? Deserialize<T>(string? json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) return null;

        try
        {
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"League Client API JSON deserialization error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get current lobby information.
    /// <para>Endpoint: <c>GET /lol-lobby/v2/lobby</c></para>
    /// </summary>
    public async Task<Lobby?> GetLobbyAsync(CancellationToken ct = default)
    {
        var json = await _api.GetLobbyJsonAsync(ct);
        return Deserialize<Lobby>(json);
    }

    /// <summary>
    /// Get the current champion select session.
    /// <para>Endpoint: <c>GET /lol-champ-select/v1/session</c></para>
    /// </summary>
    public async Task<ChampSelectSession?> GetChampSelectSessionAsync(CancellationToken ct = default)
    {
        var json = await _api.GetChampSelectSessionJsonAsync(ct);
        return Deserialize<ChampSelectSession>(json);
    }

    /// <summary>
    /// Get the current ready-check state.
    /// <para>Endpoint: <c>GET /lol-matchmaking/v1/ready-check</c></para>
    /// </summary>
    public async Task<ReadyCheck?> GetReadyCheckAsync(CancellationToken ct = default)
    {
        var json = await _api.GetReadyCheckJsonAsync(ct);
        return Deserialize<ReadyCheck>(json);
    }

    /// <summary>
    /// Accept a ready check (when a game has been found).
    /// <para>Endpoint: <c>POST /lol-matchmaking/v1/ready-check/accept</c></para>
    /// </summary>
    public async Task<bool> AcceptReadyCheckAsync(CancellationToken ct = default)
    {
        return await _api.AcceptReadyCheckAsync(ct);
    }

    /// <summary>
    /// Decline a ready check.
    /// <para>Endpoint: <c>POST /lol-matchmaking/v1/ready-check/decline</c></para>
    /// </summary>
    public async Task<bool> DeclineReadyCheckAsync(CancellationToken ct = default)
    {
        return await _api.DeclineReadyCheckAsync(ct);
    }
}

