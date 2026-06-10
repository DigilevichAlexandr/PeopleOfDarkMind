import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from 'react';
import {
  buildState, createNewSave, getEvidence, getJournal, getLocations, getNpcs,
  makeChoice, performAction, travel, triggerRandom,
} from '../game/gameEngine';
import { deleteSave, hasSave, loadSave, storeSave } from '../game/storage';
import type { GameResult, GameState, SaveData } from '../game/types';

interface GameContextType {
  state: GameState | null;
  hasSave: boolean;
  refresh: () => void;
  newGame: (name: string) => GameResult<GameState>;
  performAction: (actionType: string) => GameResult<GameState>;
  makeChoice: (eventId: string, choiceId: string) => GameResult<GameState>;
  triggerRandom: () => GameResult<GameState>;
  travelTo: (locationId: string) => GameResult<GameState>;
  getLocations: () => ReturnType<typeof getLocations>;
  getJournal: () => ReturnType<typeof getJournal>;
  getEvidence: () => ReturnType<typeof getEvidence>;
  getNpcs: () => ReturnType<typeof getNpcs>;
  resetGame: () => void;
}

const GameContext = createContext<GameContextType | null>(null);

function persist(save: SaveData) {
  storeSave(save);
  return buildState(save);
}

export function GameProvider({ children }: { children: ReactNode }) {
  const [save, setSave] = useState<SaveData | null>(() => loadSave());

  const refresh = useCallback(() => setSave(loadSave()), []);

  const wrap = useCallback((fn: (s: SaveData) => GameResult<GameState>): GameResult<GameState> => {
    const current = loadSave();
    if (!current) return { ok: false, error: 'Нет активной игры' };
    const result = fn(current);
    if (result.ok) {
      storeSave(current);
      setSave({ ...current });
    }
    return result;
  }, []);

  const value = useMemo<GameContextType>(() => ({
    state: save ? buildState(save) : null,
    hasSave: hasSave(),
    refresh,
    newGame: (name) => {
      if (hasSave()) return { ok: false, error: 'Игра уже существует. Сбросьте сохранение в настройках.' };
      const created = createNewSave(name);
      const state = persist(created);
      setSave(created);
      return { ok: true, data: state };
    },
    performAction: (actionType) => wrap((s) => performAction(s, actionType)),
    makeChoice: (eventId, choiceId) => wrap((s) => makeChoice(s, eventId, choiceId)),
    triggerRandom: () => wrap((s) => triggerRandom(s)),
    travelTo: (locationId) => wrap((s) => travel(s, locationId)),
    getLocations: () => {
      const s = loadSave();
      return s ? getLocations(s) : [];
    },
    getJournal: () => {
      const s = loadSave();
      return s ? getJournal(s) : [];
    },
    getEvidence: () => {
      const s = loadSave();
      return s ? getEvidence(s) : [];
    },
    getNpcs: () => {
      const s = loadSave();
      return s ? getNpcs(s) : [];
    },
    resetGame: () => {
      deleteSave();
      setSave(null);
    },
  }), [save, refresh, wrap]);

  return <GameContext.Provider value={value}>{children}</GameContext.Provider>;
}

export function useGame() {
  const ctx = useContext(GameContext);
  if (!ctx) throw new Error('useGame outside provider');
  return ctx;
}
