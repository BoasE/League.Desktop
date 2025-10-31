namespace BE.League.Desktop.Models;

public sealed class ChampSelectSession
{
    public TimerObj? Timer { get; set; }
    public int LocalPlayerCellId { get; set; }
    public List<List<ActionObj>> Actions { get; set; } = new();
    public List<TeamMember> MyTeam { get; set; } = new();
    public int? MySelectionChampionId { get; set; }
    public int? MyLockedChampionId { get; set; }
    public int? MyTeamIntentChampionId { get; set; }

    public sealed class TimerObj 
    { 
        public string? Phase { get; set; } 
    }
    
    public sealed class ActionObj 
    { 
        public int ActorCellId { get; set; }
        public bool IsInProgress { get; set; }
        public string? Type { get; set; }
        public int ChampionId { get; set; }
    }
    
    public sealed class TeamMember 
    {
        public int CellId { get; set; }
        public int ChampionId { get; set; }
        public int? ChampionPickIntent { get; set; }
        public int? Spell1Id { get; set; }
        public int? Spell2Id { get; set; }
    }
}