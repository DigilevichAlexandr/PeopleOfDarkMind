using PeopleOfDarkMind.Domain.Entities;

namespace PeopleOfDarkMind.Application.Interfaces;

public interface IGameCharacterRepository
{
    Task<GameCharacter?> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<GameCharacter?> GetByUserIdForGameplayAsync(string userId, CancellationToken ct = default);
    Task<GameCharacter?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<bool> HasCompletedEventAsync(Guid characterId, Guid eventId, CancellationToken ct = default);
    Task<HashSet<Guid>> GetCompletedEventIdsAsync(Guid characterId, CancellationToken ct = default);
    Task AddCompletedEventAsync(CompletedEvent completed, CancellationToken ct = default);
    Task AddJournalEntryAsync(GameJournalEntry entry, CancellationToken ct = default);
    Task<bool> HasEvidenceAsync(Guid characterId, Guid evidenceId, CancellationToken ct = default);
    Task AddCharacterEvidenceAsync(CharacterEvidence evidence, CancellationToken ct = default);
    Task<GameCharacter> CreateAsync(GameCharacter character, CancellationToken ct = default);
    Task UpdateAsync(GameCharacter character, CancellationToken ct = default);
}

public interface ILocationRepository
{
    Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken ct = default);
    Task<Location?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<Location?> GetByIdAsync(Guid id, CancellationToken ct = default);
}

public interface INpcRepository
{
    Task<IReadOnlyList<Npc>> GetAllAsync(CancellationToken ct = default);
}

public interface IGameEventRepository
{
    Task<GameEvent?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<GameEvent?> GetByIdWithChoicesAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<GameEvent>> GetRandomPoolAsync(CancellationToken ct = default);
    Task<IReadOnlyList<GameEvent>> GetStoryEventsForChapterAsync(int chapter, CancellationToken ct = default);
}

public interface IEvidenceRepository
{
    Task<IReadOnlyList<Evidence>> GetAllAsync(CancellationToken ct = default);
    Task<Evidence?> GetByCodeAsync(string code, CancellationToken ct = default);
}

public interface IGameSaveRepository
{
    Task<IReadOnlyList<GameSave>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<GameSave?> GetBySlotAsync(string userId, int slot, CancellationToken ct = default);
    Task<GameSave> UpsertAsync(GameSave save, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
