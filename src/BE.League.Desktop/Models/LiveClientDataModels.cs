using System.Text.Json;
using System.Text.Json.Serialization;

namespace BE.League.Desktop.Models;

/// <summary>
/// Alle Spieldaten - Response von /liveclientdata/allgamedata
/// </summary>
public class AllGameData
{
    [JsonPropertyName("activePlayer")]
    public ActivePlayer? ActivePlayer { get; set; }

    [JsonPropertyName("allPlayers")]
    public List<Player>? AllPlayers { get; set; }

    [JsonPropertyName("events")]
    public Event? Events { get; set; }

    [JsonPropertyName("gameData")]
    public GameData? GameData { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Aktiver Spieler - der Spieler an diesem Computer
/// </summary>
public class ActivePlayer
{
    [JsonPropertyName("abilities")]
    public Abilities? Abilities { get; set; }

    [JsonPropertyName("championStats")]
    public ChampionStats? ChampionStats { get; set; }

    [JsonPropertyName("currentGold")]
    public float CurrentGold { get; set; }

    [JsonPropertyName("fullRunes")]
    public FullRunes? FullRunes { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("summonerName")]
    public string? SummonerName { get; set; }

    [JsonPropertyName("teamRelativeColors")]
    public bool TeamRelativeColors { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Fähigkeiten (Q, W, E, R, Passiv)
/// </summary>
public class Abilities
{
    [JsonPropertyName("Passive")]
    public Ability? Passive { get; set; }

    [JsonPropertyName("Q")]
    public Ability? Q { get; set; }

    [JsonPropertyName("W")]
    public Ability? W { get; set; }

    [JsonPropertyName("E")]
    public Ability? E { get; set; }

    [JsonPropertyName("R")]
    public Ability? R { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Einzelne Fähigkeit
/// </summary>
public class Ability
{
    [JsonPropertyName("abilityLevel")]
    public int AbilityLevel { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("rawDescription")]
    public string? RawDescription { get; set; }

    [JsonPropertyName("rawDisplayName")]
    public string? RawDisplayName { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Champion-Statistiken
/// </summary>
public class ChampionStats
{
    [JsonPropertyName("abilityHaste")]
    public float AbilityHaste { get; set; }

    [JsonPropertyName("abilityPower")]
    public float AbilityPower { get; set; }

    [JsonPropertyName("armor")]
    public float Armor { get; set; }

    [JsonPropertyName("armorPenetrationFlat")]
    public float ArmorPenetrationFlat { get; set; }

    [JsonPropertyName("armorPenetrationPercent")]
    public float ArmorPenetrationPercent { get; set; }

    [JsonPropertyName("attackDamage")]
    public float AttackDamage { get; set; }

    [JsonPropertyName("attackRange")]
    public float AttackRange { get; set; }

    [JsonPropertyName("attackSpeed")]
    public float AttackSpeed { get; set; }

    [JsonPropertyName("bonusArmorPenetrationPercent")]
    public float BonusArmorPenetrationPercent { get; set; }

    [JsonPropertyName("bonusMagicPenetrationPercent")]
    public float BonusMagicPenetrationPercent { get; set; }

    [JsonPropertyName("critChance")]
    public float CritChance { get; set; }

    [JsonPropertyName("critDamage")]
    public float CritDamage { get; set; }

    [JsonPropertyName("currentHealth")]
    public float CurrentHealth { get; set; }

    [JsonPropertyName("healShieldPower")]
    public float HealShieldPower { get; set; }

    [JsonPropertyName("healthRegenRate")]
    public float HealthRegenRate { get; set; }

    [JsonPropertyName("lifeSteal")]
    public float LifeSteal { get; set; }

    [JsonPropertyName("magicLethality")]
    public float MagicLethality { get; set; }

    [JsonPropertyName("magicPenetrationFlat")]
    public float MagicPenetrationFlat { get; set; }

    [JsonPropertyName("magicPenetrationPercent")]
    public float MagicPenetrationPercent { get; set; }

    [JsonPropertyName("magicResist")]
    public float MagicResist { get; set; }

    [JsonPropertyName("maxHealth")]
    public float MaxHealth { get; set; }

    [JsonPropertyName("moveSpeed")]
    public float MoveSpeed { get; set; }

    [JsonPropertyName("omnivamp")]
    public float Omnivamp { get; set; }

    [JsonPropertyName("physicalLethality")]
    public float PhysicalLethality { get; set; }

    [JsonPropertyName("physicalVamp")]
    public float PhysicalVamp { get; set; }

    [JsonPropertyName("resourceMax")]
    public float ResourceMax { get; set; }

    [JsonPropertyName("resourceRegenRate")]
    public float ResourceRegenRate { get; set; }

    [JsonPropertyName("resourceType")]
    public string? ResourceType { get; set; }

    [JsonPropertyName("resourceValue")]
    public float ResourceValue { get; set; }

    [JsonPropertyName("spellVamp")]
    public float SpellVamp { get; set; }

    [JsonPropertyName("tenacity")]
    public float Tenacity { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Vollständige Runen-Konfiguration
/// </summary>
public class FullRunes
{
    [JsonPropertyName("generalRunes")]
    public List<Rune>? GeneralRunes { get; set; }

    [JsonPropertyName("keystone")]
    public Rune? Keystone { get; set; }

    [JsonPropertyName("primaryRuneTree")]
    public RuneTree? PrimaryRuneTree { get; set; }

    [JsonPropertyName("secondaryRuneTree")]
    public RuneTree? SecondaryRuneTree { get; set; }

    [JsonPropertyName("statRunes")]
    public List<StatRune>? StatRunes { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Einzelne Rune
/// </summary>
public class Rune
{
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("rawDescription")]
    public string? RawDescription { get; set; }

    [JsonPropertyName("rawDisplayName")]
    public string? RawDisplayName { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Runen-Baum (z.B. Dominanz, Präzision)
/// </summary>
public class RuneTree
{
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("rawDescription")]
    public string? RawDescription { get; set; }

    [JsonPropertyName("rawDisplayName")]
    public string? RawDisplayName { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Stat-Rune (Anpassung, Flex, Verteidigung)
/// </summary>
public class StatRune
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("rawDescription")]
    public string? RawDescription { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Spieler im Spiel
/// </summary>
public class Player
{
    [JsonPropertyName("championName")]
    public string? ChampionName { get; set; }

    [JsonPropertyName("isBot")]
    public bool IsBot { get; set; }

    [JsonPropertyName("isDead")]
    public bool IsDead { get; set; }

    [JsonPropertyName("items")]
    public List<Item>? Items { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("position")]
    public string? Position { get; set; }

    [JsonPropertyName("rawChampionName")]
    public string? RawChampionName { get; set; }

    [JsonPropertyName("rawSkinName")]
    public string? RawSkinName { get; set; }

    [JsonPropertyName("respawnTimer")]
    public float RespawnTimer { get; set; }

    [JsonPropertyName("runes")]
    public PlayerRunes? Runes { get; set; }

    [JsonPropertyName("scores")]
    public Scores? Scores { get; set; }

    [JsonPropertyName("skinID")]
    public int SkinId { get; set; }

    [JsonPropertyName("skinName")]
    public string? SkinName { get; set; }

    [JsonPropertyName("summonerName")]
    public string? SummonerName { get; set; }

    [JsonPropertyName("summonerSpells")]
    public SummonerSpells? SummonerSpells { get; set; }

    [JsonPropertyName("team")]
    public string? Team { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Item eines Spielers
/// </summary>
public class Item
{
    [JsonPropertyName("canUse")]
    public bool CanUse { get; set; }

    [JsonPropertyName("consumable")]
    public bool Consumable { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("itemID")]
    public int ItemId { get; set; }

    [JsonPropertyName("price")]
    public int Price { get; set; }

    [JsonPropertyName("rawDescription")]
    public string? RawDescription { get; set; }

    [JsonPropertyName("rawDisplayName")]
    public string? RawDisplayName { get; set; }

    [JsonPropertyName("slot")]
    public int Slot { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Runen eines Spielers (vereinfacht)
/// </summary>
public class PlayerRunes
{
    [JsonPropertyName("keystone")]
    public Rune? Keystone { get; set; }

    [JsonPropertyName("primaryRuneTree")]
    public RuneTree? PrimaryRuneTree { get; set; }

    [JsonPropertyName("secondaryRuneTree")]
    public RuneTree? SecondaryRuneTree { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Scores/Statistiken eines Spielers
/// </summary>
public class Scores
{
    [JsonPropertyName("assists")]
    public int Assists { get; set; }

    [JsonPropertyName("creepScore")]
    public int CreepScore { get; set; }

    [JsonPropertyName("deaths")]
    public int Deaths { get; set; }

    [JsonPropertyName("kills")]
    public int Kills { get; set; }

    [JsonPropertyName("wardScore")]
    public float WardScore { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Beschwörerzauber eines Spielers
/// </summary>
public class SummonerSpells
{
    [JsonPropertyName("summonerSpellOne")]
    public SummonerSpell? SummonerSpellOne { get; set; }

    [JsonPropertyName("summonerSpellTwo")]
    public SummonerSpell? SummonerSpellTwo { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Einzelner Beschwörerzauber
/// </summary>
public class SummonerSpell
{
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("rawDescription")]
    public string? RawDescription { get; set; }

    [JsonPropertyName("rawDisplayName")]
    public string? RawDisplayName { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Events im Spiel (Kills, Dragons, Barons, Towers, etc.)
/// </summary>
public class Event
{
    [JsonPropertyName("Events")]
    public List<GameEvent>? EventsList { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Einzelnes Game-Event
/// </summary>
public class GameEvent
{
    [JsonPropertyName("EventID")]
    public int EventId { get; set; }

    [JsonPropertyName("EventName")]
    public string? EventName { get; set; }

    [JsonPropertyName("EventTime")]
    public float EventTime { get; set; }

    // Kill-spezifische Properties
    [JsonPropertyName("KillerName")]
    public string? KillerName { get; set; }

    [JsonPropertyName("VictimName")]
    public string? VictimName { get; set; }

    [JsonPropertyName("Assisters")]
    public List<string>? Assisters { get; set; }

    // Dragon/Baron-spezifische Properties
    [JsonPropertyName("DragonType")]
    public string? DragonType { get; set; }

    [JsonPropertyName("Stolen")]
    public string? Stolen { get; set; }

    // Turm-spezifische Properties
    [JsonPropertyName("TurretKilled")]
    public string? TurretKilled { get; set; }

    // Inhibitor-spezifische Properties
    [JsonPropertyName("InhibKilled")]
    public string? InhibKilled { get; set; }

    [JsonPropertyName("InhibRespawned")]
    public string? InhibRespawned { get; set; }

    // Ace-spezifische Properties
    [JsonPropertyName("Acer")]
    public string? Acer { get; set; }

    [JsonPropertyName("AcingTeam")]
    public string? AcingTeam { get; set; }

    // Multikill-spezifische Properties
    [JsonPropertyName("KillStreak")]
    public int? KillStreak { get; set; }

    // Alle unbekannten Properties
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

/// <summary>
/// Allgemeine Spieldaten
/// </summary>
public class GameData
{
    [JsonPropertyName("gameMode")]
    public string? GameMode { get; set; }

    [JsonPropertyName("gameTime")]
    public float GameTime { get; set; }

    [JsonPropertyName("mapName")]
    public string? MapName { get; set; }

    [JsonPropertyName("mapNumber")]
    public int MapNumber { get; set; }

    [JsonPropertyName("mapTerrain")]
    public string? MapTerrain { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

