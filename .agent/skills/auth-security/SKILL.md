---
name: auth-security
description: Правила авторизации, проверки полномочий и защиты данных в приложении CineNote. Охватывает разграничение прав доступа (Владелец / Редактор / Читатель), использование AuthenticationStateProvider, извлечение идентификатора пользователя из Claims и защиту эндпоинтов.
---

# Стандарты авторизации и информационной безопасности в CineNote

Данный документ регламентирует правила защиты данных, проверки прав доступа и предотвращения несанкционированных действий в веб-приложении CineNote.

---

## 1. Защита маршрутов и компонентов ([Authorize] и AuthenticationStateProvider)

- **Декларативная защита страниц**:
  - Все страницы рабочей области и личного кабинета, требующие авторизации, обязаны содержать атрибут `@attribute [Authorize]`:
    ```razor
    @page "/projects"
    @attribute [Authorize]
    ```
  - Неавторизованные пользователи должны автоматически перенаправляться на страницу входа (`/login`) через компонент `RedirectToLogin` или `Routes.razor`.

- **Получение состояния пользователя через AuthenticationStateProvider**:
  - Состояние аутентификации получается только через каскадный параметр `Task<AuthenticationState>` либо внедренный сервис `AuthService` / `AuthenticationStateProvider`:
    ```csharp
    [CascadingParameter]
    private Task<AuthenticationState> AuthStateTask { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateTask;
        var user = authState.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            Navigation.NavigateTo("/login");
            return;
        }
    }
    ```

---

## 2. Безопасное извлечение идентификатора пользователя (Claims)

- **Категорический запрет доверия клиентским параметрам пользователя**:
  - Никогда не принимать идентификатор текущего пользователя (`currentUserId`) из открытых query-параметров URL, скрытых полей форм или заголовков запросов без сверки с сессией.
  - Идентификатор пользователя извлекается **строго** из клеймов аутентифицированного принципала (`ClaimsPrincipal`):
    ```csharp
    // ПРАВИЛЬНО:
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (!int.TryParse(userIdClaim, out int currentUserId) || currentUserId <= 0)
    {
        // Отказ в доступе
    }

    // ИЛИ через проверенный сервис AuthService, работающий с защищенными Claims:
    var currentUserId = AuthService.CurrentUser?.Id ?? 0;
    ```

---

## 3. Серверная проверка прав перед любой модификацией данных

- **Проверка полномочий на уровне сервисов (Defense in Depth)**:
  - Скрытие кнопки в пользовательском интерфейсе (UI) — это исключительно элемент UX, но **не гарантия безопасности**.
  - Любой серверный метод, модифицирующий данные (`Create`, `Update`, `Delete`, `Move`, `Reorder`), обязан принимать `currentUserId` и валидировать права доступа перед выполнением транзакции к базе данных.

- **Разграничение ролей в кинопроектах**:
  - **Удаление проекта (`DeleteProjectAsync`)**:
    - Разрешено **только создателю (Владельцу)**:
      ```csharp
      var project = await context.MovieProjects.FirstOrDefaultAsync(p => p.Id == projectId);
      if (project == null || project.UserId != currentUserId)
      {
          return false; // Доступ запрещен: соавторы не могут удалить чужой проект
      }
      ```
  - **Редактирование проекта, сценария, сцен и персонажей**:
    - Разрешено Владельцу и соавторам с ролью **«Редактор»**:
      ```csharp
      bool canEdit = await ShareService.CanUserEditProjectAsync(projectId, currentUserId);
      if (!canEdit)
      {
          return false; // Читатель не имеет права вносить изменения
      }
      ```
  - **Чтение и просмотр**:
    - Разрешено Владельцу, Редактору и Читателю:
      ```csharp
      bool canView = await ShareService.CanUserViewProjectAsync(projectId, currentUserId);
      if (!canView)
      {
          return null; // Чужой проект недоступен для просмотра
      }
      ```

---

## 4. Защита паролей и учетных данных

- **Хеширование паролей**:
  - Пароли никогда не сохраняются в базе данных в открытом виде.
  - Применяется надежный криптографический алгоритм хеширования с солью (`BCrypt` / `PBKDF2` / `ASP.NET Core PasswordHasher`).

- **Конфиденциальность в UI**:
  - Поля ввода паролей по умолчанию имеют тип `type="password"`.
  - Кнопка показа/скрытия пароля реализуется через изменение типа ввода и снабжается иконкой глазика с корректным состоянием доступности (`aria-label`).
