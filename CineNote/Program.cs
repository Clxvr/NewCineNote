using CineNote.Components;
using CineNote.Data;
using CineNote.Models;
using CineNote.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавление Razor компонентов и интерактивного сервера
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Регистрация фабрики контекста базы данных (нужна для сервисов)
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Стандартная регистрация DbContext (на всякий случай)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация всех наших сервисов
builder.Services.AddScoped<MovieProjectService>();
builder.Services.AddScoped<CharacterService>();
builder.Services.AddScoped<SceneService>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// --- ПРИМИТИВНАЯ ПРОВЕРКА И ДОБАВЛЕНИЕ ДАННЫХ В БД ---
using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    using var context = contextFactory.CreateDbContext();

    // 1. Если пользователей нет — создаем тестового пользователя
    if (!context.Users.Any())
    {
        var testUser = new User
        {
            Email = "test@cinenote.com",
            Password = "123456",
            Name = "Тестовый Режиссер",
            GlobalRole = "Администратор"
        };
        context.Users.Add(testUser);
        context.SaveChanges();
    }

    // 2. Берем первого пользователя из базы
    var user = context.Users.First();

  
}
// --------------------------------------------------

app.Run();