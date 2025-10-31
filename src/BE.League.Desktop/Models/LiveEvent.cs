namespace BE.League.Desktop.Models;

public sealed class LiveEvent
{
    public int EventID { get; set; }
    public string EventName { get; set; } = "";
    public float EventTime { get; set; }
    public string? KillerName { get; set; }
    public string? VictimName { get; set; }
    public List<string>? Assisters { get; set; }
    public string? ItemName { get; set; }
    public string? Player { get; set; }
    public string? KillerTeam { get; set; }
    public string? DragonType { get; set; }
    public string? TurretKilled { get; set; }
    public string? InhibKilled { get; set; }
    public string? Result { get; set; }
}