import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function HomePage() {
  const { isAuthenticated } = useAuth();

  return (
    <div className="min-h-screen flex flex-col items-center justify-center px-4 bg-void relative overflow-hidden">
      <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_top,_#1a1033_0%,_transparent_50%)] pointer-events-none" />
      <img src="/logo.svg" alt="" className="w-24 h-24 mb-6 opacity-90" />
      <h1 className="font-[family-name:var(--font-display)] text-4xl md:text-5xl text-center text-white mb-3">
        People of Dark Mind
      </h1>
      <p className="text-muted text-center max-w-lg mb-10 leading-relaxed">
        Текстовая RPG о программисте, потерявшем работу, и тайном мире за гранью реальности.
        Выживайте, расследуйте, выбирайте судьбу.
      </p>
      <div className="flex flex-col sm:flex-row gap-4 z-10">
        {isAuthenticated ? (
          <>
            <Link to="/game" className="btn-primary">Продолжить</Link>
            <Link to="/new-game" className="btn-secondary">Новая игра</Link>
          </>
        ) : (
          <>
            <Link to="/register" className="btn-primary">Начать</Link>
            <Link to="/login" className="btn-secondary">Войти</Link>
          </>
        )}
      </div>
      <p className="text-xs text-muted mt-16">Вдохновлено Disco Elysium · Roadwarden · Citizen Sleeper</p>
    </div>
  );
}
