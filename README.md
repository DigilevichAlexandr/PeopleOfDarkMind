# People of Dark Mind

Текстовая RPG с элементами мистики, симулятора жизни и расследования.

## Стек

- **Backend:** ASP.NET Core 9, EF Core, PostgreSQL, Clean Architecture, CQRS (MediatR)
- **Frontend:** React, TypeScript, Vite, Tailwind CSS
- **Инфраструктура:** Docker Compose

## Быстрый старт

### Docker

```bash
docker compose up --build
```

- Frontend: http://localhost:5173
- API: http://localhost:5000

### Локально

1. PostgreSQL на порту 5432 (см. `appsettings.json`)
2. Backend:

```bash
cd backend
dotnet run --project src/PeopleOfDarkMind.API
```

3. Frontend:

```bash
cd frontend
npm install
npm run dev
```

## Игровые системы (начальная версия)

- День / утро / день / вечер / ночь — одно действие за период
- Открытые и скрытые характеристики
- 13 локаций города + Изнанка
- 10 NPC с отношениями
- 30 событий, глава 1 «Падение»
- Доска расследования, журнал
- JWT-авторизация

## GitHub Pages (браузерная версия)

Игра работает **полностью в браузере** — без сервера, сохранение в `localStorage`.

После push в `master` GitHub Actions деплоит фронтенд:

**https://digilevichalexandr.github.io/PeopleOfDarkMind/**

Локальная сборка для Pages:

```bash
cd frontend
set GITHUB_PAGES=true   # Windows
npm run build
```

## Публикация backend (Render)

1. Форкните репозиторий или подключите свой на [Render](https://render.com).
2. **New → Blueprint** → укажите URL репозитория с файлом `render.yaml`.
3. Render создаст PostgreSQL, API и фронтенд. Публичный URL будет у сервиса `podm-frontend`.

Кнопка быстрого деплоя:

[![Deploy to Render](https://render.com/images/deploy-to-render-button.svg)](https://render.com/deploy?repo=https://github.com/DigilevichAlexandr/PeopleOfDarkMind)

## Структура

```
backend/
  src/PeopleOfDarkMind.Domain
  src/PeopleOfDarkMind.Application
  src/PeopleOfDarkMind.Infrastructure
  src/PeopleOfDarkMind.API
frontend/
docker-compose.yml
render.yaml
```
