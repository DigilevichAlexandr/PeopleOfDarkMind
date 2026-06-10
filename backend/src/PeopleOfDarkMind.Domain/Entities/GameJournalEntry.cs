namespace PeopleOfDarkMind.Domain.Entities;

public class GameJournalEntry
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public int Day { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public GameCharacter Character { get; set; } = null!;
}
