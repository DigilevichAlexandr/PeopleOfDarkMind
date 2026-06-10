import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function RegisterPage() {
  const [name, setName] = useState('Алексей');
  const { setPlayerName } = useAuth();
  const navigate = useNavigate();

  const submit = (e: React.FormEvent) => {
    e.preventDefault();
    setPlayerName(name);
    navigate('/new-game');
  };

  return (
    <div className="min-h-screen flex items-center justify-center px-4 bg-void">
      <form onSubmit={submit} className="w-full max-w-md bg-panel border border-border rounded-xl p-6 space-y-4">
        <h1 className="font-[family-name:var(--font-display)] text-2xl">Начать</h1>
        <p className="text-sm text-muted">Создайте локальный профиль — без сервера и пароля</p>
        <input
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="input"
          placeholder="Имя игрока"
          required
        />
        <button type="submit" className="btn-primary w-full">Далее</button>
        <p className="text-sm text-muted text-center">
          Уже играли? <Link to="/login" className="text-accent hover:underline">Войти</Link>
        </p>
      </form>
    </div>
  );
}
