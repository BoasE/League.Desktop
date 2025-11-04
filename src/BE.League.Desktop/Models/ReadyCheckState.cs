using System.Text.Json;
using System.Text.Json.Serialization;

namespace BE.League.Desktop.Models;

/// <summary>
/// Ready Check State innerhalb der Search State
/// </summary>
public sealed class ReadyCheck
{
    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("playerResponse")]
    public string? PlayerResponse { get; set; }

    [JsonPropertyName("dodgeWarning")]
    public string? DodgeWarning { get; set; }

    [JsonPropertyName("timer")]
    public float Timer { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}