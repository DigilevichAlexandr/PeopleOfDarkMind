import { EVIDENCE, EVENTS, LOCATIONS, NPCS, eventByCode, locationByCode } from './gameData';
import { applyEffects, clampStats } from './statEffects';
import type {
  Character,
  CharacterStats,
  GameEvent,
  GameResult,
  GameState,
  LocationDef,
  NpcRelation,
  SaveData,
  TimePeriod,
} from './types';

const PERIOD_LABELS: Record<TimePeriod, string> = {
  Morning: 'Утро',
  Day: 'День',
  Evening: 'Вечер',
  Night: 'Ночь',
};

const ACTIONS = [
  'JobSearch', 'Interview', 'Freelance', 'Stream', 'Video',
  'LanguageStudy', 'WalkCity', 'Socialize', 'Read', 'Sport',
  'Investigate', 'VisitLocation', 'Rest', 'PayBills',
];

function defaultStats(): CharacterStats {
  return {
    money: 15000, mood: 50, energy: 80, health: 90, reputation: 30, stress: 40,
    awareness: 5, sensitivity: 10, intuition: 15, underworldBond: 0,
    influenceResistance: 20, willpower: 50,
  };
}

function defaultNpcRelations(): Record<string, NpcRelation> {
  const rel: Record<string, NpcRelation> = {};
  for (const n of NPCS) {
    rel[n.code] = { hasMet: n.code === 'mother', trust: 50, affection: 50, fear: 0 };
  }
  return rel;
}

export function createNewSave(characterName: string): SaveData {
  return {
    characterName: characterName.trim() || 'Алексей',
    age: 35,
    currentDay: 1,
    currentPeriod: 'Morning',
    storyChapter: 1,
    storyProgress: 0,
    underworldUnlocked: false,
    isGameOver: false,
    currentLocationCode: 'home',
    pendingEventCode: null,
    stats: defaultStats(),
    completedEventCodes: [],
    journal: [{
      id: crypto.randomUUID(),
      day: 1,
      title: 'Глава 1: Падение',
      content: 'Три месяца без работы. Мать в соседней комнате. Квартира от деда ждёт арендаторов. Город шумит за окном, но ты слышишь только тишину собственных мыслей.',
      createdAt: new Date().toISOString(),
    }],
    collectedEvidenceCodes: [],
    npcRelations: defaultNpcRelations(),
  };
}

function advanceTime(save: SaveData) {
  const order: TimePeriod[] = ['Morning', 'Day', 'Evening', 'Night'];
  const idx = order.indexOf(save.currentPeriod);
  save.currentPeriod = order[(idx + 1) % order.length];
  if (save.currentPeriod === 'Morning') save.currentDay++;
}

function applyAction(save: SaveData, actionType: string) {
  const s = save.stats;
  const rnd = (max: number) => Math.floor(Math.random() * max);
  switch (actionType) {
    case 'JobSearch': s.energy -= 15; s.stress += 10; s.mood -= 5; break;
    case 'Interview': s.energy -= 20; s.stress += 15; s.reputation += 5; break;
    case 'Freelance': s.money += 3000 + rnd(2000); s.energy -= 25; break;
    case 'Stream': s.money += 500 + rnd(1500); s.energy -= 20; s.mood += 5; break;
    case 'Video': s.money += 200 + rnd(800); s.energy -= 15; break;
    case 'LanguageStudy': s.energy -= 10; s.mood += 3; s.reputation += 2; break;
    case 'WalkCity': s.energy -= 10; s.mood += 8; s.stress -= 5; s.awareness += 1; break;
    case 'Socialize': s.energy -= 15; s.mood += 10; s.stress -= 8; break;
    case 'Read': s.energy -= 5; s.mood += 5; s.awareness += 2; break;
    case 'Sport': s.health += 5; s.energy -= 20; s.stress -= 10; s.mood += 5; break;
    case 'Investigate': s.energy -= 25; s.stress += 10; s.awareness += 3; s.intuition += 2; break;
    case 'Rest': s.energy = Math.min(100, s.energy + 30); s.health += 3; break;
    case 'PayBills': s.money -= 8000; s.stress -= 5; break;
    case 'VisitLocation': s.energy -= 5; break;
  }
  clampStats(s);
}

