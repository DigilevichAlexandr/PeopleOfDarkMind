using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeopleOfDarkMind.Domain.Entities;
using PeopleOfDarkMind.Infrastructure.Identity;

namespace PeopleOfDarkMind.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<GameCharacter> Characters => Set<GameCharacter>();
    public DbSet<CharacterStats> CharacterStats => Set<CharacterStats>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Npc> Npcs => Set<Npc>();
    public DbSet<CharacterNpcRelation> CharacterNpcRelations => Set<CharacterNpcRelation>();
    public DbSet<GameEvent> GameEvents => Set<GameEvent>();
    public DbSet<EventChoice> EventChoices => Set<EventChoice>();
    public DbSet<StoryChapter> StoryChapters => Set<StoryChapter>();
    public DbSet<Evidence> EvidenceItems => Set<Evidence>();
    public DbSet<CharacterEvidence> CharacterEvidence => Set<CharacterEvidence>();
    public DbSet<GameSave> GameSaves => Set<GameSave>();
    public DbSet<GameJournalEntry> JournalEntries => Set<GameJournalEntry>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<CompletedEvent> CompletedEvents => Set<CompletedEvent>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<GameCharacter>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.UserId).IsUnique();
            e.HasOne(x => x.Stats).WithOne(s => s.Character)
                .HasForeignKey<CharacterStats>(s => s.CharacterId);
            e.HasOne(x => x.CurrentLocation).WithMany().HasForeignKey(x => x.CurrentLocationId);
        });

        builder.Entity<CharacterStats>(e => e.HasKey(s => s.CharacterId));

        builder.Entity<Location>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Code).IsUnique();
        });

        builder.Entity<Npc>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Code).IsUnique();
        });

        builder.Entity<GameEvent>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Code).IsUnique();
            e.HasMany(x => x.Choices).WithOne(c => c.Event).HasForeignKey(c => c.EventId);
        });

        builder.Entity<Evidence>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Code).IsUnique();
        });

        builder.Entity<StoryChapter>(e => e.HasKey(x => x.Number));

        builder.Entity<GameSave>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.SlotNumber }).IsUnique();
        });
    }
}
