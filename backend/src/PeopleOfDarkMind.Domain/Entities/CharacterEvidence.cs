namespace PeopleOfDarkMind.Domain.Entities;

public class CharacterEvidence
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public Guid EvidenceId { get; set; }
    public DateTime CollectedAt { get; set; } = DateTime.UtcNow;
    public bool IsConnected { get; set; }
    public Guid? ConnectedToEvidenceId { get; set; }

    public GameCharacter Character { get; set; } = null!;
    public Evidence Evidence { get; set; } = null!;
}
