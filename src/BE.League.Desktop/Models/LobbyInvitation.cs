using System.Text.Json.Serialization;

namespace BE.League.Desktop.Models;

public sealed class LobbyInvitation
{
    [JsonPropertyName("invitationId")]
    public string? InvitationId { get; set; }
    
    [JsonPropertyName("invitationType")]
    public string? InvitationType { get; set; }
    
    [JsonPropertyName("state")]
    public string? State { get; set; }
    
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }
    
    [JsonPropertyName("toSummonerId")]
    public long ToSummonerId { get; set; }
    
    [JsonPropertyName("toSummonerName")]
    public string? ToSummonerName { get; set; }
}

