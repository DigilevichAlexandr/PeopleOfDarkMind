namespace PeopleOfDarkMind.Domain.Entities;

public class CompletedEvent
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public Guid EventId { get; set; }
    public int Day { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    public GameCharacter Character { get; set; } = null!;
    public GameEvent Event { get; set; } = null!;
}
