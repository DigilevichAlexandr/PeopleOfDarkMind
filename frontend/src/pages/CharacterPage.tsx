import { useEffect, useState } from 'react';
import api, { type Character } from '../api/client';
import StatBar from '../components/StatBar';

export default function CharacterPage() {
  const [c, setC] = useState<Character | null>(null);

  useEffect(() => {
    api.get<Character>('/game/character').then((r) => setC(r.data)).catch(() => setC(null));
  }, []);

  if (!c) return <p className="text-muted">Персонаж не найден</p>;

  const s = c.stats;
  return (
    <div className="max-w-2xl">
      <h1 className="font-[family-name:var(--font-display)] text-3xl mb-6">{c.name}</h1>
      <div className="grid gap-6">
        <section className="bg-panel border border-border rounded-xl p-5 space-y-3">
          <h2 className="text-muted text-sm uppercase">Открытые</h2>
          <StatBar label="Настроение" value={s.mood} />
          <StatBar label="Энергия" value={s.energy} color="bg-emerald-500" />
          <StatBar label="Здоровье" value={s.health} color="bg-rose-500" />
          <StatBar label="Репутация" value={s.reputation} />
          <StatBar label="Стресс" value={s.stress} color="bg-orange-500" />
          <p>Деньги: {s.money.toLocaleString('ru-RU')} ₽</p>
        </section>
        <section className="bg-panel border border-mystic/30 rounded-xl p-5 space-y-3">
          <h2 className="text-mystic text-sm uppercase">Скрытые</h2>
          <StatBar label="Осознанность" value={s.awareness} color="bg-mystic" />
          <StatBar label="Восприимчивость" value={s.sensitivity} />
          <StatBar label="Интуиция" value={s.intuition} />
          <StatBar label="Связь с Изнанкой" value={s.underworldBond} color="bg-purple-400" />
          <StatBar label="Сопротивление" value={s.influenceResistance} />
          <StatBar label="Воля" value={s.willpower} color="bg-amber-500" />
        </section>
      </div>
    </div>
  );
}
