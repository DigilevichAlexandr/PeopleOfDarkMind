using PeopleOfDarkMind.Domain.Enums;

namespace PeopleOfDarkMind.Domain.Entities;

public class Evidence
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EvidenceType Type { get; set; }
    public int Chapter { get; set; } = 1;
    public string? RelatedEventCode { get; set; }
    public string? ConnectsToCode { get; set; }
}
