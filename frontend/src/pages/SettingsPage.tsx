import { useNavigate } from 'react-router-dom';
import { useGame } from '../context/GameContext';

export default function SettingsPage() {
  const { resetGame } = useGame();
  const navigate = useNavigate();

  const clearSave = () => {
    if (!confirm('Удалить сохранение? Прогресс будет потерян.')) return;
    resetGame();
    navigate('/new-game');
  };

  return (
    <div className="max-w-lg">
      <h1 className="font-[family-name:var(--font-display)] text-3xl mb-6">Настройки</h1>
      <div className="bg-panel border border-border rounded-xl p-5 space-y-4 text-sm">
        <p className="text-muted">Автосохранение: включено (после каждого действия)</p>
        <p className="text-muted">Хранилище: localStorage браузера</p>
        <button type="button" onClick={clearSave} className="text-danger hover:underline">
          Сбросить сохранение
        </button>
      </div>
    </div>
  );
}
