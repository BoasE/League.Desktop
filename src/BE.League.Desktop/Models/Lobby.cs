using System.Text.Json.Serialization;

namespace BE.League.Desktop.Models;

public sealed class Lobby
{
    [JsonPropertyName("canStartActivity")]
    public bool CanStartActivity { get; set; }
    
    [JsonPropertyName("gameConfig")]
    public GameConfig? GameConfig { get; set; }
    
    [JsonPropertyName("invitations")]
    public List<LobbyInvitation>? Invitations { get; set; }
    
    [JsonPropertyName("localMember")]
    public LobbyMember? LocalMember { get; set; }
    
    [JsonPropertyName("members")]
    public LobbyMember[] Members { get; set; } = Array.Empty<LobbyMember>();
    
    [JsonPropertyName("mucJwtDto")]
    public MucJwt? MucJwtDto { get; set; }
    
    [JsonPropertyName("multiUserChatId")]
    public string? MultiUserChatId { get; set; }
    
    [JsonPropertyName("multiUserChatPassword")]
    public string? MultiUserChatPassword { get; set; }
    
    [JsonPropertyName("partyId")]
    public string? PartyId { get; set; }
    
    [JsonPropertyName("partyType")]
    public string? PartyType { get; set; }
    
    [JsonPropertyName("popularChampions")]
    public List<object>? PopularChampions { get; set; }
    
    [JsonPropertyName("restrictions")]
    public List<object>? Restrictions { get; set; }
    
    [JsonPropertyName("scarcePositions")]
    public List<string>? ScarcePositions { get; set; }
    
    [JsonPropertyName("warnings")]
    public List<object>? Warnings { get; set; }
}

