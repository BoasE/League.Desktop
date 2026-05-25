using System.Text;
using BE.League.Desktop.Connection;

namespace BE.League.Desktop.LeagueClientApi;

/// <summary>
/// HTTP client for the League Client API (LCU).
/// <para>
/// Connects to the running League of Legends desktop client via a <b>dynamic port</b>
/// discovered from the lockfile. Uses <c>Authorization: Basic riot:&lt;token&gt;</c>
/// and accepts the client's self-signed HTTPS certificate.
/// </para>
/// <para>
/// The client is only available while the League Client process is running.
/// Connection details are read from the lockfile at:<br/>
/// Windows: <c>C:\Riot Games\League of Legends\lockfile</c>
/// </para>
/// <para>
/// Official documentation:
/// <see href="https://developer.riotgames.com/docs/lol#league-client-api">
/// Riot Games — League Client API
/// </see>
/// </para>
/// <para>
/// Community endpoint reference:
/// <see href="https://hextechdocs.dev/">HexTech Docs</see>
/// </para>
/// </summary>
public sealed class LeagueClientApiClient : IDisposable, ILeagueClientApi
{
    private readonly HttpClient _httpClient;
    private readonly LeagueClientConnectionInfo _connectionInfo;

    public LeagueClientApiClient(LeagueClientConnectionInfo? connectionInfo = null, TimeSpan? timeout = null)
    {
        _connectionInfo = connectionInfo ?? LeagueClientConnectionInfo.GetFromRunningClient()
            ?? throw new InvalidOperationException(
                "League Client is not running or lockfile not found. " +
                "Ensure the League of Legends client is open before instantiating LeagueClientApiClient.");

        var handler = new HttpClientHandler
        {
            // The League Client uses a self-signed certificate; we bypass validation for localhost.
            ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                _ = sender;
                _ = certificate;
                _ = chain;
                _ = sslPolicyErrors;
                return true;
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
    /// Get current lobby information.
    /// <para>Endpoint: <c>GET /lol-lobby/v2/lobby</c></para>
    /// </summary>
    public Task<string?> GetLobbyJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/lol-lobby/v2/lobby", ct);

    /// <summary>
    /// Get the current champion select session.
    /// <para>Endpoint: <c>GET /lol-champ-select/v1/session</c></para>
    /// </summary>
    public Task<string?> GetChampSelectSessionJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/lol-champ-select/v1/session", ct);

    /// <summary>
    /// Get the current ready-check state.
    /// <para>Endpoint: <c>GET /lol-matchmaking/v1/ready-check</c></para>
    /// </summary>
    public Task<string?> GetReadyCheckJsonAsync(CancellationToken ct = default)
        => GetJsonAsync("/lol-matchmaking/v1/ready-check", ct);

    /// <summary>
    /// Accept a ready check (when a game has been found).
    /// <para>Endpoint: <c>POST /lol-matchmaking/v1/ready-check/accept</c></para>
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
    /// Decline a ready check.
    /// <para>Endpoint: <c>POST /lol-matchmaking/v1/ready-check/decline</c></para>
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
            // League Client not available or endpoint not accessible in current state
            return null;
        }
        catch (TaskCanceledException)
        {
            // Timeout or cancellation requested
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"League Client API error: {ex.Message}");
            return null;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

