using System.Text.Json;
using BE.League.Desktop.Connection;
using BE.League.Desktop.Models;

namespace BE.League.Desktop.LcuClient;

/// <summary>
/// Object reader for LCU API (League Client Update)
/// Deserializes JSON responses into strongly-typed models
/// </summary>
public sealed class LcuObjectReader
{
    private readonly LcuApi _api;
    private readonly JsonSerializerOptions _jsonOptions;

    public LcuObjectReader(LcuApi? api = null, LeagueClientConnectionInfo? connectionInfo = null)
    {
        _api = api ?? new LcuApi(connectionInfo);
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
            System.Diagnostics.Debug.WriteLine($"LCU API JSON Error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get current lobby information
    /// </summary>
    public async Task<Lobby?> GetLobbyAsync(CancellationToken ct = default)
    {
        var json = await _api.GetLobbyJsonAsync(ct);
        return Deserialize<Lobby>(json);
    }

    /// <summary>
    /// Get current champion select session
    /// </summary>
    public async Task<ChampSelectSession?> GetChampSelectSessionAsync(CancellationToken ct = default)
    {
        var json = await _api.GetChampSelectSessionJsonAsync(ct);
        return Deserialize<ChampSelectSession>(json);
    }

    /// <summary>
    /// Get ready check state
    /// </summary>
    public async Task<ReadyCheckDto?> GetReadyCheckAsync(CancellationToken ct = default)
    {
        var json = await _api.GetReadyCheckJsonAsync(ct);
        return Deserialize<ReadyCheckDto>(json);
    }

    /// <summary>
    /// Accept ready check (when game is found)
    /// </summary>
    public async Task<bool> AcceptReadyCheckAsync(CancellationToken ct = default)
    {
        return await _api.AcceptReadyCheckAsync(ct);
    }

    /// <summary>
    /// Decline ready check
    /// </summary>
    public async Task<bool> DeclineReadyCheckAsync(CancellationToken ct = default)
    {
        return await _api.DeclineReadyCheckAsync(ct);
    }
}

