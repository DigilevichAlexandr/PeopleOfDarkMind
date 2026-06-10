using Microsoft.EntityFrameworkCore;
using PeopleOfDarkMind.Application.Interfaces;
using PeopleOfDarkMind.Domain.Entities;
using PeopleOfDarkMind.Infrastructure.Persistence;

namespace PeopleOfDarkMind.Infrastructure.Repositories;

public class GameCharacterRepository(AppDbContext db) : IGameCharacterRepository
{
    public Task<GameCharacter?> GetByUserIdAsync(string userId, CancellationToken ct = default) =>
        db.Characters
            .Include(c => c.Stats)
            .Include(c => c.CurrentLocation)
            .Include(c => c.NpcRelations)
            .Include(c => c.Evidence)
            .Include(c => c.Journal)
            .Include(c => c.Inventory)
            .Include(c => c.CompletedEvents)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

    public Task<GameCharacter?> GetByUserIdForGameplayAsync(string userId, CancellationToken ct = default) =>
        db.Characters
            .Include(c => c.Stats)
            .Include(c => c.CurrentLocation)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

    public Task<bool> HasCompletedEventAsync(Guid characterId, Guid eventId, CancellationToken ct = default) =>
        db.CompletedEvents.AnyAsync(e => e.CharacterId == characterId && e.EventId == eventId, ct);

    public async Task<HashSet<Guid>> GetCompletedEventIdsAsync(Guid characterId, CancellationToken ct = default)
    {
        var ids = await db.CompletedEvents
            .Where(e => e.CharacterId == characterId)
            .Select(e => e.EventId)
            .ToListAsync(ct);
        return ids.ToHashSet();
    }

    public Task AddCompletedEventAsync(CompletedEvent completed, CancellationToken ct = default)
    {
        db.CompletedEvents.Add(completed);
        return Task.CompletedTask;
    }

    public Task AddJournalEntryAsync(GameJournalEntry entry, CancellationToken ct = default)
    {
        db.JournalEntries.Add(entry);
        return Task.CompletedTask;
    }

    public Task<bool> HasEvidenceAsync(Guid characterId, Guid evidenceId, CancellationToken ct = default) =>
        db.CharacterEvidence.AnyAsync(e => e.CharacterId == characterId && e.EvidenceId == evidenceId, ct);

    public Task AddCharacterEvidenceAsync(CharacterEvidence evidence, CancellationToken ct = default)
    {
        db.CharacterEvidence.Add(evidence);
        return Task.CompletedTask;
    }

    public Task<GameCharacter?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default) =>
        db.Characters
            .Include(c => c.Stats)
            .Include(c => c.CurrentLocation)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<GameCharacter> CreateAsync(GameCharacter character, CancellationToken ct = default)
    {
        db.Characters.Add(character);
        await db.SaveChangesAsync(ct);
        return character;
    }

    public Task UpdateAsync(GameCharacter character, CancellationToken ct = default)
    {
        // Персонаж уже отслеживается контекстом после GetByUserIdAsync — Update() ломает связи и даёт 500.
        return Task.CompletedTask;
    }
}

public class LocationRepository(AppDbContext db) : ILocationRepository
{
    public Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken ct = default) =>
        db.Locations.OrderBy(l => l.Name).ToListAsync(ct).ContinueWith(t => (IReadOnlyList<Location>)t.Result, ct);

    public Task<Location?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        db.Locations.Include(l => l.Events).ThenInclude(e => e.Choices)
            .FirstOrDefaultAsync(l => l.Code == code, ct);

    public Task<Location?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Locations.Include(l => l.Events).ThenInclude(e => e.Choices)
            .FirstOrDefaultAsync(l => l.Id == id, ct);
}

public class NpcRepository(AppDbContext db) : INpcRepository
{
    public Task<IReadOnlyList<Npc>> GetAllAsync(CancellationToken ct = default) =>
        db.Npcs.OrderBy(n => n.Name).ToListAsync(ct).ContinueWith(t => (IReadOnlyList<Npc>)t.Result, ct);
}

public class GameEventRepository(AppDbContext db) : IGameEventRepository
{
    public Task<GameEvent?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        db.GameEvents.Include(e => e.Choices).FirstOrDefaultAsync(e => e.Code == code, ct);

    public Task<GameEvent?> GetByIdWithChoicesAsync(Guid id, CancellationToken ct = default) =>
        db.GameEvents.AsNoTracking().Include(e => e.Choices).FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<IReadOnlyList<GameEvent>> GetRandomPoolAsync(CancellationToken ct = default) =>
        db.GameEvents.Include(e => e.Choices).Where(e => e.IsRandomPool).ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<GameEvent>)t.Result, ct);

    public Task<IReadOnlyList<GameEvent>> GetStoryEventsForChapterAsync(int chapter, CancellationToken ct = default) =>
        db.GameEvents.Include(e => e.Choices)
            .Where(e => e.MinChapter == chapter && !e.IsRandomPool)
            .OrderBy(e => e.StoryOrder).ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<GameEvent>)t.Result, ct);
}

public class EvidenceRepository(AppDbContext db) : IEvidenceRepository
{
    public Task<IReadOnlyList<Evidence>> GetAllAsync(CancellationToken ct = default) =>
        db.EvidenceItems.ToListAsync(ct).ContinueWith(t => (IReadOnlyList<Evidence>)t.Result, ct);

    public Task<Evidence?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        db.EvidenceItems.FirstOrDefaultAsync(e => e.Code == code, ct);
}

public class GameSaveRepository(AppDbContext db) : IGameSaveRepository
{
    public Task<IReadOnlyList<GameSave>> GetByUserIdAsync(string userId, CancellationToken ct = default) =>
        db.GameSaves.Where(s => s.UserId == userId).OrderBy(s => s.SlotNumber).ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<GameSave>)t.Result, ct);

    public Task<GameSave?> GetBySlotAsync(string userId, int slot, CancellationToken ct = default) =>
        db.GameSaves.FirstOrDefaultAsync(s => s.UserId == userId && s.SlotNumber == slot, ct);

    public async Task<GameSave> UpsertAsync(GameSave save, CancellationToken ct = default)
    {
        var existing = await GetBySlotAsync(save.UserId, save.SlotNumber, ct);
        if (existing is not null)
        {
            existing.SnapshotJson = save.SnapshotJson;
            existing.SaveName = save.SaveName;
            existing.SavedAt = DateTime.UtcNow;
            existing.IsAutoSave = save.IsAutoSave;
            await db.SaveChangesAsync(ct);
            return existing;
        }
        db.GameSaves.Add(save);
        await db.SaveChangesAsync(ct);
        return save;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var save = await db.GameSaves.FindAsync([id], ct);
        if (save is not null) { db.GameSaves.Remove(save); await db.SaveChangesAsync(ct); }
    }
}

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
