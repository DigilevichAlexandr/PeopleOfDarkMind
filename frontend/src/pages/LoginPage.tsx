import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useGame } from '../context/GameContext';

export default function LoginPage() {
  const [name, setName] = useState('');
  const { setPlayerName } = useAuth();
  const { hasSave } = useGame();
  const navigate = useNavigate();

  const submit = (e: React.FormEvent) => {
    e.preventDefault();
    setPlayerName(name);
    navigate(hasSave ? '/game' : '/new-game');
  };

  return (
    <div className="min-h-screen flex items-center justify-center px-4 bg-void">
      <form onSubmit={submit} className="w-full max-w-md bg-panel border border-border rounded-xl p-6 space-y-4">
        <h1 className="font-[family-name:var(--font-display)] text-2xl">Войти</h1>
        <p className="text-sm text-muted">Имя сохраняется локально в браузере</p>
        <input
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="input"
          placeholder="Ваше имя"
          required
        />
        <button type="submit" className="btn-primary w-full">Продолжить</button>
        <p className="text-sm text-muted text-center">
          Нет профиля? <Link to="/register" className="text-accent hover:underline">Создать</Link>
        </p>
      </form>
    </div>
  );
}
