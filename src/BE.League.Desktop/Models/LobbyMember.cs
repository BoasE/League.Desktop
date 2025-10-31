﻿using System.Text.Json.Serialization;

 namespace BE.League.Desktop.Models;

public sealed class LobbyMember 
{
    [JsonPropertyName("allowedChangeActivity")]
    public bool AllowedChangeActivity { get; set; }
    
    [JsonPropertyName("allowedInviteOthers")]
    public bool AllowedInviteOthers { get; set; }
    
    [JsonPropertyName("allowedKickOthers")]
    public bool AllowedKickOthers { get; set; }
    
    [JsonPropertyName("allowedStartActivity")]
    public bool AllowedStartActivity { get; set; }
    
    [JsonPropertyName("allowedToggleInvite")]
    public bool AllowedToggleInvite { get; set; }
    
    [JsonPropertyName("autoFillEligible")]
    public bool AutoFillEligible { get; set; }
    
    [JsonPropertyName("autoFillProtectedForPromos")]
    public bool AutoFillProtectedForPromos { get; set; }
    
    [JsonPropertyName("autoFillProtectedForRemedy")]
    public bool AutoFillProtectedForRemedy { get; set; }
    
    [JsonPropertyName("autoFillProtectedForSoloing")]
    public bool AutoFillProtectedForSoloing { get; set; }
    
    [JsonPropertyName("autoFillProtectedForStreaking")]
    public bool AutoFillProtectedForStreaking { get; set; }
    
    [JsonPropertyName("botChampionId")]
    public int BotChampionId { get; set; }
    
    [JsonPropertyName("botDifficulty")]
    public string? BotDifficulty { get; set; }
    
    [JsonPropertyName("botId")]
    public string? BotId { get; set; }
    
    [JsonPropertyName("botPosition")]
    public string? BotPosition { get; set; }
    
    [JsonPropertyName("botUuid")]
    public string? BotUuid { get; set; }
    
    [JsonPropertyName("firstPositionPreference")]
    public string? FirstPositionPreference { get; set; }
    
    [JsonPropertyName("intraSubteamPosition")]
    public int? IntraSubteamPosition { get; set; }
    
    [JsonPropertyName("isBot")]
    public bool IsBot { get; set; }
    
    [JsonPropertyName("isLeader")]
    public bool IsLeader { get; set; }
    
    [JsonPropertyName("isSpectator")]
    public bool IsSpectator { get; set; }
    
    [JsonPropertyName("memberData")]
    public object? MemberData { get; set; }
    
    [JsonPropertyName("playerSlots")]
    public List<object>? PlayerSlots { get; set; }
    
    [JsonPropertyName("puuid")]
    public string? Puuid { get; set; }
    
    [JsonPropertyName("ready")]
    public bool Ready { get; set; }
    
    [JsonPropertyName("secondPositionPreference")]
    public string? SecondPositionPreference { get; set; }
    
    [JsonPropertyName("showGhostedBanner")]
    public bool ShowGhostedBanner { get; set; }
    
    [JsonPropertyName("strawberryMapId")]
    public int? StrawberryMapId { get; set; }
    
    [JsonPropertyName("subteamIndex")]
    public int? SubteamIndex { get; set; }
    
    [JsonPropertyName("summonerIconId")]
    public int SummonerIconId { get; set; }
    
    [JsonPropertyName("summonerId")]
    public long SummonerId { get; set; }
    
    [JsonPropertyName("summonerInternalName")]
    public string? SummonerInternalName { get; set; }
    
    [JsonPropertyName("summonerLevel")]
    public int SummonerLevel { get; set; }
    
    [JsonPropertyName("summonerName")]
    public string? SummonerName { get; set; }
    
    [JsonPropertyName("teamId")]
    public int TeamId { get; set; }
}