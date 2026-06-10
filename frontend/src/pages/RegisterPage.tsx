import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { AuthForm } from './LoginPage';

export default function RegisterPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [name, setName] = useState('');
  const [error, setError] = useState('');
  const { register } = useAuth();
  const navigate = useNavigate();

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    try {
      await register(email, password, name || undefined);
      navigate('/new-game');
    } catch {
      setError('Не удалось зарегистрироваться');
    }
  };

  return (
    <AuthForm title="Регистрация" error={error} onSubmit={submit}>
      <input type="text" placeholder="Имя героя (опционально)" value={name} onChange={(e) => setName(e.target.value)} className="input" />
      <input type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} required className="input" />
      <input type="password" placeholder="Пароль (мин. 6)" value={password} onChange={(e) => setPassword(e.target.value)} required minLength={6} className="input" />
      <button type="submit" className="btn-primary w-full">Создать аккаунт</button>
      <p className="text-sm text-muted text-center">
        Уже есть аккаунт? <Link to="/login" className="text-accent">Войти</Link>
      </p>
    </AuthForm>
  );
}
