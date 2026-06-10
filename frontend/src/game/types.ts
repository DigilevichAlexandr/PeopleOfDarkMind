export type TimePeriod = 'Morning' | 'Day' | 'Evening' | 'Night';

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

export interface LocationDef {
  code: string;
  name: string;
  description: string;
  district: string;
  isMystic: boolean;
  requiresAwareness: boolean;
  minAwareness: number;
  mapX: number;
  mapY: number;
  icon: string;
}

export interface NpcDef {
  code: string;
  name: string;
  description: string;
  portraitEmoji: string;
  faction: string | null;
  locationCode: string;
}

export interface EventChoiceDef {
  id: string;
  text: string;
  outcomeText: string;
  effects: Record<string, number>;
  unlocksEvidenceCode?: string;
  minMoney?: number;
  minWillpower?: number;
  minAwareness?: number;
}

export interface GameEventDef {
  code: string;
  title: string;
  description: string;
  category: string;
  isRandomPool: boolean;
  weight: number;
  minDay: number;
  minChapter: number;
  storyOrder: number;
  locationCode?: string;
  npcCode?: string;
  requiredAwareness?: number;
  choices: EventChoiceDef[];
}

export interface EvidenceDef {
  code: string;
  title: string;
  description: string;
  type: string;
  chapter: number;
}

export interface JournalEntry {
  id: string;
  day: number;
  title: string;
  content: string;
  createdAt: string;
}

export interface NpcRelation {
  hasMet: boolean;
  trust: number;
  affection: number;
  fear: number;
}

export interface SaveData {
  characterName: string;
  age: number;
  currentDay: number;
  currentPeriod: TimePeriod;
  storyChapter: number;
  storyProgress: number;
  underworldUnlocked: boolean;
  isGameOver: boolean;
  currentLocationCode: string;
  pendingEventCode: string | null;
  stats: CharacterStats;
  completedEventCodes: string[];
  journal: JournalEntry[];
  collectedEvidenceCodes: string[];
  npcRelations: Record<string, NpcRelation>;
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
  currentLocation: {
    id: string;
    code: string;
    name: string;
    description: string;
    district: string;
    isMystic: boolean;
    mapX: number;
    mapY: number;
    icon: string;
  } | null;
}

export interface GameState {
  character: Character;
  currentEvent: GameEvent | null;
  availableActions: string[];
  periodLabel: string;
  dayLabel: string;
}

export type GameResult<T = void> =
  | { ok: true; data: T }
  | { ok: false; error: string };
