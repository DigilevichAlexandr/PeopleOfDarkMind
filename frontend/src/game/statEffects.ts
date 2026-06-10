import type { CharacterStats } from './types';

const clamp = (v: number, min = 0, max = 100) => Math.max(min, Math.min(max, v));

export function applyEffects(stats: CharacterStats, effects: Record<string, number>) {
  for (const [key, value] of Object.entries(effects)) {
    switch (key.toLowerCase()) {
      case 'money':
        stats.money = clamp(stats.money + value, 0, 999999);
        break;
      case 'mood':
        stats.mood = clamp(stats.mood + value);
        break;
      case 'energy':
        stats.energy = clamp(stats.energy + value);
        break;
      case 'health':
        stats.health = clamp(stats.health + value);
        break;
      case 'reputation':
        stats.reputation = clamp(stats.reputation + value);
        break;
      case 'stress':
        stats.stress = clamp(stats.stress + value);
        break;
      case 'awareness':
        stats.awareness = clamp(stats.awareness + value);
        break;
      case 'sensitivity':
        stats.sensitivity = clamp(stats.sensitivity + value);
        break;
      case 'intuition':
        stats.intuition = clamp(stats.intuition + value);
        break;
      case 'underworldbond':
        stats.underworldBond = clamp(stats.underworldBond + value);
        break;
      case 'influenceresistance':
        stats.influenceResistance = clamp(stats.influenceResistance + value);
        break;
      case 'willpower':
        stats.willpower = clamp(stats.willpower + value);
        break;
    }
  }
}

export function clampStats(stats: CharacterStats) {
  stats.mood = clamp(stats.mood);
  stats.energy = clamp(stats.energy);
  stats.health = clamp(stats.health);
  stats.reputation = clamp(stats.reputation);
  stats.stress = clamp(stats.stress);
  stats.awareness = clamp(stats.awareness);
}
