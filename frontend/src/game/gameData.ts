import type { EventChoiceDef, EvidenceDef, GameEventDef, LocationDef, NpcDef } from './types';

function parseEffects(json: string): Record<string, number> {
  return JSON.parse(json) as Record<string, number>;
}

function chFromJson(eventCode: string, defs: [string, string, string, { unlocks?: string; minMoney?: number; minWill?: number }?][]): EventChoiceDef[] {
  return defs.map(([text, outcome, json, opts], i) => ({
    id: `${eventCode}_c${i}`,
    text,
    outcomeText: outcome,
    effects: parseEffects(json),
    unlocksEvidenceCode: opts?.unlocks,
    minMoney: opts?.minMoney,
    minWillpower: opts?.minWill,
  }));
}

export const LOCATIONS: LocationDef[] = [
  { code: 'home', name: 'Квартира героя', description: 'Тесная двушка с матерью. Запах кофе и тишина.', district: 'Центр', isMystic: false, requiresAwareness: false, minAwareness: 0, mapX: 2, mapY: 3, icon: '🏠' },
  { code: 'rental', name: 'Квартира для сдачи', description: 'Наследство деда. Скрипит паркет, пахнет пылью.', district: 'Север', isMystic: false, requiresAwareness: false, minAwareness: 0, mapX: 1, mapY: 1, icon: '🔑' },
  { code: 'park', name: 'Парк', description: 'Каштаны, скамейки, бегуны. Здесь легче дышать.', district: 'Центр', isMystic: false, requiresAwareness: false, minAwareness: 0, mapX: 4, mapY: 5, icon: '🌳' },
  { code: 'library', name: 'Библиотека', description: 'Тишина и пыльные тома. Бесплатный Wi‑Fi.', district: 'Центр', isMystic: false, requiresAwareness: false, minAwareness: 0, mapX: 5, mapY: 2, icon: '📚' },
  { code: 'cafe', name: 'Кафе «Сумерки»', description: 'Тусклый свет, джаз, разговоры за соседним столом.', district: 'Центр', isMystic: false, requiresAwareness: false, minAwareness: 0, mapX: 6, mapY: 4, icon: '☕' },
  { code: 'mall', name: 'Торговый центр', description: 'Неон, толпы, витрины. Город в миниатюре.', district: 'Восток', isMystic: false, requiresAwareness: false, minAwareness: 0, mapX: 8, mapY: 3, icon: '🛍️' },
  { code: 'offices', name: 'Офисный квартал', description: 'Стекло и костюмы. Здесь решают судьбы за зарплату.', district: 'Запад', isMystic: false, requiresAwareness: false, minAwareness: 0, mapX: 1, mapY: 5, icon: '🏢' },
  { code: 'factory', name: 'Заброшенный завод', description: 'Ржавчина, эхо шагов, чужие граффити.', district: 'Юг', isMystic: true, requiresAwareness: false, minAwareness: 10, mapX: 3, mapY: 8, icon: '🏭' },
  { code: 'underpass', name: 'Подземный переход', description: 'Сырость, неон, лица без имён.', district: 'Центр', isMystic: true, requiresAwareness: false, minAwareness: 5, mapX: 5, mapY: 6, icon: '🚇' },
  { code: 'hospital', name: 'Старая больница', description: 'Заколоченные окна. Говорят, слышны крики.', district: 'Север', isMystic: true, requiresAwareness: false, minAwareness: 15, mapX: 2, mapY: 7, icon: '🏥' },
  { code: 'cemetery', name: 'Кладбище', description: 'Мрамор, туман, имена забытых.', district: 'Север', isMystic: true, requiresAwareness: false, minAwareness: 8, mapX: 1, mapY: 9, icon: '⚰️' },
  { code: 'newdistrict', name: 'Район новостроек', description: 'Бетон, краны, обещания лучшей жизни.', district: 'Восток', isMystic: false, requiresAwareness: false, minAwareness: 0, mapX: 9, mapY: 1, icon: '🏗️' },
  { code: 'underworld', name: 'Изнанка', description: 'Город перевёрнут. Эмоции — как туман.', district: '—', isMystic: true, requiresAwareness: true, minAwareness: 25, mapX: 5, mapY: 5, icon: '🌑' },
];

