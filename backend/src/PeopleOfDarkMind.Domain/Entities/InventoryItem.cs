namespace PeopleOfDarkMind.Domain.Entities;

public class InventoryItem
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;

    public GameCharacter Character { get; set; } = null!;
}
