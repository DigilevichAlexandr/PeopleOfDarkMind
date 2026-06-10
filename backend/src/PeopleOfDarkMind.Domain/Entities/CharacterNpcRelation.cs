namespace PeopleOfDarkMind.Domain.Entities;

public class CharacterNpcRelation
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public Guid NpcId { get; set; }
    public int Trust { get; set; } = 30;
    public int Affection { get; set; } = 30;
    public int Fear { get; set; }
    public bool HasMet { get; set; }
    public string Notes { get; set; } = string.Empty;

    public GameCharacter Character { get; set; } = null!;
    public Npc Npc { get; set; } = null!;
}
