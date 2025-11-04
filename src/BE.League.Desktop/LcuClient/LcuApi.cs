using System.Text;
using BE.League.Desktop.Connection;

namespace BE.League.Desktop.LcuClient;

/// <summary>
/// API client for League Client Update (LCU) API
/// Provides access to lobby, champion select, and matchmaking features
/// Requires connection info from running League Client (lockfile)
/// </summary>
public sealed class LcuApi : IDisposable, ILcuApi
{
    private readonly HttpClient _httpClient;
    private readonly LeagueClientConnectionInfo _connectionInfo;

    public LcuApi(LeagueClientConnectionInfo? connectionInfo = null, TimeSpan? timeout = null)
    {
        _connectionInfo = connectionInfo ?? LeagueClientConnectionInfo.GetFromRunningClient()
            ?? throw new InvalidOperationException("League Client is not running or lockfile not found");

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                _ = sender;
                _ = certificate;
                _ = chain;
                _ = sslPolicyErrors;
                return true; // Accept self-signed certificates
            }
        };

        var credentials = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"riot:{_connectionInfo.Token}"));

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(_connectionInfo.GetBaseUrl()),
            Timeout = timeout ?? TimeSpan.FromSeconds(10)
        };

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");
    }

    /// <summary>
    /// Get current lobby information
    /// </summary>
    public Task<string?> GetLobbyJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/lol-lobby/v2/lobby", ct);

    /// <summary>
    /// Get current champion select session
    /// </summary>
    public Task<string?> GetChampSelectSessionJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/lol-champ-select/v1/session", ct);

    /// <summary>
    /// Get ready check state
    /// </summary>
    public Task<string?> GetReadyCheckJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/lol-matchmaking/v1/ready-check", ct);

    /// <summary>
    /// Accept ready check (when game is found)
    /// </summary>
    public async Task<bool> AcceptReadyCheckAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                "/lol-matchmaking/v1/ready-check/accept",
                null,
                ct);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Decline ready check
    /// </summary>
    public async Task<bool> DeclineReadyCheckAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                "/lol-matchmaking/v1/ready-check/decline",
                null,
                ct);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<string?> GetJsonAsync(string endpoint, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint, ct);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return json;
        }
        catch (HttpRequestException)
        {
            // LCU not available
            return null;
        }
        catch (TaskCanceledException)
        {
            // Timeout or Cancellation
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LCU API Error: {ex.Message}");
            return null;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