function tryRandomEvent(save: SaveData, mysticBias: boolean): boolean {
  const available = EVENTS.filter((e) =>
    e.isRandomPool &&
    e.minDay <= save.currentDay &&
    !save.completedEventCodes.includes(e.code) &&
    (!e.requiredAwareness || save.stats.awareness >= e.requiredAwareness)
  );
  if (!available.length) return false;

  let pool = available;
  if (mysticBias) {
    const mystic = available.filter((e) => e.category === 'Mystic' || e.category === 'Underworld');
    if (mystic.length) pool = mystic;
  }

  const total = pool.reduce((s, e) => s + e.weight, 0);
  let roll = Math.floor(Math.random() * total);
  let selected = pool[pool.length - 1];
  for (const e of pool) {
    roll -= e.weight;
    if (roll < 0) { selected = e; break; }
  }
  save.pendingEventCode = selected.code;
  return true;
}

function checkChapter(save: SaveData) {
  if (save.storyChapter === 1 && save.storyProgress >= 5) {
    save.storyChapter = 2;
    save.journal.unshift({
      id: crypto.randomUUID(),
      day: save.currentDay,
      title: 'Пробуждение',
      content: 'Что-то изменилось. Тени движутся иначе. Люди смотрят сквозь тебя.',
      createdAt: new Date().toISOString(),
    });
  }
  if (save.stats.awareness >= 25 && !save.underworldUnlocked) {
    save.underworldUnlocked = true;
  }
}

function mapEvent(save: SaveData, code: string): GameEvent | null {
  const ev = eventByCode.get(code);
  if (!ev) return null;
  return {
    id: ev.code,
    code: ev.code,
    title: ev.title,
    description: ev.description,
    category: ev.category,
    choices: ev.choices.map((ch) => {
      let available = true;
      let reason: string | null = null;
      if (ch.minAwareness && save.stats.awareness < ch.minAwareness) {
        available = false; reason = 'Недостаточно осознанности';
      }
      if (ch.minMoney && save.stats.money < ch.minMoney) {
        available = false; reason = 'Недостаточно денег';
      }
      if (ch.minWillpower && save.stats.willpower < ch.minWillpower) {
        available = false; reason = 'Недостаточно воли';
      }
      return { id: ch.id, text: ch.text, isAvailable: available, unavailableReason: reason };
    }),
  };
}

function toCharacter(save: SaveData): Character {
  const loc = locationByCode.get(save.currentLocationCode);
  return {
    id: 'local',
    name: save.characterName,
    age: save.age,
    currentDay: save.currentDay,
    currentPeriod: PERIOD_LABELS[save.currentPeriod],
    storyChapter: save.storyChapter,
    storyProgress: save.storyProgress,
    underworldUnlocked: save.underworldUnlocked,
    isGameOver: save.isGameOver,
    ending: null,
    stats: { ...save.stats },
    currentLocation: loc ? {
      id: loc.code,
      code: loc.code,
      name: loc.name,
      description: loc.description,
      district: loc.district,
      isMystic: loc.isMystic,
      mapX: loc.mapX,
      mapY: loc.mapY,
      icon: loc.icon,
    } : null,
  };
}

export function buildState(save: SaveData): GameState {
  const currentEvent = save.pendingEventCode ? mapEvent(save, save.pendingEventCode) : null;
  return {
    character: toCharacter(save),
    currentEvent,
    availableActions: ACTIONS,
    periodLabel: PERIOD_LABELS[save.currentPeriod],
    dayLabel: `День ${save.currentDay}`,
  };
}

export function performAction(save: SaveData, actionType: string): GameResult<GameState> {
  if (save.isGameOver) return { ok: false, error: 'Игра окончена' };
  if (save.pendingEventCode) return { ok: false, error: 'Сначала завершите текущее событие' };

  const dayBefore = save.currentDay;
  applyAction(save, actionType);
  advanceTime(save);
  const dayAdvanced = save.currentDay > dayBefore;

  if (actionType === 'Investigate' || actionType === 'WalkCity') {
    tryRandomEvent(save, true);
  } else if (dayAdvanced && save.currentPeriod === 'Morning') {
    tryRandomEvent(save, false);
  } else if (Math.random() < 0.25) {
    tryRandomEvent(save, false);
  }

  return { ok: true, data: buildState(save) };
}

