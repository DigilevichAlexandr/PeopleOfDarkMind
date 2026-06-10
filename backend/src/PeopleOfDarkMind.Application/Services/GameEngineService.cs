using PeopleOfDarkMind.Application.Common;
using PeopleOfDarkMind.Application.DTOs;
using PeopleOfDarkMind.Application.Interfaces;
using PeopleOfDarkMind.Domain.Entities;
using PeopleOfDarkMind.Domain.Enums;

namespace PeopleOfDarkMind.Application.Services;

public class GameEngineService(
    IGameCharacterRepository characters,
    ILocationRepository locations,
    IGameEventRepository events,
    IEvidenceRepository evidenceRepo,
    IUnitOfWork unitOfWork,
    INpcRepository npcRepo) : IGameEngineService
{
    public async Task<Result<CharacterDto>> CreateNewGameAsync(
        string userId, string? characterName, CancellationToken ct = default)
    {
        var existing = await characters.GetByUserIdAsync(userId, ct);
        if (existing is not null)
            return Result<CharacterDto>.Fail("Игра уже существует. Удалите сохранение или продолжите.");

        var home = await locations.GetByCodeAsync("home", ct);
        var character = new GameCharacter
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = characterName ?? "Алексей",
            CurrentLocationId = home?.Id,
            Stats = new CharacterStats { CharacterId = Guid.Empty }
        };
        character.Stats.CharacterId = character.Id;

        character.Journal.Add(new GameJournalEntry
        {
            Id = Guid.NewGuid(),
            CharacterId = character.Id,
            Day = 1,
            Title = "Глава 1: Падение",
            Content = "Три месяца без работы. Мать в соседней комнате. Квартира от деда ждёт арендаторов. " +
                      "Город шумит за окном, но ты слышишь только тишину собственных мыслей."
        });

        var allNpcsList = await npcRepo.GetAllAsync(ct);
        foreach (var npc in allNpcsList)
        {
            character.NpcRelations.Add(new CharacterNpcRelation
            {
                Id = Guid.NewGuid(),
                CharacterId = character.Id,
                NpcId = npc.Id,
                HasMet = npc.Code == "mother"
            });
        }

        await characters.CreateAsync(character, ct);
        await unitOfWork.SaveChangesAsync(ct);
        var loaded = await characters.GetByIdWithDetailsAsync(character.Id, ct);
        return Result<CharacterDto>.Ok(GameMapper.ToDto(loaded!));
    }

    public async Task<Result<GameStateDto>> GetGameStateAsync(string userId, CancellationToken ct = default)
    {
        var c = await characters.GetByUserIdAsync(userId, ct);
        if (c is null) return Result<GameStateDto>.Fail("Персонаж не найден");
        var pending = await GetPendingEventDtoAsync(c, ct);
        return Result<GameStateDto>.Ok(BuildState(c, pending));
    }

    public async Task<Result<GameStateDto>> PerformActionAsync(
        string userId, PerformActionRequest request, CancellationToken ct = default)
    {
        var c = await characters.GetByUserIdForGameplayAsync(userId, ct);
        if (c is null) return Result<GameStateDto>.Fail("Персонаж не найден");
        if (c.IsGameOver) return Result<GameStateDto>.Fail("Игра окончена");
        if (c.PendingEventId.HasValue)
            return Result<GameStateDto>.Fail("Сначала завершите текущее событие");

        ApplyActionEffects(c, request.ActionType, request.LocationId);
        var dayBefore = c.CurrentDay;
        AdvanceTime(c);
        var dayAdvanced = c.CurrentDay > dayBefore;

        if (request.ActionType is "Investigate" or "WalkCity")
            await TryRandomEventAsync(c, mysticBias: true, ct);
        else if (dayAdvanced && c.CurrentPeriod == TimePeriod.Morning)
            await TryRandomEventAsync(c, mysticBias: false, ct);
        else if (Random.Shared.Next(100) < 25)
            await TryRandomEventAsync(c, mysticBias: false, ct);

        c.UpdatedAt = DateTime.UtcNow;
        await characters.UpdateAsync(c, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var reloaded = await characters.GetByUserIdAsync(userId, ct);
        var pending = await GetPendingEventDtoAsync(reloaded!, ct);
        return Result<GameStateDto>.Ok(BuildState(reloaded!, pending));
    }

    public async Task<Result<GameStateDto>> MakeChoiceAsync(
        string userId, MakeChoiceRequest request, CancellationToken ct = default)
    {
        var c = await characters.GetByUserIdForGameplayAsync(userId, ct);
        if (c is null) return Result<GameStateDto>.Fail("Персонаж не найден");

        if (!c.PendingEventId.HasValue)
            return Result<GameStateDto>.Fail("Нет активного события");
        if (c.PendingEventId != request.EventId)
            return Result<GameStateDto>.Fail("Активно другое событие");

        var ev = await events.GetByIdWithChoicesAsync(request.EventId, ct);
        if (ev is null) return Result<GameStateDto>.Fail("Событие не найдено");

        var choice = ev.Choices.FirstOrDefault(ch => ch.Id == request.ChoiceId);
        if (choice is null) return Result<GameStateDto>.Fail("Выбор не найден");

        if (choice.MinAwareness.HasValue && c.Stats.Awareness < choice.MinAwareness)
            return Result<GameStateDto>.Fail("Недостаточно осознанности");
        if (choice.MinMoney.HasValue && c.Stats.Money < choice.MinMoney)
            return Result<GameStateDto>.Fail("Недостаточно денег");
        if (choice.MinWillpower.HasValue && c.Stats.Willpower < choice.MinWillpower)
            return Result<GameStateDto>.Fail("Недостаточно воли");

        StatEffects.Apply(c.Stats, choice.EffectsJson);
        c.StoryProgress++;
        c.PendingEventId = null;
        c.UpdatedAt = DateTime.UtcNow;

        if (!await characters.HasCompletedEventAsync(c.Id, ev.Id, ct))
        {
            await characters.AddCompletedEventAsync(new CompletedEvent
            {
                Id = Guid.NewGuid(),
                CharacterId = c.Id,
                EventId = ev.Id,
                Day = c.CurrentDay
            }, ct);
        }

        await characters.AddJournalEntryAsync(new GameJournalEntry
        {
            Id = Guid.NewGuid(),
            CharacterId = c.Id,
            Day = c.CurrentDay,
            Title = ev.Title,
            Content = choice.OutcomeText
        }, ct);

        if (!string.IsNullOrEmpty(choice.UnlocksEvidenceCode))
            await CollectEvidenceForCharacter(c, choice.UnlocksEvidenceCode, ct);

        await CheckChapterProgressAsync(c, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var reloaded = await characters.GetByUserIdAsync(userId, ct);
        return Result<GameStateDto>.Ok(BuildState(reloaded!, null));
    }

    public async Task<Result<GameStateDto>> TravelToLocationAsync(
        string userId, Guid locationId, CancellationToken ct = default)
    {
        var c = await characters.GetByUserIdForGameplayAsync(userId, ct);
        if (c is null) return Result<GameStateDto>.Fail("Персонаж не найден");
        if (c.PendingEventId.HasValue)
            return Result<GameStateDto>.Fail("Сначала завершите текущее событие");

        var completedIds = await characters.GetCompletedEventIdsAsync(c.Id, ct);
        var loc = await locations.GetByIdAsync(locationId, ct);
        if (loc is null) return Result<GameStateDto>.Fail("Локация не найдена");
        if (loc.RequiresAwareness && c.Stats.Awareness < loc.MinAwareness)
            return Result<GameStateDto>.Fail("Вы ещё не видите эту сторону города.");

        c.CurrentLocationId = loc.Id;
        c.Stats.Energy = Math.Max(0, c.Stats.Energy - 5);
        c.UpdatedAt = DateTime.UtcNow;
        await characters.UpdateAsync(c, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var locationEvent = loc.Events
            .Where(e => e.MinDay <= c.CurrentDay && e.MinChapter <= c.StoryChapter)
            .Where(e => !completedIds.Contains(e.Id))
            .OrderBy(e => e.StoryOrder)
            .FirstOrDefault();

        if (locationEvent is not null)
        {
            c.PendingEventId = locationEvent.Id;
            await characters.UpdateAsync(c, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        var reloaded = await characters.GetByUserIdAsync(userId, ct);
        var pending = await GetPendingEventDtoAsync(reloaded!, ct);
        return Result<GameStateDto>.Ok(BuildState(reloaded!, pending));
    }

    public async Task<Result<GameStateDto>> TriggerRandomEventAsync(string userId, CancellationToken ct = default)
    {
        var c = await characters.GetByUserIdForGameplayAsync(userId, ct);
        if (c is null) return Result<GameStateDto>.Fail("Персонаж не найден");
        if (c.PendingEventId.HasValue)
            return Result<GameStateDto>.Fail("Сначала завершите текущее событие");

        var triggered = await TryRandomEventAsync(c, mysticBias: false, ct);
        if (!triggered)
            return Result<GameStateDto>.Fail("Нет доступных случайных событий");

        c.UpdatedAt = DateTime.UtcNow;
        await characters.UpdateAsync(c, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var reloaded = await characters.GetByUserIdAsync(userId, ct);
        var pending = await GetPendingEventDtoAsync(reloaded!, ct);
        return Result<GameStateDto>.Ok(BuildState(reloaded!, pending));
    }

    private async Task<bool> TryRandomEventAsync(GameCharacter c, bool mysticBias, CancellationToken ct)
    {
        var pool = await events.GetRandomPoolAsync(ct);
        var completedIds = await characters.GetCompletedEventIdsAsync(c.Id, ct);
        var available = pool
            .Where(e => e.MinDay <= c.CurrentDay && !completedIds.Contains(e.Id))
            .Where(e => !e.RequiredAwareness.HasValue || c.Stats.Awareness >= e.RequiredAwareness)
            .ToList();

        if (available.Count == 0) return false;

        if (mysticBias)
        {
            var mystic = available.Where(e =>
                e.Category is EventCategory.Mystic or EventCategory.Underworld).ToList();
            if (mystic.Count > 0) available = mystic;
        }

        var totalWeight = available.Sum(e => e.Weight);
        var roll = Random.Shared.Next(totalWeight);
        var acc = 0;
        GameEvent? selected = null;
        foreach (var e in available)
        {
            acc += e.Weight;
            if (roll < acc) { selected = e; break; }
        }

        selected ??= available[Random.Shared.Next(available.Count)];
        c.PendingEventId = selected.Id;
        return true;
    }

    private async Task<GameEventDto?> GetPendingEventDtoAsync(GameCharacter c, CancellationToken ct)
    {
        if (!c.PendingEventId.HasValue) return null;
        var ev = await events.GetByIdWithChoicesAsync(c.PendingEventId.Value, ct);
        return ev is null ? null : MapEvent(ev, c);
    }

    private static void ApplyActionEffects(GameCharacter c, string actionType, Guid? locationId)
    {
        var s = c.Stats;
        switch (actionType)
        {
            case "JobSearch":
                s.Energy -= 15; s.Stress += 10; s.Mood -= 5; break;
            case "Interview":
                s.Energy -= 20; s.Stress += 15; s.Reputation += 5; break;
            case "Freelance":
                s.Money += 3000 + Random.Shared.Next(2000); s.Energy -= 25; break;
            case "Stream":
                s.Money += 500 + Random.Shared.Next(1500); s.Energy -= 20; s.Mood += 5; break;
            case "Video":
                s.Money += 200 + Random.Shared.Next(800); s.Energy -= 15; break;
            case "LanguageStudy":
                s.Energy -= 10; s.Mood += 3; s.Reputation += 2; break;
            case "WalkCity":
                s.Energy -= 10; s.Mood += 8; s.Stress -= 5; s.Awareness += 1; break;
            case "Socialize":
                s.Energy -= 15; s.Mood += 10; s.Stress -= 8; break;
            case "Read":
                s.Energy -= 5; s.Mood += 5; s.Awareness += 2; break;
            case "Sport":
                s.Health += 5; s.Energy -= 20; s.Stress -= 10; s.Mood += 5; break;
            case "Investigate":
                s.Energy -= 25; s.Stress += 10; s.Awareness += 3; s.Intuition += 2; break;
            case "Rest":
                s.Energy = Math.Min(100, s.Energy + 30); s.Health += 3; break;
            case "PayBills":
                s.Money -= 8000; s.Stress -= 5; break;
            case "VisitLocation":
                s.Energy -= 5; break;
        }
        ClampStats(s);
    }

    private static void AdvanceTime(GameCharacter c)
    {
        c.CurrentPeriod = c.CurrentPeriod switch
        {
            TimePeriod.Morning => TimePeriod.Day,
            TimePeriod.Day => TimePeriod.Evening,
            TimePeriod.Evening => TimePeriod.Night,
            _ => TimePeriod.Morning
        };
        if (c.CurrentPeriod == TimePeriod.Morning)
            c.CurrentDay++;
    }

    private async Task CollectEvidenceForCharacter(GameCharacter c, string code, CancellationToken ct)
    {
        var ev = await evidenceRepo.GetByCodeAsync(code, ct);
        if (ev is null || await characters.HasEvidenceAsync(c.Id, ev.Id, ct)) return;
        await characters.AddCharacterEvidenceAsync(new CharacterEvidence
        {
            Id = Guid.NewGuid(),
            CharacterId = c.Id,
            EvidenceId = ev.Id
        }, ct);
    }

    private async Task CheckChapterProgressAsync(GameCharacter c, CancellationToken ct)
    {
        if (c.StoryChapter == 1 && c.StoryProgress >= 5)
        {
            c.StoryChapter = 2;
            await characters.AddJournalEntryAsync(new GameJournalEntry
            {
                Id = Guid.NewGuid(),
                CharacterId = c.Id,
                Day = c.CurrentDay,
                Title = "Пробуждение",
                Content = "Что-то изменилось. Тени движутся иначе. Люди смотрят сквозь тебя."
            }, ct);
        }
        if (c.Stats.Awareness >= 25 && !c.UnderworldUnlocked)
        {
            c.UnderworldUnlocked = true;
            c.UnderworldAccessLevel = 1;
        }
    }

    private static GameStateDto BuildState(GameCharacter c, GameEventDto? currentEvent) =>
        new(GameMapper.ToDto(c), currentEvent, GameMapper.GetAvailableActions(c),
            GameMapper.PeriodLabel(c.CurrentPeriod), $"День {c.CurrentDay}");

    private static GameEventDto MapEvent(GameEvent e, GameCharacter c) => new(
        e.Id, e.Code, e.Title, e.Description, e.Category.ToString(),
        e.Choices.OrderBy(ch => ch.SortOrder).Select(ch =>
        {
            var available = true;
            string? reason = null;
            if (ch.MinAwareness.HasValue && c.Stats.Awareness < ch.MinAwareness)
            { available = false; reason = "Недостаточно осознанности"; }
            if (ch.MinMoney.HasValue && c.Stats.Money < ch.MinMoney)
            { available = false; reason = "Недостаточно денег"; }
            if (ch.MinWillpower.HasValue && c.Stats.Willpower < ch.MinWillpower)
            { available = false; reason = "Недостаточно воли"; }
            return new EventChoiceDto(ch.Id, ch.Text, available, reason);
        }).ToList());

    private static void ClampStats(CharacterStats s)
    {
        s.Mood = Math.Clamp(s.Mood, 0, 100);
        s.Energy = Math.Clamp(s.Energy, 0, 100);
        s.Health = Math.Clamp(s.Health, 0, 100);
        s.Reputation = Math.Clamp(s.Reputation, 0, 100);
        s.Stress = Math.Clamp(s.Stress, 0, 100);
        s.Awareness = Math.Clamp(s.Awareness, 0, 100);
    }
}
