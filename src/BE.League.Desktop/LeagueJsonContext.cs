using System.Text.Json.Serialization;
using BE.League.Desktop.Models;

namespace BE.League.Desktop;

/// <summary>
/// JSON Source Generator Context for AOT/Trimming support
/// </summary>
[JsonSerializable(typeof(AllGameData))]
[JsonSerializable(typeof(ActivePlayer))]
[JsonSerializable(typeof(Abilities))]
[JsonSerializable(typeof(FullRunes))]
[JsonSerializable(typeof(List<Player>))]
[JsonSerializable(typeof(Scores))]
[JsonSerializable(typeof(SummonerSpells))]
[JsonSerializable(typeof(PlayerRunes))]
[JsonSerializable(typeof(List<Item>))]
[JsonSerializable(typeof(Event))]
[JsonSerializable(typeof(GameData))]
[JsonSerializable(typeof(Lobby))]
[JsonSerializable(typeof(ChampSelectSession))]
[JsonSerializable(typeof(ReadyCheck))]
[JsonSerializable(typeof(LobbyMember))]
[JsonSerializable(typeof(GameConfig))]
[JsonSerializable(typeof(LobbyInvitation))]
[JsonSerializable(typeof(MucJwt))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
public partial class LeagueJsonContext : JsonSerializerContext
{
}