export function triggerRandom(save: SaveData): GameResult<GameState> {
  if (save.pendingEventCode) return { ok: false, error: 'Сначала завершите текущее событие' };
  if (!tryRandomEvent(save, false)) return { ok: false, error: 'Нет доступных случайных событий' };
  return { ok: true, data: buildState(save) };
}

export function makeChoice(save: SaveData, eventCode: string, choiceId: string): GameResult<GameState> {
  if (!save.pendingEventCode) return { ok: false, error: 'Нет активного события' };
  if (save.pendingEventCode !== eventCode) return { ok: false, error: 'Активно другое событие' };

  const ev = eventByCode.get(eventCode);
  if (!ev) return { ok: false, error: 'Событие не найдено' };
  const choice = ev.choices.find((c) => c.id === choiceId);
  if (!choice) return { ok: false, error: 'Выбор не найден' };

  if (choice.minAwareness && save.stats.awareness < choice.minAwareness)
    return { ok: false, error: 'Недостаточно осознанности' };
  if (choice.minMoney && save.stats.money < choice.minMoney)
    return { ok: false, error: 'Недостаточно денег' };
  if (choice.minWillpower && save.stats.willpower < choice.minWillpower)
    return { ok: false, error: 'Недостаточно воли' };

  applyEffects(save.stats, choice.effects);
  clampStats(save.stats);
  save.storyProgress++;
  save.pendingEventCode = null;

  if (!save.completedEventCodes.includes(ev.code)) {
    save.completedEventCodes.push(ev.code);
  }

  save.journal.unshift({
    id: crypto.randomUUID(),
    day: save.currentDay,
    title: ev.title,
    content: choice.outcomeText,
    createdAt: new Date().toISOString(),
  });

  if (choice.unlocksEvidenceCode && !save.collectedEvidenceCodes.includes(choice.unlocksEvidenceCode)) {
    save.collectedEvidenceCodes.push(choice.unlocksEvidenceCode);
  }

  checkChapter(save);
  return { ok: true, data: buildState(save) };
}

export function travel(save: SaveData, locationCode: string): GameResult<GameState> {
  if (save.pendingEventCode) return { ok: false, error: 'Сначала завершите текущее событие' };
  const loc = locationByCode.get(locationCode);
  if (!loc) return { ok: false, error: 'Локация не найдена' };
  if (loc.requiresAwareness && save.stats.awareness < loc.minAwareness)
    return { ok: false, error: 'Вы ещё не видите эту сторону города.' };

  save.currentLocationCode = loc.code;
  save.stats.energy = Math.max(0, save.stats.energy - 5);

  const locationEvent = EVENTS
    .filter((e) => !e.isRandomPool && e.locationCode === loc.code)
    .filter((e) => e.minDay <= save.currentDay && e.minChapter <= save.storyChapter)
    .filter((e) => !save.completedEventCodes.includes(e.code))
    .sort((a, b) => a.storyOrder - b.storyOrder)[0];

  if (locationEvent) save.pendingEventCode = locationEvent.code;
  return { ok: true, data: buildState(save) };
}

export function getLocations(save: SaveData) {
  return LOCATIONS.map((loc: LocationDef) => ({
    id: loc.code,
    code: loc.code,
    name: loc.name,
    description: loc.description,
    district: loc.district,
    isMystic: loc.isMystic,
    isAccessible: !loc.requiresAwareness || save.stats.awareness >= loc.minAwareness,
    mapX: loc.mapX,
    mapY: loc.mapY,
    icon: loc.icon,
  }));
}

export function getJournal(save: SaveData) {
  return [...save.journal];
}

export function getEvidence(save: SaveData) {
  return EVIDENCE.map((e) => ({
    id: e.code,
    code: e.code,
    title: e.title,
    description: e.description,
    type: e.type,
    isCollected: save.collectedEvidenceCodes.includes(e.code),
    isConnected: false,
    collectedAt: save.collectedEvidenceCodes.includes(e.code) ? new Date().toISOString() : null,
  }));
}

export function getNpcs(save: SaveData) {
  return NPCS.map((n) => {
    const rel = save.npcRelations[n.code];
    return {
      id: n.code,
      code: n.code,
      name: n.name,
      description: n.description,
      portraitEmoji: n.portraitEmoji,
      trust: rel?.trust ?? 50,
      affection: rel?.affection ?? 50,
      fear: rel?.fear ?? 0,
      hasMet: rel?.hasMet ?? false,
      faction: n.faction,
    };
  });
}
