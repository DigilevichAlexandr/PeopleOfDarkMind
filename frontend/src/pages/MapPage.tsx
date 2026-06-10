import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/client';

interface Location {
  id: string;
  code: string;
  name: string;
  description: string;
  district: string;
  isMystic: boolean;
  isAccessible: boolean;
  mapX: number;
  mapY: number;
  icon: string;
}

export default function MapPage() {
  const [locations, setLocations] = useState<Location[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    api.get<Location[]>('/game/locations').then((r) => setLocations(r.data));
  }, []);

  const travel = async (id: string) => {
    await api.post(`/game/travel/${id}`);
    navigate('/game');
  };

  return (
    <div>
      <h1 className="font-[family-name:var(--font-display)] text-3xl mb-2">Карта города</h1>
      <p className="text-muted mb-6">Выберите локацию для перемещения</p>
      <div className="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {locations.map((loc) => (
          <button
            key={loc.id}
            type="button"
            disabled={!loc.isAccessible}
            onClick={() => travel(loc.id)}
            className={`text-left p-4 rounded-xl border transition-colors ${
              loc.isMystic ? 'border-mystic/40 bg-mystic/5' : 'border-border bg-panel'
            } ${loc.isAccessible ? 'hover:border-accent cursor-pointer' : 'opacity-40 cursor-not-allowed'}`}
          >
            <span className="text-2xl">{loc.icon}</span>
            <h3 className="font-medium mt-2">{loc.name}</h3>
            <p className="text-xs text-muted">{loc.district}</p>
            <p className="text-sm text-slate-400 mt-2 line-clamp-2">{loc.description}</p>
          </button>
        ))}
      </div>
    </div>
  );
}
