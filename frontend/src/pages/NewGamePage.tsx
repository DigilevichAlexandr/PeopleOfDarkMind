import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useGame } from '../context/GameContext';

export default function NewGamePage() {
  const [name, setName] = useState('Алексей');
  const [error, setError] = useState('');
  const { newGame, resetGame } = useGame();
  const navigate = useNavigate();

  const start = () => {
    setError('');
    resetGame();
    const result = newGame(name);
    if (!result.ok) {
      setError(result.error);
      return;
    }
    navigate('/game');
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
