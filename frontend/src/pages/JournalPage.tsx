import { useGame } from '../context/GameContext';

export default function JournalPage() {
  const { getJournal } = useGame();
  const entries = getJournal();

  return (
    <div>
      <h1 className="font-[family-name:var(--font-display)] text-3xl mb-6">Журнал событий</h1>
      <div className="space-y-4">
        {entries.map((e) => (
          <article key={e.id} className="bg-panel border-l-2 border-accent pl-4 py-3">
            <div className="text-xs text-muted">День {e.day}</div>
            <h3 className="font-medium">{e.title}</h3>
            <p className="text-slate-400 mt-1 text-sm leading-relaxed">{e.content}</p>
          </article>
        ))}
        {entries.length === 0 && <p className="text-muted">Записей пока нет</p>}
      </div>
    </div>
  );
}
