---
name: sqlserver-efcore
description: Стандарты работы с базой данных Microsoft SQL Server и Entity Framework Core в CineNote. Охватывает управление контекстом через IDbContextFactory, предотвращение проблемы N+1, оптимизацию AsNoTracking и правила миграций.
---

# Стандарты работы с SQL Server и Entity Framework Core в CineNote

Данный документ содержит архитектурные правила и практические рекомендации по проектированию сущностей, выполнению запросов и обеспечению производительности базы данных в CineNote.

---

## 1. Управление контекстом базы данных через IDbContextFactory

- **Отказ от прямого внедрения Scoped DbContext в компоненты**:
  - В Blazor Interactive Server один экземпляр компонента может жить дольше обычного HTTP-запроса, а параллельные вызовы из SignalR к одному экземпляру `DbContext` приводят к исключению `InvalidOperationException: A second operation was started on this context instance before a previous operation completed`.
  - Все серверные сервисы и репозитории обязаны внедрять `IDbContextFactory<ApplicationDbContext>`:
    ```csharp
    public class SceneService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SceneService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Scene>> GetScenesByProjectIdAsync(int projectId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Scenes
                .AsNoTracking()
                .Where(s => s.MovieProjectId == projectId)
                .OrderBy(s => s.SceneNumber)
                .ToListAsync();
        }
    }
    ```

- **Паттерн await using**:
  - Каждый метод сервиса создает легковесный экземпляр контекста на время выполнения конкретной операции через `await using var context = await _contextFactory.CreateDbContextAsync();`, гарантируя мгновенное закрытие подключения к пулу соединений SQL Server.

---

## 2. Оптимизация операций чтения (.AsNoTracking)

- **Обязательное использование для запросов выборки**:
  - Любой запрос, данные которого предназначены только для отображения в UI, сериализации или вычислений, обязан завершаться `.AsNoTracking()` перед материализацией (`ToListAsync`, `FirstOrDefaultAsync`, `CountAsync`).
  - `.AsNoTracking()` отключает накладные расходы на Change Tracker EF Core, снижает потребление оперативной памяти и ускоряет выполнение запросов на 30–50%.

- **Исключения**:
  - Контекстный трекинг допустим только в сценариях, где сущность загружается для немедленной модификации и последующего вызова `await context.SaveChangesAsync()`.

---

## 3. Защита от проблемы N+1 запросов

- **Явная жадная загрузка связанных данных (.Include / .ThenInclude)**:
  - Категорически запрещено итерироваться по сущностям в цикле с последующим отдельным обращением к БД за связанными объектами (авторы, соавторы, сцены, персонажи сцены).
  - При необходимости связных сущностей использовать явную жадную загрузку:
    ```csharp
    // ПРАВИЛЬНО: 1 запрос с JOIN
    var project = await context.MovieProjects
        .AsNoTracking()
        .Include(p => p.User)
        .Include(p => p.ProjectShares)
            .ThenInclude(ps => ps.SharedWithUser)
        .FirstOrDefaultAsync(p => p.Id == projectId);
    ```

- **Проекции через .Select() для сложных выборок**:
  - Если для отображения требуется только часть колонок, предпочитать проекции в DTO или анонимные типы через `.Select()`. Это извлекает из SQL Server только нужные поля (без передачи тяжелых текстов сценариев или HTML-диалогов):
    ```csharp
    var projectSummaries = await context.MovieProjects
        .AsNoTracking()
        .Where(p => p.UserId == userId)
        .Select(p => new ProjectSummaryDto
        {
            Id = p.Id,
            Title = p.Title,
            Genre = p.Genre,
            ScenesCount = p.Scenes.Count
        })
        .ToListAsync();
    ```

---

## 4. Целостность данных и атомарные транзакции

- **Пакетные операции и транзакции**:
  - Операции, затрагивающие несколько связанных таблиц (например, дублирование сцены с ее участниками или пересчет сквозной нумерации всех сцен сценария), должны выполняться в единой транзакции базы данных:
    ```csharp
    await using var context = await _contextFactory.CreateDbContextAsync();
    await using var transaction = await context.Database.BeginTransactionAsync();
    try
    {
        // ... изменение позиций сцен ...
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
    ```

- **Правила именования и миграций**:
  - Имена таблиц в схеме SQL Server задаются в `snake_case` через атрибут `[Table("table_name")]` (например, `movie_projects`, `project_shares`, `scenes`).
  - Имена колонок явно сопоставляются через `[Column("column_name")]`.
  - Все изменения схемы БД оформляются строго через стандартные миграции Entity Framework Core (`dotnet ef migrations add`), без ручных неконтролируемых правок в базе данных.
