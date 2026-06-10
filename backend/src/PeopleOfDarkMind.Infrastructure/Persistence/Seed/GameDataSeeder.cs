using PeopleOfDarkMind.Domain.Entities;
using PeopleOfDarkMind.Domain.Enums;

namespace PeopleOfDarkMind.Infrastructure.Persistence.Seed;

public static class GameDataSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Locations.Any()) return;

        var locations = SeedLocations();
        db.Locations.AddRange(locations);
        db.SaveChanges();

        var npcs = SeedNpcs(locations);
        db.Npcs.AddRange(npcs);
        db.SaveChanges();

        db.StoryChapters.AddRange(SeedChapters());
        db.EvidenceItems.AddRange(SeedEvidence());
        db.SaveChanges();

        var events = SeedEvents(locations, npcs);
        db.GameEvents.AddRange(events);
        db.SaveChanges();
    }

    private static List<Location> SeedLocations() =>
    [
        Loc("home", "Квартира героя", "Тесная двушка с матерью. Запах кофе и тишина.", "Центр", false, 2, 3, "🏠"),
        Loc("rental", "Квартира для сдачи", "Наследство деда. Скрипит паркет, пахнет пылью.", "Север", false, 1, 1, "🔑"),
        Loc("park", "Парк", "Каштаны, скамейки, бегуны. Здесь легче дышать.", "Центр", false, 4, 5, "🌳"),
        Loc("library", "Библиотека", "Тишина и пыльные тома. Бесплатный Wi‑Fi.", "Центр", false, 5, 2, "📚"),
        Loc("cafe", "Кафе «Сумерки»", "Тусклый свет, джаз, разговоры за соседним столом.", "Центр", false, 6, 4, "☕"),
        Loc("mall", "Торговый центр", "Неон, толпы, витрины. Город в миниатюре.", "Восток", false, 8, 3, "🛍️"),
        Loc("offices", "Офисный квартал", "Стекло и костюмы. Здесь решают судьбы за зарплату.", "Запад", false, 1, 5, "🏢"),
        Loc("factory", "Заброшенный завод", "Ржавчина, эхо шагов, чужие граффити.", "Юг", true, 3, 8, "🏭", awareness: 10),
        Loc("underpass", "Подземный переход", "Сырость, неон, лица без имён.", "Центр", true, 5, 6, "🚇", awareness: 5),
        Loc("hospital", "Старая больница", "Заколоченные окна. Говорят, слышны крики.", "Север", true, 2, 7, "🏥", awareness: 15),
        Loc("cemetery", "Кладбище", "Мрамор, туман, имена забытых.", "Север", true, 1, 9, "⚰️", awareness: 8),
        Loc("newdistrict", "Район новостроек", "Бетон, краны, обещания лучшей жизни.", "Восток", false, 9, 1, "🏗️"),
        Loc("underworld", "Изнанка", "Город перевёрнут. Эмоции — как туман.", "—", true, 5, 5, "🌑", awareness: 25, requires: true)
    ];

    private static Location Loc(string code, string name, string desc, string district,
        bool mystic, int x, int y, string icon, int awareness = 0, bool requires = false) =>
        new()
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            Description = desc,
            District = district,
            IsMystic = mystic,
            RequiresAwareness = requires,
            MinAwareness = awareness,
            MapX = x,
            MapY = y,
            Icon = icon
        };

    private static List<Npc> SeedNpcs(List<Location> locations)
    {
        Guid LocId(string code) => locations.First(l => l.Code == code).Id;
        return
        [
            Npc("mother", "Мать", "Устала, но держится. Верит, что всё наладится.", "Заботится о сыне", LocId("home"), "👩"),
            Npc("landlord", "Арендатор", "Молодой IT-специалист. Платит вовремя.", "Ищет тишины", LocId("rental"), "🧑‍💻"),
            Npc("hr_maria", "Мария (HR)", "Улыбка как маска. Знает больше, чем говорит.", "Оценивает кандидатов", LocId("offices"), "👩‍💼"),
            Npc("barista", "Вера", "Бариста в «Сумерках». Слушает больше, чем говорит.", "Скрывает способности", LocId("cafe"), "👩‍🍳", FactionType.Empath, true, true),
            Npc("stranger", "Незнакомец", "Серые глаза. Появляется там, где не ждёшь.", "Наблюдает", LocId("underpass"), "🕴️", FactionType.Guide, true, true),
            Npc("hunter_kira", "Кира", "Острый взгляд. Охотница на «особенных».", "Ищет добычу", LocId("park"), "🏹", FactionType.Hunter, true, true),
            Npc("architect_viktor", "Виктор", "Улыбается редко. Говорит о вероятностях.", "Манипулирует судьбами", LocId("offices"), "🎭", FactionType.Architect, true, true),
            Npc("leech_igor", "Игорь", "Бледный, уставший. После встречи — пустота.", "Питается энергией", LocId("hospital"), "🧛", FactionType.Leech, true, true),
            Npc("neighbor", "Сосед Пётр", "Вечно жалуется на шум. Или на тишину.", "Завидует чужой жизни", LocId("home"), "👴"),
            Npc("librarian", "Елена", "Хранительница книг и тайн.", "Знает старые истории города", LocId("library"), "📖")
        ];
    }

    private static Npc Npc(string code, string name, string desc, string motives, Guid locId, string emoji,
        FactionType faction = FactionType.None, bool supernatural = false, bool hides = false) => new()
    {
        Id = Guid.NewGuid(),
        Code = code,
        Name = name,
        Description = desc,
        HiddenMotives = motives,
        DefaultLocationId = locId,
        PortraitEmoji = emoji,
        Faction = faction,
        IsSupernatural = supernatural,
        HidesTrueNature = hides
    };

    private static List<StoryChapter> SeedChapters() =>
    [
        new() { Number = 1, Title = "Падение", Description = "Потеря работы и поиск себя",
            IntroText = "Три месяца без зарплаты. Город не ждёт.", RequiredProgress = 5 },
        new() { Number = 2, Title = "Пробуждение", Description = "Первые признаки Изнанки",
            IntroText = "Мир дрогнул. Ты видишь то, чего не должно быть.", RequiredProgress = 8 }
    ];

    private static List<Evidence> SeedEvidence() =>
    [
        Ev("note_strange", "Странная записка", "«Они видят тебя. Не доверяй зеркалам.»", EvidenceType.Document, 1),
        Ev("photo_shadow", "Фото тени", "На снимке — силуэт без источника света.", EvidenceType.Photo, 1),
        Ev("recording_cafe", "Запись из кафе", "Голос шепчет имя, которого ты не называл.", EvidenceType.Recording, 1),
        Ev("witness_park", "Показания бегуна", "«Вчера в парке светился человек без лица.»", EvidenceType.Witness, 1),
        Ev("doc_hospital", "Журнал больницы", "Записи о «истощении без причины» — десятки имён.", EvidenceType.Document, 2)
    ];

    private static Evidence Ev(string code, string title, string desc, EvidenceType type, int chapter) => new()
    {
        Id = Guid.NewGuid(),
        Code = code,
        Title = title,
        Description = desc,
        Type = type,
        Chapter = chapter
    };

    private static List<GameEvent> SeedEvents(List<Location> locations, List<Npc> npcs)
    {
        Guid L(string c) => locations.First(x => x.Code == c).Id;
        Guid N(string c) => npcs.First(x => x.Code == c).Id;

        return
        [
            Story("ch1_layoff", "Последний день", "Письмо от HR. Доступ к репозиторию отозван.",
                EventCategory.Story, 1, L("home"),
                [
                    Ch("Принять молча", "Ты закрыл ноутбук. Комната стала тише.", """{"mood":-15,"stress":20}"""),
                    Ch("Взорваться", "Слова, которые нельзя отозвать. Репутация пострадала.", """{"mood":-5,"stress":25,"reputation":-10}"""),
                    Ch("Попросить отсрочку", "Мария пообещала «подумать». Надежда хрупкая.", """{"mood":-8,"stress":15}""", unlocks: "note_strange")
                ], order: 1),

            Story("ch1_mother_talk", "Разговор с матерью", "«Сынок, мы справимся. Главное — не опускай руки.»",
                EventCategory.Social, 1, L("home"),
                [
                    Ch("Обнять её", "Тепло её рук — единственное, что сегодня реально.", """{"mood":10,"stress":-10}"""),
                    Ch("Скрыть правду", "Улыбнулся. Ложь ради любви тоже боль.", """{"mood":-5,"stress":5}"""),
                    Ch("Рассказать всё", "Она плакала. Ты почувствовал вину и облегчение.", """{"mood":5,"stress":-5}""")
                ], order: 2, npc: N("mother")),

            Story("ch1_rental_check", "Осмотр квартиры", "Арендатор приедет завтра. Нужно прибраться.",
                EventCategory.Daily, 1, L("rental"),
                [
                    Ch("Убраться самому", "Пыль в носу, но +5000 к доходу от аренды скоро.", """{"energy":-20,"money":2000,"mood":-3}"""),
                    Ch("Нанять клининг", "−3000, но время сэкономлено.", """{"money":-3000,"energy":5,"mood":5}""", minMoney: 3000)
                ], order: 3),

            Rand("rand_stranger_watch", "Наблюдатель", "На углу стоит человек в сером пальто. Смотрит только на тебя.",
                [
                    Ch("Подойти", "Он растворился в толпе. Сердце колотится.", """{"awareness":3,"stress":10}"""),
                    Ch("Сделать вид, что не заметил", "Взгляд обжигал спину до поворота.", """{"awareness":2,"stress":5}"""),
                    Ch("Сфотографировать", "На экране — пустой тротуар.", """{"awareness":5,"intuition":3}""", unlocks: "photo_shadow")
                ], weight: 15, awareness: 3),

            Rand("rand_job_call", "Звонок работодателя", "Неизвестный номер: «Мы получили ваше резюме».",
                [
                    Ch("Ответить сразу", "Собеседование через три дня. Надежда.", """{"mood":15,"stress":-10,"reputation":5}"""),
                    Ch("Перезвонить вечером", "Гудки. Возможность уплыла.", """{"mood":-5,"stress":10}"""),
                    Ch("Игнорировать", "Страх отказа сильнее желания.", """{"mood":-8,"stress":15}""")
                ]),

            Rand("rand_neighbors_fight", "Ссора соседей", "Пётр стучит в стену: «Опять ваш шум!»",
                [
                    Ch("Извиниться", "Затишье. Репутация среди соседей +.", """{"reputation":5,"stress":5}"""),
                    Ch("Поспорить", "Мать вздохнула. Стыдно.", """{"mood":-10,"stress":15,"reputation":-5}""")
                ]),

            Rand("rand_lost_wallet", "Потеря кошелька", "Карман пуст. 2000 ₽ исчезли.",
                [
                    Ch("Искать по дороге", "Напрасно. Усталость.", """{"money":-2000,"energy":-10,"stress":10}"""),
                    Ch("Забить", "Деньги не вернуть. Двигаться дальше.", """{"money":-2000,"willpower":3}""")
                ]),

            Rand("rand_lucky_find", "Удачная находка", "На скамейке — конверт с деньгами. Без адреса.",
                [
                    Ch("Оставить", "Честность не кормит, но спит спокойнее.", """{"mood":5,"willpower":5}"""),
                    Ch("Взять", "5000 ₽. Чужая удача стала твоей.", """{"money":5000,"mood":10,"awareness":-2}""")
                ]),

            Rand("rand_mystic_dream", "Необычный сон", "Город перевёрнут. Люди идут по потолку.",
                [
                    Ch("Записать сон", "Детали ускользают, но осознанность растёт.", """{"awareness":5,"underworldbond":2}"""),
                    Ch("Забыть", "Утро стёрло всё, кроме тревоги.", """{"stress":8}""")
                ]),

            Rand("rand_empath_cafe", "Взгляд бариста", "Вера смотрит так, будто знает твою боль.",
                [
                    Ch("Спросить прямо", "«Ты чувствуешь больше, чем кажется. Это нормально.»", """{"awareness":4,"mood":8,"sensitivity":3}"""),
                    Ch("Уйти", "Кофе остыл. Что-то упущено.", """{"mood":-3,"awareness":1}""")
                ], category: EventCategory.Mystic, awareness: 5, location: L("cafe"), npc: N("barista")),

            Rand("rand_stream_donation", "Донат на стриме", "Аноним перевёл 3000 ₽: «Не сдавайся».",
                [Ch("Забрать", "Кто-то верит в тебя.", """{"money":3000,"mood":12,"energy":-5}""")]),

            Rand("rand_bill_shock", "Счёт за коммуналку", "Квитанция выросла на 40%.",
                [
                    Ch("Оплатить сразу", "−8000. Спокойствие дорого.", """{"money":-8000,"stress":-5}"""),
                    Ch("Отложить", "Пени. Стресс растёт.", """{"stress":20,"reputation":-3}""")
                ]),

            Rand("rand_library_book", "Старая книга", "Елена дала том о «невидимых слоях реальности».",
                [
                    Ch("Читать всю ночь", "Глаза горят. Осознанность +.", """{"awareness":6,"energy":-25,"intuition":4}"""),
                    Ch("Вернуть не прочитав", "Упущенный шанс.", """{"mood":-2}""")
                ], location: L("library")),

            Rand("rand_park_jogger", "Бегун в парке", "Мужчина падает без сил. Вокруг — пустота.",
                [
                    Ch("Помочь", "Скорая увезла его. Ты опустошён.", """{"energy":-15,"awareness":4,"health":-5}"""),
                    Ch("Пройти мимо", "Вина. Сны станут хуже.", """{"stress":15,"willpower":-5}""")
                ], category: EventCategory.Mystic, location: L("park")),

            Rand("rand_interview_win", "Оффер", "Джун-позиция. Зарплата скромная, но стабильная.",
                [Ch("Принять", "Новая глава.", """{"mood":20,"stress":-15,"money":5000,"reputation":10}""")]),

            Rand("rand_underpass_whisper", "Шёпот в переходе", "Имя из темноты. Твоё. Но голос чужой.",
                [
                    Ch("Ответить", "Эхо. Связь с Изнанкой +.", """{"underworldbond":8,"awareness":5,"stress":10}"""),
                    Ch("Бежать", "Сердце. Выжил.", """{"stress":20,"energy":-10}""")
                ], category: EventCategory.Underworld, awareness: 8, location: L("underpass")),

            Rand("rand_factory_light", "Свет на заводе", "В окне вспыхнул огонь. Здание заброшено лет десять.",
                [
                    Ch("Исследовать", "Холод. След на стене — как сожжённая рука.", """{"awareness":8,"stress":15}""", unlocks: "witness_park"),
                    Ch("Вызвать полицию", "Приехали. Ничего не нашли.", """{"reputation":3}""")
                ], category: EventCategory.Mystic, awareness: 12, location: L("factory")),

            Rand("rand_hospital_noise", "Звук из больницы", "Скрежет металла. Окна заколочены.",
                [
                    Ch("Подойти к воротам", "Игорь смотрит из темноты. Улыбается.", """{"awareness":6,"health":-10,"stress":15}"""),
                    Ch("Уйти", "Разум говорит «правильно». Интуиция — «трус».", """{"willpower":-3,"intuition":2}""")
                ], awareness: 15, location: L("hospital")),

            Rand("rand_cemetery_fog", "Туман на кладбище", "Надгробия тонут в сером.",
                [
                    Ch("Остаться до рассвета", "Видение: город как нервная система.", """{"awareness":10,"underworldbond":5,"energy":-30}"""),
                    Ch("Уйти до темноты", "Безопасность.", """{"stress":-5}""")
                ], awareness: 8, location: L("cemetery")),

            Rand("rand_mother_pills", "Лекарства матери", "Нужно купить рецептурные препараты.",
                [
                    Ch("Купить", "−4000. Мать благодарна.", """{"money":-4000,"mood":10,"stress":-8}"""),
                    Ch("Отложить", "Вина съедает.", """{"mood":-12,"stress":20}""")
                ], npc: N("mother")),

            Rand("rand_freelance_rush", "Срочный заказ", "Клиент платит двойную ставку за ночь.",
                [Ch("Взяться", "Ночь без сна.", """{"money":8000,"energy":-35,"stress":15}""")]),

            Rand("rand_viktor_offer", "Предложение Виктора", "«Я могу изменить твою вероятность успеха. Цена — услуга».",
                [
                    Ch("Согласиться", "Мир сдвинулся. Деньги пришли сами.", """{"money":15000,"awareness":8,"willpower":-10,"underworldbond":10}"""),
                    Ch("Отказать", "Виктор кивнул. «Ты ещё вернёшься».", """{"willpower":10,"awareness":3}""")
                ], category: EventCategory.Mystic, awareness: 20, npc: N("architect_viktor")),

            Rand("rand_kira_warning", "Предупреждение Киры", "«Они уже нюхают тебя. Не доверяй улыбкам».",
                [
                    Ch("Спросить кто «они»", "Охотники, Архитекторы, Пиявки. Выбери врага.", """{"awareness":5,"intuition":5}"""),
                    Ch("Сказать, что не веришь", "Кира усмехнулась. «Пока».", """{"reputation":0}""")
                ], category: EventCategory.Mystic, npc: N("hunter_kira")),

            Rand("rand_guide_meeting", "Встреча с Проводником", "Незнакомец: «Ты видишь. Это редкость».",
                [
                    Ch("Слушать", "Он рассказал об Изнанке. Мир расширился.", """{"awareness":8,"underworldbond":12,"sensitivity":5}"""),
                    Ch("Сбежать", "Рациональность — щит. Пока.", """{"stress":10}""")
                ], category: EventCategory.Underworld, awareness: 10, npc: N("stranger")),

            Rand("rand_mall_collapse", "Оборванный лифт", "Толпа. Крик. Ты помог ребёнку выбраться.",
                [Ch("Помочь", "Герой на один день.", """{"reputation":15,"health":-5,"mood":10}""")], location: L("mall")),

            Rand("rand_newdistrict_ad", "Реклама новостроек", "«Квартира мечты». Цены не для тебя.",
                [
                    Ch("Мечтать", "Надежда и боль.", """{"mood":-5,"stress":5}"""),
                    Ch("Записать идею стрима", "Контент о «ложных обещаниях».", """{"mood":5,"money":500}""")
                ], location: L("newdistrict")),

            Rand("rand_stress_breakdown", "Срыв", "Ты не можешь встать с кровати.",
                [Ch("Лежать", "Мир подождёт.", """{"mood":-20,"energy":-20,"stress":25,"health":-5}""")]),

            Rand("rand_good_sleep", "Крепкий сон", "Впервые за неделю — восемь часов.",
                [Ch("Отдохнуть", "Силы возвращаются.", """{"energy":30,"health":10,"stress":-15,"mood":10}""")]),

            Rand("rand_underworld_glimpse", "Проблеск Изнанки", "На миг мир стал прозрачным. Эмоции — цветные нити.",
                [
                    Ch("Войти", "Холод. Красота. Страх.", """{"underworldbond":15,"awareness":10,"sensitivity":8}""", minWill: 40),
                    Ch("Вернуться", "Земля. Облегчение.", """{"willpower":5}""")
                ], category: EventCategory.Underworld, awareness: 20),

            Rand("rand_leech_drain", "Встреча с Игорем", "После разговора — будто неделю не спал.",
                [
                    Ch("Противостоять", "Воля спасла. Он отступил.", """{"willpower":8,"energy":-20,"influenceresistance":5}"""),
                    Ch("Поддаться", "Пустота. Деньги на лечение.", """{"energy":-40,"health":-15,"money":-3000}""")
                ], category: EventCategory.Mystic, awareness: 12, npc: N("leech_igor"))
        ];
    }

    private sealed record ChoiceDef(string Text, string Outcome, string Effects, string? Unlocks = null, int? MinMoney = null, int? MinWill = null);

    private static ChoiceDef Ch(string text, string outcome, string effects, string? unlocks = null, int? minMoney = null, int? minWill = null) =>
        new(text, outcome, effects, unlocks, minMoney, minWill);

    private static GameEvent Story(string code, string title, string desc, EventCategory cat, int chapter,
        Guid locationId, ChoiceDef[] choices, int order = 0, Guid? npc = null)
    {
        var ev = BaseEvent(code, title, desc, cat, false, 0, chapter, locationId, npc, order);
        ev.Choices = choices.Select((c, i) => Choice(ev.Id, i, c.Text, c.Outcome, c.Effects, c.Unlocks, c.MinMoney, c.MinWill)).ToList();
        return ev;
    }

    private static GameEvent Rand(string code, string title, string desc, ChoiceDef[] choices,
        int weight = 10, EventCategory category = EventCategory.Random,
        int? awareness = null, Guid? location = null, Guid? npc = null)
    {
        var ev = BaseEvent(code, title, desc, category, true, weight, 1, location, npc, 0, awareness);
        ev.Choices = choices.Select((c, i) => Choice(ev.Id, i, c.Text, c.Outcome, c.Effects, c.Unlocks, c.MinMoney, c.MinWill)).ToList();
        return ev;
    }

    private static GameEvent BaseEvent(string code, string title, string desc, EventCategory cat,
        bool random, int weight, int chapter, Guid? locationId, Guid? npcId, int order, int? awareness = null) => new()
    {
        Id = Guid.NewGuid(),
        Code = code,
        Title = title,
        Description = desc,
        Category = cat,
        MinDay = 1,
        MinChapter = chapter,
        LocationId = locationId,
        NpcId = npcId,
        IsRandomPool = random,
        Weight = weight,
        StoryOrder = order,
        RequiredAwareness = awareness
    };

    private static EventChoice Choice(Guid eventId, int order, string text, string outcome, string effects,
        string? unlocks = null, int? minMoney = null, int? minWill = null) => new()
    {
        Id = Guid.NewGuid(),
        EventId = eventId,
        Text = text,
        OutcomeText = outcome,
        SortOrder = order,
        EffectsJson = effects,
        UnlocksEvidenceCode = unlocks,
        MinMoney = minMoney,
        MinWillpower = minWill
    };
}
