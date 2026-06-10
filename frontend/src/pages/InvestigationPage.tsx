import { useEffect, useState } from 'react';
import api from '../api/client';

interface Evidence {
  id: string;
  code: string;
  title: string;
  description: string;
  type: string;
  isCollected: boolean;
  isConnected: boolean;
  collectedAt: string | null;
}

export default function InvestigationPage() {
  const [items, setItems] = useState<Evidence[]>([]);

  useEffect(() => {
    api.get<Evidence[]>('/game/evidence').then((r) => setItems(r.data));
  }, []);

  const collected = items.filter((e) => e.isCollected);

  return (
    <div>
      <h1 className="font-[family-name:var(--font-display)] text-3xl mb-2">Доска расследования</h1>
      <p className="text-muted mb-8">Связывайте улики, открывайте новые ветки сюжета</p>
      <div className="grid md:grid-cols-2 gap-4">
        {collected.length === 0 ? (
          <p className="text-muted col-span-2">Улики ещё не собраны. Исследуйте город и делайте выборы.</p>
        ) : (
          collected.map((e) => (
            <article key={e.id} className="bg-panel border border-border rounded-xl p-4">
              <span className="text-xs text-accent">{e.type}</span>
              <h3 className="font-medium mt-1">{e.title}</h3>
              <p className="text-sm text-slate-400 mt-2">{e.description}</p>
              {e.isConnected && <span className="text-xs text-mystic mt-2 block">🔗 Связана</span>}
            </article>
          ))
        )}
      </div>
      <div className="mt-8 p-4 border border-dashed border-border rounded-xl text-muted text-sm">
        Несобранные улики: {items.filter((e) => !e.isCollected).map((e) => e.title).join(' · ') || '—'}
      </div>
    </div>
  );
}
