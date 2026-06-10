using PeopleOfDarkMind.Domain.Enums;

namespace PeopleOfDarkMind.Domain.Entities;

public class GameCharacter
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = "Алексей";
    public int Age { get; set; } = 35;
    public int CurrentDay { get; set; } = 1;
    public TimePeriod CurrentPeriod { get; set; } = TimePeriod.Morning;
    public Guid? CurrentLocationId { get; set; }
    public int StoryChapter { get; set; } = 1;
    public int StoryProgress { get; set; }
    public bool UnderworldUnlocked { get; set; }
    public int UnderworldAccessLevel { get; set; }
    public EndingType? AchievedEnding { get; set; }
    public bool IsGameOver { get; set; }
    public Guid? PendingEventId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public CharacterStats Stats { get; set; } = new();
    public Location? CurrentLocation { get; set; }
    public ICollection<GameSave> Saves { get; set; } = [];
    public ICollection<CharacterNpcRelation> NpcRelations { get; set; } = [];
    public ICollection<CharacterEvidence> Evidence { get; set; } = [];
    public ICollection<GameJournalEntry> Journal { get; set; } = [];
    public ICollection<InventoryItem> Inventory { get; set; } = [];
    public ICollection<CompletedEvent> CompletedEvents { get; set; } = [];
}