export const NPCS: NpcDef[] = [
  { code: 'mother', name: 'Мать', description: 'Устала, но держится. Верит, что всё наладится.', portraitEmoji: '👩', faction: null, locationCode: 'home' },
  { code: 'landlord', name: 'Арендатор', description: 'Молодой IT-специалист. Платит вовремя.', portraitEmoji: '🧑‍💻', faction: null, locationCode: 'rental' },
  { code: 'hr_maria', name: 'Мария (HR)', description: 'Улыбка как маска. Знает больше, чем говорит.', portraitEmoji: '👩‍💼', faction: null, locationCode: 'offices' },
  { code: 'barista', name: 'Вера', description: 'Бариста в «Сумерках». Слушает больше, чем говорит.', portraitEmoji: '👩‍🍳', faction: 'Empath', locationCode: 'cafe' },
  { code: 'stranger', name: 'Незнакомец', description: 'Серые глаза. Появляется там, где не ждёшь.', portraitEmoji: '🕴️', faction: 'Guide', locationCode: 'underpass' },
  { code: 'hunter_kira', name: 'Кира', description: 'Острый взгляд. Охотница на «особенных».', portraitEmoji: '🏹', faction: 'Hunter', locationCode: 'park' },
  { code: 'architect_viktor', name: 'Виктор', description: 'Улыбается редко. Говорит о вероятностях.', portraitEmoji: '🎭', faction: 'Architect', locationCode: 'offices' },
  { code: 'leech_igor', name: 'Игорь', description: 'Бледный, уставший. После встречи — пустота.', portraitEmoji: '🧛', faction: 'Leech', locationCode: 'hospital' },
  { code: 'neighbor', name: 'Сосед Пётр', description: 'Вечно жалуется на шум. Или на тишину.', portraitEmoji: '👴', faction: null, locationCode: 'home' },
  { code: 'librarian', name: 'Елена', description: 'Хранительница книг и тайн.', portraitEmoji: '📖', faction: null, locationCode: 'library' },
];

export const EVIDENCE: EvidenceDef[] = [
  { code: 'note_strange', title: 'Странная записка', description: '«Они видят тебя. Не доверяй зеркалам.»', type: 'Document', chapter: 1 },
  { code: 'photo_shadow', title: 'Фото тени', description: 'На снимке — силуэт без источника света.', type: 'Photo', chapter: 1 },
  { code: 'recording_cafe', title: 'Запись из кафе', description: 'Голос шепчет имя, которого ты не называл.', type: 'Recording', chapter: 1 },
  { code: 'witness_park', title: 'Показания бегуна', description: '«Вчера в парке светился человек без лица.»', type: 'Witness', chapter: 1 },
  { code: 'doc_hospital', title: 'Журнал больницы', description: 'Записи о «истощении без причины» — десятки имён.', type: 'Document', chapter: 2 },
];

