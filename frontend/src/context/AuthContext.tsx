import { createContext, useContext, useState, type ReactNode } from 'react';

interface AuthContextType {
  playerName: string | null;
  setPlayerName: (name: string) => void;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);
const NAME_KEY = 'podm_player';

export function AuthProvider({ children }: { children: ReactNode }) {
  const [playerName, setName] = useState<string | null>(localStorage.getItem(NAME_KEY));

  const setPlayerName = (name: string) => {
    const trimmed = name.trim() || 'Игрок';
    localStorage.setItem(NAME_KEY, trimmed);
    setName(trimmed);
  };

  const logout = () => {
    localStorage.removeItem(NAME_KEY);
    setName(null);
  };

  return (
    <AuthContext.Provider value={{ playerName, setPlayerName, logout, isAuthenticated: !!playerName }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth outside provider');
  return ctx;
}
