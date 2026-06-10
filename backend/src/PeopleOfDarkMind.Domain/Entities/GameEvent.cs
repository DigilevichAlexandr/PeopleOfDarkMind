using PeopleOfDarkMind.Domain.Enums;

namespace PeopleOfDarkMind.Domain.Entities;

public class GameEvent
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EventCategory Category { get; set; }
    public int MinDay { get; set; } = 1;
    public int? MaxDay { get; set; }
    public int MinChapter { get; set; } = 1;
    public int? RequiredAwareness { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? NpcId { get; set; }
    public bool IsRandomPool { get; set; }
    public int Weight { get; set; } = 10;
    public bool IsRepeatable { get; set; }
    public int StoryOrder { get; set; }

    public Location? Location { get; set; }
    public Npc? Npc { get; set; }
    public ICollection<EventChoice> Choices { get; set; } = [];
}
