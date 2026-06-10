using MediatR;
using PeopleOfDarkMind.Application.Common;
using PeopleOfDarkMind.Application.DTOs;
using PeopleOfDarkMind.Application.Interfaces;
using PeopleOfDarkMind.Application.Services;

namespace PeopleOfDarkMind.Application.Features.Game;

public record GetGameStateQuery(string UserId) : IRequest<Result<GameStateDto>>;
public record GetCharacterQuery(string UserId) : IRequest<Result<CharacterDto>>;
public record GetLocationsQuery(string UserId) : IRequest<Result<IReadOnlyList<LocationDto>>>;
public record GetNpcsQuery(string UserId) : IRequest<Result<IReadOnlyList<NpcDto>>>;
public record GetEvidenceQuery(string UserId) : IRequest<Result<IReadOnlyList<EvidenceDto>>>;
public record GetJournalQuery(string UserId) : IRequest<Result<IReadOnlyList<JournalEntryDto>>>;
public record GetSavesQuery(string UserId) : IRequest<Result<IReadOnlyList<SaveSlotDto>>>;

public class GetGameStateHandler(IGameEngineService engine)
    : IRequestHandler<GetGameStateQuery, Result<GameStateDto>>
{
    public Task<Result<GameStateDto>> Handle(GetGameStateQuery request, CancellationToken ct) =>
        engine.GetGameStateAsync(request.UserId, ct);
}

public class GetCharacterHandler(IGameCharacterRepository repo)
    : IRequestHandler<GetCharacterQuery, Result<CharacterDto>>
{
    public async Task<Result<CharacterDto>> Handle(GetCharacterQuery request, CancellationToken ct)
    {
        var c = await repo.GetByUserIdAsync(request.UserId, ct);
        return c is null
            ? Result<CharacterDto>.Fail("Персонаж не найден")
            : Result<CharacterDto>.Ok(GameMapper.ToDto(c));
    }
}

public class GetLocationsHandler(
    ILocationRepository locations,
    IGameCharacterRepository characters)
    : IRequestHandler<GetLocationsQuery, Result<IReadOnlyList<LocationDto>>>
{
    public async Task<Result<IReadOnlyList<LocationDto>>> Handle(GetLocationsQuery request, CancellationToken ct)
    {
        var character = await characters.GetByUserIdAsync(request.UserId, ct);
        var all = await locations.GetAllAsync(ct);
        var awareness = character?.Stats.Awareness ?? 0;
        var list = all.Select(l => GameMapper.ToDto(l,
            !l.RequiresAwareness || awareness >= l.MinAwareness)).ToList();
        return Result<IReadOnlyList<LocationDto>>.Ok(list);
    }
}

public class GetNpcsHandler(
    INpcRepository npcs,
    IGameCharacterRepository characters)
    : IRequestHandler<GetNpcsQuery, Result<IReadOnlyList<NpcDto>>>
{
    public async Task<Result<IReadOnlyList<NpcDto>>> Handle(GetNpcsQuery request, CancellationToken ct)
    {
        var character = await characters.GetByUserIdAsync(request.UserId, ct);
        if (character is null) return Result<IReadOnlyList<NpcDto>>.Fail("Персонаж не найден");

        var all = await npcs.GetAllAsync(ct);
        var dtos = all.Select(n =>
        {
            var rel = character.NpcRelations.FirstOrDefault(r => r.NpcId == n.Id);
            return new NpcDto(n.Id, n.Code, n.Name, n.Description, n.PortraitEmoji,
                rel?.Trust ?? 0, rel?.Affection ?? 0, rel?.Fear ?? 0,
                rel?.HasMet ?? false,
                n.HidesTrueNature ? null : n.Faction.ToString());
        }).ToList();
        return Result<IReadOnlyList<NpcDto>>.Ok(dtos);
    }
}

public class GetEvidenceHandler(
    IEvidenceRepository evidence,
    IGameCharacterRepository characters)
    : IRequestHandler<GetEvidenceQuery, Result<IReadOnlyList<EvidenceDto>>>
{
    public async Task<Result<IReadOnlyList<EvidenceDto>>> Handle(GetEvidenceQuery request, CancellationToken ct)
    {
        var character = await characters.GetByUserIdAsync(request.UserId, ct);
        if (character is null) return Result<IReadOnlyList<EvidenceDto>>.Fail("Персонаж не найден");

        var all = await evidence.GetAllAsync(ct);
        var dtos = all.Select(e =>
        {
            var collected = character.Evidence.FirstOrDefault(ce => ce.EvidenceId == e.Id);
            return new EvidenceDto(e.Id, e.Code, e.Title, e.Description, e.Type.ToString(),
                collected is not null, collected?.IsConnected ?? false, collected?.CollectedAt);
        }).ToList();
        return Result<IReadOnlyList<EvidenceDto>>.Ok(dtos);
    }
}

public class GetJournalHandler(IGameCharacterRepository characters)
    : IRequestHandler<GetJournalQuery, Result<IReadOnlyList<JournalEntryDto>>>
{
    public async Task<Result<IReadOnlyList<JournalEntryDto>>> Handle(GetJournalQuery request, CancellationToken ct)
    {
        var character = await characters.GetByUserIdAsync(request.UserId, ct);
        if (character is null) return Result<IReadOnlyList<JournalEntryDto>>.Fail("Персонаж не найден");

        var entries = character.Journal
            .OrderByDescending(j => j.Day)
            .ThenByDescending(j => j.CreatedAt)
            .Select(j => new JournalEntryDto(j.Id, j.Day, j.Title, j.Content, j.CreatedAt))
            .ToList();
        return Result<IReadOnlyList<JournalEntryDto>>.Ok(entries);
    }
}

public class GetSavesHandler(IGameSaveRepository saves)
    : IRequestHandler<GetSavesQuery, Result<IReadOnlyList<SaveSlotDto>>>
{
    public async Task<Result<IReadOnlyList<SaveSlotDto>>> Handle(GetSavesQuery request, CancellationToken ct)
    {
        var list = await saves.GetByUserIdAsync(request.UserId, ct);
        var dtos = list.Select(s => new SaveSlotDto(s.Id, s.SlotNumber, s.SaveName,
            s.IsAutoSave, 0, s.SavedAt)).ToList();
        return Result<IReadOnlyList<SaveSlotDto>>.Ok(dtos);
    }
}
