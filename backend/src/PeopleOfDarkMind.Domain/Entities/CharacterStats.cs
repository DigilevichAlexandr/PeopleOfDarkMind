namespace PeopleOfDarkMind.Domain.Entities;

public class CharacterStats
{
    public Guid CharacterId { get; set; }

    // Open stats
    public int Money { get; set; } = 15000;
    public int Mood { get; set; } = 50;
    public int Energy { get; set; } = 80;
    public int Health { get; set; } = 90;
    public int Reputation { get; set; } = 30;
    public int Stress { get; set; } = 40;

    // Hidden stats
    public int Awareness { get; set; } = 5;
    public int Sensitivity { get; set; } = 10;
    public int Intuition { get; set; } = 15;
    public int UnderworldBond { get; set; }
    public int InfluenceResistance { get; set; } = 20;
    public int Willpower { get; set; } = 50;

    public GameCharacter Character { get; set; } = null!;
}
