namespace PeopleOfDarkMind.Domain.Entities;

public class EventChoice
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string OutcomeText { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public int? MinAwareness { get; set; }
    public int? MinMoney { get; set; }
    public int? MinWillpower { get; set; }
    public string EffectsJson { get; set; } = "{}";
    public string? UnlocksEventCode { get; set; }
    public string? UnlocksEvidenceCode { get; set; }

    public GameEvent Event { get; set; } = null!;
}
