using PeopleOfDarkMind.Application.DTOs;
using PeopleOfDarkMind.Domain.Entities;
using PeopleOfDarkMind.Domain.Enums;

namespace PeopleOfDarkMind.Application.Services;

public static class GameMapper
{
    public static CharacterStatsDto ToDto(CharacterStats s) => new(
        s.Money, s.Mood, s.Energy, s.Health, s.Reputation, s.Stress,
        s.Awareness, s.Sensitivity, s.Intuition, s.UnderworldBond,
        s.InfluenceResistance, s.Willpower);

    public static CharacterDto ToDto(GameCharacter c) => new(
        c.Id, c.Name, c.Age, c.CurrentDay, PeriodLabel(c.CurrentPeriod),
        c.StoryChapter, c.StoryProgress, c.UnderworldUnlocked,
        c.IsGameOver, c.AchievedEnding?.ToString(), ToDto(c.Stats),
        c.CurrentLocation is null ? null : ToBrief(c.CurrentLocation));

    public static LocationBriefDto ToBrief(Location l) => new(
        l.Id, l.Code, l.Name, l.Description, l.District, l.IsMystic, l.MapX, l.MapY, l.Icon);

    public static LocationDto ToDto(Location l, bool accessible) => new(
        l.Id, l.Code, l.Name, l.Description, l.District, l.IsMystic,
        accessible, l.MapX, l.MapY, l.Icon);

    public static string PeriodLabel(TimePeriod p) => p switch
    {
        TimePeriod.Morning => "Утро",
        TimePeriod.Day => "День",
        TimePeriod.Evening => "Вечер",
        TimePeriod.Night => "Ночь",
        _ => "?"
    };

    public static IReadOnlyList<string> GetAvailableActions(GameCharacter c) =>
    [
        "JobSearch", "Interview", "Freelance", "Stream", "Video",
        "LanguageStudy", "WalkCity", "Socialize", "Read", "Sport",
        "Investigate", "VisitLocation", "Rest", "PayBills"
    ];
}
