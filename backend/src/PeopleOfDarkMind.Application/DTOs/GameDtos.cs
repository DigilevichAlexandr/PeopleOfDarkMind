using PeopleOfDarkMind.Domain.Enums;

namespace PeopleOfDarkMind.Application.DTOs;

public record CharacterStatsDto(
    int Money, int Mood, int Energy, int Health, int Reputation, int Stress,
    int Awareness, int Sensitivity, int Intuition, int UnderworldBond,
    int InfluenceResistance, int Willpower);

public record CharacterDto(
    Guid Id, string Name, int Age, int CurrentDay, string CurrentPeriod,
    int StoryChapter, int StoryProgress, bool UnderworldUnlocked,
    bool IsGameOver, string? Ending, CharacterStatsDto Stats,
    LocationBriefDto? CurrentLocation);

public record LocationBriefDto(Guid Id, string Code, string Name, string Description,
    string District, bool IsMystic, int MapX, int MapY, string Icon);

public record LocationDto(Guid Id, string Code, string Name, string Description,
    string District, bool IsMystic, bool IsAccessible, int MapX, int MapY, string Icon);

public record NpcDto(Guid Id, string Code, string Name, string Description,
    string PortraitEmoji, int Trust, int Affection, int Fear, bool HasMet, string? Faction);

public record EventChoiceDto(Guid Id, string Text, bool IsAvailable, string? UnavailableReason);
public record GameEventDto(Guid Id, string Code, string Title, string Description,
    string Category, IReadOnlyList<EventChoiceDto> Choices);

public record GameStateDto(CharacterDto Character, GameEventDto? CurrentEvent,
    IReadOnlyList<string> AvailableActions, string PeriodLabel, string DayLabel);

public record SaveSlotDto(Guid Id, int SlotNumber, string SaveName, bool IsAutoSave,
    int Day, DateTime SavedAt);

public record EvidenceDto(Guid Id, string Code, string Title, string Description,
    string Type, bool IsCollected, bool IsConnected, DateTime? CollectedAt);

public record JournalEntryDto(Guid Id, int Day, string Title, string Content, DateTime CreatedAt);

public record PerformActionRequest(string ActionType, Guid? LocationId);
public record MakeChoiceRequest(Guid EventId, Guid ChoiceId);
public record CreateSaveRequest(int SlotNumber, string? SaveName);
public record ConnectEvidenceRequest(Guid EvidenceId, Guid ConnectToId);
