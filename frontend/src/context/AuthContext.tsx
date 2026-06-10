import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import api, { type AuthResponse } from '../api/client';

interface AuthContextType {
  token: string | null;
  email: string | null;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, displayName?: string) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
  const [email, setEmail] = useState<string | null>(localStorage.getItem('email'));

  useEffect(() => {
    if (token) localStorage.setItem('token', token);
    else localStorage.removeItem('token');
  }, [token]);

  const saveAuth = (data: AuthResponse) => {
    setToken(data.token);
    setEmail(data.email);
    localStorage.setItem('email', data.email);
  };

  const login = async (e: string, p: string) => {
    const { data } = await api.post<AuthResponse>('/auth/login', { email: e, password: p });
    saveAuth(data);
  };

  const register = async (e: string, p: string, displayName?: string) => {
    const { data } = await api.post<AuthResponse>('/auth/register', { email: e, password: p, displayName });
    saveAuth(data);
  };

  const logout = () => {
    setToken(null);
    setEmail(null);
    localStorage.removeItem('token');
    localStorage.removeItem('email');
  };

  return (
    <AuthContext.Provider value={{ token, email, login, register, logout, isAuthenticated: !!token }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth outside provider');
  return ctx;
}
