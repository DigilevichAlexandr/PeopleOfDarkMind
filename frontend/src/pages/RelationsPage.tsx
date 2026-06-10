import { useEffect, useState } from 'react';
import api from '../api/client';

interface Npc {
  id: string;
  code: string;
  name: string;
  description: string;
  portraitEmoji: string;
  trust: number;
  affection: number;
  fear: number;
  hasMet: boolean;
  faction: string | null;
}

export default function RelationsPage() {
  const [npcs, setNpcs] = useState<Npc[]>([]);

  useEffect(() => {
    api.get<Npc[]>('/game/npcs').then((r) => setNpcs(r.data));
  }, []);

  return (
    <div>
      <h1 className="font-[family-name:var(--font-display)] text-3xl mb-6">Отношения</h1>
      <div className="grid sm:grid-cols-2 gap-4">
        {npcs.map((n) => (
          <div key={n.id} className={`bg-panel border rounded-xl p-4 ${n.hasMet ? 'border-border' : 'border-border/50 opacity-60'}`}>
            <div className="flex gap-3 items-start">
              <span className="text-3xl">{n.portraitEmoji}</span>
              <div className="flex-1 min-w-0">
                <h3 className="font-medium">{n.name}</h3>
                <p className="text-sm text-muted line-clamp-2">{n.description}</p>
                {n.hasMet && (
                  <div className="grid grid-cols-3 gap-2 mt-3 text-xs">
                    <span>Доверие {n.trust}</span>
                    <span>Симпатия {n.affection}</span>
                    <span>Страх {n.fear}</span>
                  </div>
                )}
                {n.faction && <span className="text-xs text-mystic mt-2 block">{n.faction}</span>}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
