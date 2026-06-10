namespace PeopleOfDarkMind.Domain.Entities;

public class Location
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public bool IsMystic { get; set; }
    public bool RequiresAwareness { get; set; }
    public int MinAwareness { get; set; }
    public int MapX { get; set; }
    public int MapY { get; set; }
    public string Icon { get; set; } = "📍";

    public ICollection<GameEvent> Events { get; set; } = [];
}
