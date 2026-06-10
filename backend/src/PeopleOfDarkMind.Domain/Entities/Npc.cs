using PeopleOfDarkMind.Domain.Enums;

namespace PeopleOfDarkMind.Domain.Entities;

public class Npc
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string HiddenMotives { get; set; } = string.Empty;
    public FactionType Faction { get; set; } = FactionType.None;
    public bool IsSupernatural { get; set; }
    public bool HidesTrueNature { get; set; }
    public Guid? DefaultLocationId { get; set; }
    public string PortraitEmoji { get; set; } = "👤";

    public Location? DefaultLocation { get; set; }
    public ICollection<CharacterNpcRelation> CharacterRelations { get; set; } = [];
}
