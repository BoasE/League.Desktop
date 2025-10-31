﻿using System.Text.Json.Serialization;

 namespace BE.League.Desktop.Models;

public sealed class GameConfig 
{
    [JsonPropertyName("allowablePremadeSizes")]
    public List<int>? AllowablePremadeSizes { get; set; }
    
    [JsonPropertyName("customLobbyName")]
    public string? CustomLobbyName { get; set; }
    
    [JsonPropertyName("customMutatorName")]
    public string? CustomMutatorName { get; set; }
    
    [JsonPropertyName("customRewardsDisabledReasons")]
    public List<string>? CustomRewardsDisabledReasons { get; set; }
    
    [JsonPropertyName("customSpectatorPolicy")]
    public string? CustomSpectatorPolicy { get; set; }
    
    [JsonPropertyName("customSpectators")]
    public List<object>? CustomSpectators { get; set; }
    
    [JsonPropertyName("customTeam100")]
    public List<LobbyMember>? CustomTeam100 { get; set; }
    
    [JsonPropertyName("customTeam200")]
    public List<LobbyMember>? CustomTeam200 { get; set; }
    
    [JsonPropertyName("gameMode")]
    public string? GameMode { get; set; }
    
    [JsonPropertyName("isCustom")]
    public bool IsCustom { get; set; }
    
    [JsonPropertyName("isLobbyFull")]
    public bool IsLobbyFull { get; set; }
    
    [JsonPropertyName("isTeamBuilderManaged")]
    public bool IsTeamBuilderManaged { get; set; }
    
    [JsonPropertyName("mapId")]
    public int MapId { get; set; }
    
    [JsonPropertyName("maxHumanPlayers")]
    public int MaxHumanPlayers { get; set; }
    
    [JsonPropertyName("maxLobbySize")]
    public int MaxLobbySize { get; set; }
    
    [JsonPropertyName("maxLobbySpectatorCount")]
    public int MaxLobbySpectatorCount { get; set; }
    
    [JsonPropertyName("maxTeamSize")]
    public int MaxTeamSize { get; set; }
    
    [JsonPropertyName("numPlayersPerTeam")]
    public int NumPlayersPerTeam { get; set; }
    
    [JsonPropertyName("numberOfTeamsInLobby")]
    public int NumberOfTeamsInLobby { get; set; }
    
    [JsonPropertyName("pickType")]
    public string? PickType { get; set; }
    
    [JsonPropertyName("premadeSizeAllowed")]
    public bool PremadeSizeAllowed { get; set; }
    
    [JsonPropertyName("queueId")]
    public int? QueueId { get; set; }
    
    [JsonPropertyName("shouldForceScarcePositionSelection")]
    public bool ShouldForceScarcePositionSelection { get; set; }
    
    [JsonPropertyName("showPositionSelector")]
    public bool ShowPositionSelector { get; set; }
    
    [JsonPropertyName("showQuickPlaySlotSelection")]
    public bool ShowQuickPlaySlotSelection { get; set; }
}