export const EVENTS: GameEventDef[] = [
  { code: 'ch1_layoff', title: 'Последний день', description: 'Письмо от HR. Доступ к репозиторию отозван.', category: 'Story', isRandomPool: false, weight: 0, minDay: 1, minChapter: 1, storyOrder: 1, locationCode: 'home',
    choices: chFromJson('ch1_layoff', [
      ['Принять молча', 'Ты закрыл ноутбук. Комната стала тише.', '{"mood":-15,"stress":20}'],
      ['Взорваться', 'Слова, которые нельзя отозвать. Репутация пострадала.', '{"mood":-5,"stress":25,"reputation":-10}'],
      ['Попросить отсрочку', 'Мария пообещала «подумать». Надежда хрупкая.', '{"mood":-8,"stress":15}', { unlocks: 'note_strange' }],
    ]) },
  { code: 'ch1_mother_talk', title: 'Разговор с матерью', description: '«Сынок, мы справимся. Главное — не опускай руки.»', category: 'Social', isRandomPool: false, weight: 0, minDay: 1, minChapter: 1, storyOrder: 2, locationCode: 'home', npcCode: 'mother',
    choices: chFromJson('ch1_mother_talk', [
      ['Обнять её', 'Тепло её рук — единственное, что сегодня реально.', '{"mood":10,"stress":-10}'],
      ['Скрыть правду', 'Улыбнулся. Ложь ради любви тоже боль.', '{"mood":-5,"stress":5}'],
      ['Рассказать всё', 'Она плакала. Ты почувствовал вину и облегчение.', '{"mood":5,"stress":-5}'],
    ]) },
  { code: 'ch1_rental_check', title: 'Осмотр квартиры', description: 'Арендатор приедет завтра. Нужно прибраться.', category: 'Daily', isRandomPool: false, weight: 0, minDay: 1, minChapter: 1, storyOrder: 3, locationCode: 'rental',
    choices: chFromJson('ch1_rental_check', [
      ['Убраться самому', 'Пыль в носу, но +5000 к доходу от аренды скоро.', '{"energy":-20,"money":2000,"mood":-3}'],
      ['Нанять клининг', '−3000, но время сэкономлено.', '{"money":-3000,"energy":5,"mood":5}', { minMoney: 3000 }],
    ]) },
  { code: 'rand_stranger_watch', title: 'Наблюдатель', description: 'На углу стоит человек в сером пальто. Смотрит только на тебя.', category: 'Random', isRandomPool: true, weight: 15, minDay: 1, minChapter: 1, storyOrder: 0, requiredAwareness: 3,
    choices: chFromJson('rand_stranger_watch', [
      ['Подойти', 'Он растворился в толпе. Сердце колотится.', '{"awareness":3,"stress":10}'],
      ['Сделать вид, что не заметил', 'Взгляд обжигал спину до поворота.', '{"awareness":2,"stress":5}'],
      ['Сфотографировать', 'На экране — пустой тротуар.', '{"awareness":5,"intuition":3}', { unlocks: 'photo_shadow' }],
    ]) },
  { code: 'rand_job_call', title: 'Звонок работодателя', description: 'Неизвестный номер: «Мы получили ваше резюме».', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0,
    choices: chFromJson('rand_job_call', [
      ['Ответить сразу', 'Собеседование через три дня. Надежда.', '{"mood":15,"stress":-10,"reputation":5}'],
      ['Перезвонить вечером', 'Гудки. Возможность уплыла.', '{"mood":-5,"stress":10}'],
      ['Игнорировать', 'Страх отказа сильнее желания.', '{"mood":-8,"stress":15}'],
    ]) },
  { code: 'rand_neighbors_fight', title: 'Ссора соседей', description: 'Пётр стучит в стену: «Опять ваш шум!»', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0,
    choices: chFromJson('rand_neighbors_fight', [
      ['Извиниться', 'Затишье. Репутация среди соседей +.', '{"reputation":5,"stress":5}'],
      ['Поспорить', 'Мать вздохнула. Стыдно.', '{"mood":-10,"stress":15,"reputation":-5}'],
    ]) },
  { code: 'rand_lost_wallet', title: 'Потеря кошелька', description: 'Карман пуст. 2000 ₽ исчезли.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0,
    choices: chFromJson('rand_lost_wallet', [
      ['Искать по дороге', 'Напрасно. Усталость.', '{"money":-2000,"energy":-10,"stress":10}'],
      ['Забить', 'Деньги не вернуть. Двигаться дальше.', '{"money":-2000,"willpower":3}'],
    ]) },
  { code: 'rand_lucky_find', title: 'Удачная находка', description: 'На скамейке — конверт с деньгами. Без адреса.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0,
    choices: chFromJson('rand_lucky_find', [
      ['Оставить', 'Честность не кормит, но спит спокойнее.', '{"mood":5,"willpower":5}'],
      ['Взять', '5000 ₽. Чужая удача стала твоей.', '{"money":5000,"mood":10,"awareness":-2}'],
    ]) },
  { code: 'rand_mystic_dream', title: 'Необычный сон', description: 'Город перевёрнут. Люди идут по потолку.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0,
    choices: chFromJson('rand_mystic_dream', [
      ['Записать сон', 'Детали ускользают, но осознанность растёт.', '{"awareness":5,"underworldbond":2}'],
      ['Забыть', 'Утро стёрло всё, кроме тревоги.', '{"stress":8}'],
    ]) },
  { code: 'rand_empath_cafe', title: 'Взгляд бариста', description: 'Вера смотрит так, будто знает твою боль.', category: 'Mystic', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, locationCode: 'cafe', npcCode: 'barista', requiredAwareness: 5,
    choices: chFromJson('rand_empath_cafe', [
      ['Спросить прямо', '«Ты чувствуешь больше, чем кажется. Это нормально.»', '{"awareness":4,"mood":8,"sensitivity":3}'],
      ['Уйти', 'Кофе остыл. Что-то упущено.', '{"mood":-3,"awareness":1}'],
    ]) },
  { code: 'rand_stream_donation', title: 'Донат на стриме', description: 'Аноним перевёл 3000 ₽: «Не сдавайся».', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0,
    choices: chFromJson('rand_stream_donation', [['Забрать', 'Кто-то верит в тебя.', '{"money":3000,"mood":12,"energy":-5}']]) },
  { code: 'rand_bill_shock', title: 'Счёт за коммуналку', description: 'Квитанция выросла на 40%.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0,
    choices: chFromJson('rand_bill_shock', [
      ['Оплатить сразу', '−8000. Спокойствие дорого.', '{"money":-8000,"stress":-5}'],
      ['Отложить', 'Пени. Стресс растёт.', '{"stress":20,"reputation":-3}'],
    ]) },
  { code: 'rand_library_book', title: 'Старая книга', description: 'Елена дала том о «невидимых слоях реальности».', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, locationCode: 'library',
    choices: chFromJson('rand_library_book', [
      ['Читать всю ночь', 'Глаза горят. Осознанность +.', '{"awareness":6,"energy":-25,"intuition":4}'],
      ['Вернуть не прочитав', 'Упущенный шанс.', '{"mood":-2}'],
    ]) },
  { code: 'rand_park_jogger', title: 'Бегун в парке', description: 'Мужчина падает без сил. Вокруг — пустота.', category: 'Mystic', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, locationCode: 'park',
    choices: chFromJson('rand_park_jogger', [
      ['Помочь', 'Скорая увезла его. Ты опустошён.', '{"energy":-15,"awareness":4,"health":-5}'],
      ['Пройти мимо', 'Вина. Сны станут хуже.', '{"stress":15,"willpower":-5}'],
    ]) },
  { code: 'rand_interview_win', title: 'Оффер', description: 'Джун-позиция. Зарплата скромная, но стабильная.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0,
    choices: chFromJson('rand_interview_win', [['Принять', 'Новая глава.', '{"mood":20,"stress":-15,"money":5000,"reputation":10}']]) },
  { code: 'rand_underpass_whisper', title: 'Шёпот в переходе', description: 'Имя из темноты. Твоё. Но голос чужой.', category: 'Underworld', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, locationCode: 'underpass', requiredAwareness: 8,
    choices: chFromJson('rand_underpass_whisper', [
      ['Ответить', 'Эхо. Связь с Изнанкой +.', '{"underworldbond":8,"awareness":5,"stress":10}'],
      ['Бежать', 'Сердце. Выжил.', '{"stress":20,"energy":-10}'],
    ]) },
  { code: 'rand_factory_light', title: 'Свет на заводе', description: 'В окне вспыхнул огонь. Здание заброшено лет десять.', category: 'Mystic', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, locationCode: 'factory', requiredAwareness: 12,
    choices: chFromJson('rand_factory_light', [
      ['Исследовать', 'Холод. След на стене — как сожжённая рука.', '{"awareness":8,"stress":15}', { unlocks: 'witness_park' }],
      ['Вызвать полицию', 'Приехали. Ничего не нашли.', '{"reputation":3}'],
    ]) },
  { code: 'rand_hospital_noise', title: 'Звук из больницы', description: 'Скрежет металла. Окна заколочены.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, locationCode: 'hospital', requiredAwareness: 15,
    choices: chFromJson('rand_hospital_noise', [
      ['Подойти к воротам', 'Игорь смотрит из темноты. Улыбается.', '{"awareness":6,"health":-10,"stress":15}'],
      ['Уйти', 'Разум говорит «правильно». Интуиция — «трус».', '{"willpower":-3,"intuition":2}'],
    ]) },
  { code: 'rand_cemetery_fog', title: 'Туман на кладбище', description: 'Надгробия тонут в сером.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, locationCode: 'cemetery', requiredAwareness: 8,
    choices: chFromJson('rand_cemetery_fog', [
      ['Остаться до рассвета', 'Видение: город как нервная система.', '{"awareness":10,"underworldbond":5,"energy":-30}'],
      ['Уйти до темноты', 'Безопасность.', '{"stress":-5}'],
    ]) },
  { code: 'rand_mother_pills', title: 'Лекарства матери', description: 'Нужно купить рецептурные препараты.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, npcCode: 'mother',
    choices: chFromJson('rand_mother_pills', [
      ['Купить', '−4000. Мать благодарна.', '{"money":-4000,"mood":10,"stress":-8}'],
      ['Отложить', 'Вина съедает.', '{"mood":-12,"stress":20}'],
    ]) },
  { code: 'rand_freelance_rush', title: 'Срочный заказ', description: 'Клиент платит двойную ставку за ночь.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0,
    choices: chFromJson('rand_freelance_rush', [['Взяться', 'Ночь без сна.', '{"money":8000,"energy":-35,"stress":15}']]) },
  { code: 'rand_viktor_offer', title: 'Предложение Виктора', description: '«Я могу изменить твою вероятность успеха. Цена — услуга».', category: 'Mystic', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, npcCode: 'architect_viktor', requiredAwareness: 20,
    choices: chFromJson('rand_viktor_offer', [
      ['Согласиться', 'Мир сдвинулся. Деньги пришли сами.', '{"money":15000,"awareness":8,"willpower":-10,"underworldbond":10}'],
      ['Отказать', 'Виктор кивнул. «Ты ещё вернёшься».', '{"willpower":10,"awareness":3}'],
    ]) },
  { code: 'rand_kira_warning', title: 'Предупреждение Киры', description: '«Они уже нюхают тебя. Не доверяй улыбкам».', category: 'Mystic', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, npcCode: 'hunter_kira',
    choices: chFromJson('rand_kira_warning', [
      ['Спросить кто «они»', 'Охотники, Архитекторы, Пиявки. Выбери врага.', '{"awareness":5,"intuition":5}'],
      ['Сказать, что не веришь', 'Кира усмехнулась. «Пока».', '{"reputation":0}'],
    ]) },
  { code: 'rand_guide_meeting', title: 'Встреча с Проводником', description: 'Незнакомец: «Ты видишь. Это редкость».', category: 'Underworld', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, npcCode: 'stranger', requiredAwareness: 10,
    choices: chFromJson('rand_guide_meeting', [
      ['Слушать', 'Он рассказал об Изнанке. Мир расширился.', '{"awareness":8,"underworldbond":12,"sensitivity":5}'],
      ['Сбежать', 'Рациональность — щит. Пока.', '{"stress":10}'],
    ]) },
  { code: 'rand_mall_collapse', title: 'Оборванный лифт', description: 'Толпа. Крик. Ты помог ребёнку выбраться.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, locationCode: 'mall',
    choices: chFromJson('rand_mall_collapse', [['Помочь', 'Герой на один день.', '{"reputation":15,"health":-5,"mood":10}']]) },
  { code: 'rand_newdistrict_ad', title: 'Реклама новостроек', description: '«Квартира мечты». Цены не для тебя.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, locationCode: 'newdistrict',
    choices: chFromJson('rand_newdistrict_ad', [
      ['Мечтать', 'Надежда и боль.', '{"mood":-5,"stress":5}'],
      ['Записать идею стрима', 'Контент о «ложных обещаниях».', '{"mood":5,"money":500}'],
    ]) },
  { code: 'rand_stress_breakdown', title: 'Срыв', description: 'Ты не можешь встать с кровати.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0,
    choices: chFromJson('rand_stress_breakdown', [['Лежать', 'Мир подождёт.', '{"mood":-20,"energy":-20,"stress":25,"health":-5}']]) },
  { code: 'rand_good_sleep', title: 'Крепкий сон', description: 'Впервые за неделю — восемь часов.', category: 'Random', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0,
    choices: chFromJson('rand_good_sleep', [['Отдохнуть', 'Силы возвращаются.', '{"energy":30,"health":10,"stress":-15,"mood":10}']]) },
  { code: 'rand_underworld_glimpse', title: 'Проблеск Изнанки', description: 'На миг мир стал прозрачным. Эмоции — цветные нити.', category: 'Underworld', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, requiredAwareness: 20,
    choices: chFromJson('rand_underworld_glimpse', [
      ['Войти', 'Холод. Красота. Страх.', '{"underworldbond":15,"awareness":10,"sensitivity":8}', { minWill: 40 }],
      ['Вернуться', 'Земля. Облегчение.', '{"willpower":5}'],
    ]) },
  { code: 'rand_leech_drain', title: 'Встреча с Игорем', description: 'После разговора — будто неделю не спал.', category: 'Mystic', isRandomPool: true, weight: 10, minDay: 1, minChapter: 1, storyOrder: 0, npcCode: 'leech_igor', requiredAwareness: 12,
    choices: chFromJson('rand_leech_drain', [
      ['Противостоять', 'Воля спасла. Он отступил.', '{"willpower":8,"energy":-20,"influenceresistance":5}'],
      ['Поддаться', 'Пустота. Деньги на лечение.', '{"energy":-40,"health":-15,"money":-3000}'],
    ]) },
];

export const eventByCode = new Map(EVENTS.map((e) => [e.code, e]));
export const locationByCode = new Map(LOCATIONS.map((l) => [l.code, l]));
