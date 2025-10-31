using System.Text.Json.Serialization;

namespace BE.League.Desktop.Models;

public sealed class MucJwt
{
    [JsonPropertyName("channelClaim")]
    public string? ChannelClaim { get; set; }
    
    [JsonPropertyName("domain")]
    public string? Domain { get; set; }
    
    [JsonPropertyName("jwt")]
    public string? Jwt { get; set; }
    
    [JsonPropertyName("targetRegion")]
    public string? TargetRegion { get; set; }
}

