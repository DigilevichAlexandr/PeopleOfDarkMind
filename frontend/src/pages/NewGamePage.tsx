import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/client';

export default function NewGamePage() {
  const [name, setName] = useState('Алексей');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const start = async () => {
    setError('');
    try {
      await api.post('/game/new', { characterName: name });
      navigate('/game');
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { error?: string } } })?.response?.data?.error;
      setError(msg ?? 'Ошибка создания игры');
    }
  };

  return (
    <div className="max-w-lg mx-auto">
      <h1 className="font-[family-name:var(--font-display)] text-3xl mb-2">Новая игра</h1>
      <p className="text-muted mb-8">
        Вам 35 лет. Работа потеряна. Мать рядом. Город скрывает тайны.
      </p>
      <div className="bg-panel border border-border rounded-xl p-6 space-y-4">
        <label className="block text-sm text-muted">Имя героя</label>
        <input value={name} onChange={(e) => setName(e.target.value)} className="input" />
        {error && <p className="text-danger text-sm">{error}</p>}
        <button type="button" onClick={start} className="btn-primary w-full">Начать главу 1: Падение</button>
      </div>
    </div>
  );
}
