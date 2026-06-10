import { NavLink, Outlet } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const nav = [
  { to: '/game', label: 'Игра' },
  { to: '/map', label: 'Карта' },
  { to: '/character', label: 'Персонаж' },
  { to: '/inventory', label: 'Инвентарь' },
  { to: '/investigation', label: 'Расследование' },
  { to: '/journal', label: 'Журнал' },
  { to: '/relations', label: 'Отношения' },
  { to: '/settings', label: 'Настройки' },
];

export default function Layout() {
  const { email, logout } = useAuth();

  return (
    <div className="min-h-screen flex flex-col bg-void">
      <header className="border-b border-border bg-surface/80 backdrop-blur sticky top-0 z-10">
        <div className="max-w-6xl mx-auto px-4 py-3 flex items-center justify-between gap-4">
          <NavLink to="/" className="font-[family-name:var(--font-display)] text-xl text-accent tracking-wide">
            People of Dark Mind
          </NavLink>
          <nav className="hidden md:flex gap-1 flex-wrap justify-center">
            {nav.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) =>
                  `px-3 py-1.5 rounded text-sm transition-colors ${
                    isActive ? 'bg-accent/20 text-accent' : 'text-muted hover:text-white hover:bg-panel'
                  }`
                }
              >
                {item.label}
              </NavLink>
            ))}
          </nav>
          <div className="flex items-center gap-3 text-sm text-muted shrink-0">
            <span className="hidden sm:inline truncate max-w-[140px]">{email}</span>
            <button type="button" onClick={logout} className="text-danger hover:underline">
              Выйти
            </button>
          </div>
        </div>
      </header>
      <main className="flex-1 max-w-6xl w-full mx-auto px-4 py-6">
        <Outlet />
      </main>
    </div>
  );
}
