namespace PeopleOfDarkMind.Domain.Entities;

public class GameSave
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int SlotNumber { get; set; }
    public string SaveName { get; set; } = string.Empty;
    public bool IsAutoSave { get; set; }
    public string SnapshotJson { get; set; } = "{}";
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;

    public GameCharacter Character { get; set; } = null!;
}
