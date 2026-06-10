import { useCallback, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';
import api, { type GameState } from '../api/client';
import StatBar from '../components/StatBar';

const ACTION_LABELS: Record<string, string> = {
  JobSearch: 'Искать работу',
  Interview: 'Собеседование',
  Freelance: 'Фриланс',
  Stream: 'Стрим',
  Video: 'Записать видео',
  LanguageStudy: 'Языки',
  WalkCity: 'Прогулка',
  Socialize: 'Общение',
  Read: 'Читать',
  Sport: 'Спорт',
  Investigate: 'Расследовать',
  VisitLocation: 'Локация',
  Rest: 'Отдых',
  PayBills: 'Оплатить счета',
};

function apiError(err: unknown): string {
  if (axios.isAxiosError(err)) {
    return (err.response?.data as { error?: string })?.error ?? err.message;
  }
  return 'Неизвестная ошибка';
}

export default function GamePage() {
  const [state, setState] = useState<GameState | null>(null);
  const [error, setError] = useState('');
  const [actionError, setActionError] = useState('');
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);

  const load = useCallback(async () => {
    try {
      const { data } = await api.get<GameState>('/game/state');
      setState(data);
      setError('');
    } catch {
      setError('Нет активной игры');
      setState(null);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  const performAction = async (actionType: string) => {
    setBusy(true);
    setActionError('');
    try {
      const { data } = await api.post<GameState>('/game/action', { actionType });
      setState(data);
    } catch (err) {
      setActionError(apiError(err));
    } finally {
      setBusy(false);
    }
  };

  const triggerRandom = async () => {
    setBusy(true);
    setActionError('');
    try {
      const { data } = await api.post<GameState>('/game/random-event');
      setState(data);
    } catch (err) {
      setActionError(apiError(err));
    } finally {
      setBusy(false);
    }
  };

  const makeChoice = async (eventId: string, choiceId: string) => {
    setBusy(true);
    setActionError('');
    try {
      const { data } = await api.post<GameState>('/game/choice', { eventId, choiceId });
      setState(data);
    } catch (err) {
      setActionError(apiError(err));
    } finally {
      setBusy(false);
    }
  };

  if (loading) return <p className="text-muted">Загрузка...</p>;
  if (!state) {
    return (
      <div className="text-center py-16">
        <p className="text-muted mb-6">{error}</p>
        <Link to="/new-game" className="btn-primary">Начать новую игру</Link>
      </div>
    );
  }

  const { character: c, currentEvent, periodLabel, dayLabel } = state;
  const s = c.stats;

  return (
    <div className="grid lg:grid-cols-3 gap-6">
      <div className="lg:col-span-2 space-y-6">
        <header className="bg-panel border border-border rounded-xl p-5">
          <div className="flex flex-wrap justify-between gap-2 mb-4">
            <span className="text-accent font-medium">{dayLabel} · {periodLabel}</span>
            <span className="text-muted text-sm">Глава {c.storyChapter}</span>
          </div>
          <h2 className="font-[family-name:var(--font-display)] text-2xl">{c.name}, {c.age} лет</h2>
          {c.currentLocation && (
            <p className="text-muted mt-1">{c.currentLocation.icon} {c.currentLocation.name}</p>
          )}
        </header>

        {actionError && (
          <p className="text-danger text-sm bg-danger/10 border border-danger/30 rounded-lg px-4 py-2">
            {actionError}
          </p>
        )}

        {currentEvent ? (
          <section className="bg-panel border border-mystic/30 rounded-xl p-5">
            <span className="text-xs text-mystic uppercase tracking-wider">{currentEvent.category}</span>
            <h3 className="text-xl font-[family-name:var(--font-display)] mt-1 mb-2">{currentEvent.title}</h3>
            <p className="text-slate-300 leading-relaxed mb-6">{currentEvent.description}</p>
            <div className="space-y-2">
              {currentEvent.choices.map((ch) => (
                <button
                  key={ch.id}
                  type="button"
                  disabled={!ch.isAvailable || busy}
                  onClick={() => makeChoice(currentEvent.id, ch.id)}
                  className="w-full text-left px-4 py-3 rounded-lg border border-border hover:border-accent disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
                >
                  {ch.text}
                  {!ch.isAvailable && ch.unavailableReason && (
                    <span className="block text-xs text-danger mt-1">{ch.unavailableReason}</span>
                  )}
                </button>
              ))}
            </div>
          </section>
        ) : (
          <section className="bg-panel border border-border rounded-xl p-5">
            <div className="flex flex-wrap items-center justify-between gap-3 mb-4">
              <h3 className="text-lg">Действие за этот период</h3>
              <button
                type="button"
                disabled={busy}
                onClick={triggerRandom}
                className="text-sm px-3 py-1.5 rounded-lg border border-mystic/50 text-mystic hover:bg-mystic/10 disabled:opacity-50"
              >
                Случайное событие
              </button>
            </div>
            <div className="grid sm:grid-cols-2 gap-2">
              {state.availableActions.map((a) => (
                <button
                  key={a}
                  type="button"
                  disabled={busy}
                  onClick={() => performAction(a)}
                  className="action-btn disabled:opacity-50"
                >
                  {ACTION_LABELS[a] ?? a}
                </button>
              ))}
            </div>
            <p className="text-xs text-muted mt-4">
              «Расследовать» и «Прогулка» чаще вызывают мистические события.
            </p>
          </section>
        )}
      </div>

      <aside className="space-y-4">
        <Panel title="Состояние">
          <StatBar label="Настроение" value={s.mood} />
          <StatBar label="Энергия" value={s.energy} color="bg-emerald-500" />
          <StatBar label="Здоровье" value={s.health} color="bg-rose-500" />
          <StatBar label="Стресс" value={s.stress} color="bg-orange-500" />
          <StatBar label="Репутация" value={s.reputation} color="bg-sky-500" />
          <p className="text-sm pt-2">💰 {s.money.toLocaleString('ru-RU')} ₽</p>
        </Panel>
        {(c.underworldUnlocked || s.awareness > 0) && (
          <Panel title="Скрытое">
            <StatBar label="Осознанность" value={s.awareness} color="bg-mystic" />
            <StatBar label="Связь с Изнанкой" value={s.underworldBond} color="bg-purple-400" />
          </Panel>
        )}
      </aside>
    </div>
  );
}

function Panel({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="bg-panel border border-border rounded-xl p-4 space-y-3">
      <h4 className="text-sm text-muted uppercase tracking-wide">{title}</h4>
      {children}
    </div>
  );
}
