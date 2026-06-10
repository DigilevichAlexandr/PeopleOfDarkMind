import axios from 'axios';

const api = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export default api;

export interface AuthResponse {
  token: string;
  email: string;
  userId: string;
  expiresAt: string;
}

export interface CharacterStats {
  money: number;
  mood: number;
  energy: number;
  health: number;
  reputation: number;
  stress: number;
  awareness: number;
  sensitivity: number;
  intuition: number;
  underworldBond: number;
  influenceResistance: number;
  willpower: number;
}

export interface Character {
  id: string;
  name: string;
  age: number;
  currentDay: number;
  currentPeriod: string;
  storyChapter: number;
  storyProgress: number;
  underworldUnlocked: boolean;
  isGameOver: boolean;
  ending: string | null;
  stats: CharacterStats;
  currentLocation: { id: string; code: string; name: string; icon: string } | null;
}

export interface EventChoice {
  id: string;
  text: string;
  isAvailable: boolean;
  unavailableReason: string | null;
}

export interface GameEvent {
  id: string;
  code: string;
  title: string;
  description: string;
  category: string;
  choices: EventChoice[];
}

export interface GameState {
  character: Character;
  currentEvent: GameEvent | null;
  availableActions: string[];
  periodLabel: string;
  dayLabel: string;
}
