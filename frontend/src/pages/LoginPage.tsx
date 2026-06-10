import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    try {
      await login(email, password);
      navigate('/game');
    } catch {
      setError('Неверный email или пароль');
    }
  };

  return (
    <AuthForm title="Вход" error={error} onSubmit={submit}>
      <input type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} required className="input" />
      <input type="password" placeholder="Пароль" value={password} onChange={(e) => setPassword(e.target.value)} required className="input" />
      <button type="submit" className="btn-primary w-full">Войти</button>
      <p className="text-sm text-muted text-center">
        Нет аккаунта? <Link to="/register" className="text-accent">Регистрация</Link>
      </p>
    </AuthForm>
  );
}

function AuthForm({ title, error, onSubmit, children }: { title: string; error: string; onSubmit: (e: React.FormEvent) => void; children: React.ReactNode }) {
  return (
    <div className="min-h-screen flex items-center justify-center px-4 bg-void">
      <form onSubmit={onSubmit} className="w-full max-w-md bg-panel border border-border rounded-xl p-8 space-y-4">
        <h1 className="font-[family-name:var(--font-display)] text-2xl text-center">{title}</h1>
        {error && <p className="text-danger text-sm text-center">{error}</p>}
        {children}
      </form>
    </div>
  );
}

export { AuthForm };
