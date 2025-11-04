using System.Text.Json;
using System.Text.Json.Serialization;

namespace BE.League.Desktop.Models;

/// <summary>
/// Summoner-Informationen
/// GET /lol-summoner/v1/current-summoner
/// </summary>
public class Summoner
{
    [JsonPropertyName("accountId")]
    public long AccountId { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("internalName")]
    public string? InternalName { get; set; }

    [JsonPropertyName("nameChangeFlag")]
    public bool NameChangeFlag { get; set; }

    [JsonPropertyName("percentCompleteForNextLevel")]
    public int PercentCompleteForNextLevel { get; set; }

    [JsonPropertyName("profileIconId")]
    public int ProfileIconId { get; set; }

    [JsonPropertyName("puuid")]
    public string? Puuid { get; set; }

    [JsonPropertyName("rerollPoints")]
    public RerollPoints? RerollPoints { get; set; }

    [JsonPropertyName("summonerId")]
    public long SummonerId { get; set; }

    [JsonPropertyName("summonerLevel")]
    public int SummonerLevel { get; set; }

    [JsonPropertyName("unnamed")]
    public bool Unnamed { get; set; }

    [JsonPropertyName("xpSinceLastLevel")]
    public int XpSinceLastLevel { get; set; }

    [JsonPropertyName("xpUntilNextLevel")]
    public int XpUntilNextLevel { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Reroll-Punkte für ARAM
/// </summary>
public class RerollPoints
{
    [JsonPropertyName("currentPoints")]
    public int CurrentPoints { get; set; }

    [JsonPropertyName("maxRolls")]
    public int MaxRolls { get; set; }

    [JsonPropertyName("numberOfRolls")]
    public int NumberOfRolls { get; set; }

    [JsonPropertyName("pointsCostToRoll")]
    public int PointsCostToRoll { get; set; }

    [JsonPropertyName("pointsToReroll")]
    public int PointsToReroll { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Game Flow Session - Komplette Session-Informationen
/// GET /lol-gameflow/v1/session
/// </summary>
public class GameFlowSession
{
    [JsonPropertyName("phase")]
    public string? Phase { get; set; }

    [JsonPropertyName("gameData")]
    public GameFlowGameData? GameData { get; set; }

    [JsonPropertyName("map")]
    public GameFlowMap? Map { get; set; }

    [JsonPropertyName("gameClient")]
    public GameFlowClient? GameClient { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Game Flow - Spieldaten
/// </summary>
public class GameFlowGameData
{
    [JsonPropertyName("gameId")]
    public long GameId { get; set; }

    [JsonPropertyName("gameName")]
    public string? GameName { get; set; }

    [JsonPropertyName("isCustomGame")]
    public bool IsCustomGame { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }

    [JsonPropertyName("playerChampionSelections")]
    public List<PlayerChampionSelection>? PlayerChampionSelections { get; set; }

    [JsonPropertyName("queue")]
    public QueueInfo? Queue { get; set; }

    [JsonPropertyName("spectatorsAllowed")]
    public bool SpectatorsAllowed { get; set; }

    [JsonPropertyName("teamOne")]
    public List<TeamMemberInfo>? TeamOne { get; set; }

    [JsonPropertyName("teamTwo")]
    public List<TeamMemberInfo>? TeamTwo { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Spieler Champion-Auswahl
/// </summary>
public class PlayerChampionSelection
{
    [JsonPropertyName("championId")]
    public int ChampionId { get; set; }

    [JsonPropertyName("selectedSkinIndex")]
    public int SelectedSkinIndex { get; set; }

    [JsonPropertyName("spell1Id")]
    public long Spell1Id { get; set; }

    [JsonPropertyName("spell2Id")]
    public long Spell2Id { get; set; }

    [JsonPropertyName("summonerInternalName")]
    public string? SummonerInternalName { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Queue-Informationen
/// </summary>
public class QueueInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("shortName")]
    public string? ShortName { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("gameMode")]
    public string? GameMode { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Team-Mitglied Informationen
/// </summary>
public class TeamMemberInfo
{
    [JsonPropertyName("summonerId")]
    public long SummonerId { get; set; }

    [JsonPropertyName("summonerInternalName")]
    public string? SummonerInternalName { get; set; }

    [JsonPropertyName("summonerName")]
    public string? SummonerName { get; set; }

    [JsonPropertyName("teamId")]
    public int TeamId { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Game Flow Map-Informationen
/// </summary>
public class GameFlowMap
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("gameMode")]
    public string? GameMode { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Game Client Status
/// </summary>
public class GameFlowClient
{
    [JsonPropertyName("running")]
    public bool Running { get; set; }

    [JsonPropertyName("visible")]
    public bool Visible { get; set; }

    [JsonPropertyName("observerServerIp")]
    public string? ObserverServerIp { get; set; }

    [JsonPropertyName("observerServerPort")]
    public int ObserverServerPort { get; set; }

    [JsonPropertyName("serverIp")]
    public string? ServerIp { get; set; }

    [JsonPropertyName("serverPort")]
    public int ServerPort { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Matchmaking Search State
/// GET /lol-lobby/v2/lobby/matchmaking/search-state
/// </summary>
public class SearchState
{
    [JsonPropertyName("searchState")]
    public string? State { get; set; }

    [JsonPropertyName("timeInQueue")]
    public float TimeInQueue { get; set; }

    [JsonPropertyName("estimatedQueueTime")]
    public float EstimatedQueueTime { get; set; }

    [JsonPropertyName("readyCheck")]
    public ReadyCheck? ReadyCheck { get; set; }

    [JsonPropertyName("errors")]
    public List<SearchError>? Errors { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Search Error - Fehler beim Matchmaking
/// </summary>
public class SearchError
{
    [JsonPropertyName("errorType")]
    public string? ErrorType { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

