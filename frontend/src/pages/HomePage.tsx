import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useGame } from '../context/GameContext';

export default function HomePage() {
  const { isAuthenticated } = useAuth();
  const { hasSave } = useGame();

  return (
    <div className="min-h-screen flex flex-col items-center justify-center px-4 bg-void relative overflow-hidden">
      <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_top,_#1a1033_0%,_transparent_50%)] pointer-events-none" />
      <img src={`${import.meta.env.BASE_URL}logo.svg`} alt="" className="w-24 h-24 mb-6 opacity-90" />
      <h1 className="font-[family-name:var(--font-display)] text-4xl md:text-5xl text-center text-white mb-3">
        People of Dark Mind
      </h1>
      <p className="text-muted text-center max-w-lg mb-4 leading-relaxed">
        Текстовая RPG о программисте, потерявшем работу, и тайном мире за гранью реальности.
      </p>
      <p className="text-xs text-mystic mb-10">Браузерная версия · сохранение в localStorage</p>
      <div className="flex flex-col sm:flex-row gap-4 z-10">
        {isAuthenticated && hasSave ? (
          <>
            <Link to="/game" className="btn-primary">Продолжить</Link>
            <Link to="/new-game" className="btn-secondary">Новая игра</Link>
          </>
        ) : isAuthenticated ? (
          <Link to="/new-game" className="btn-primary">Начать игру</Link>
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
