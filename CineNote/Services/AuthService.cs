using CineNote.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace CineNote.Services
{
    public class AuthService
    {
        private readonly IJSRuntime _jsRuntime;

        public User? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;

        public event Action? OnChange;

        public AuthService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        // Загрузка пользователя из localStorage при старте
        public async Task InitializeAsync()
        {
            try
            {
                var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "currentUser");
                if (!string.IsNullOrEmpty(userJson))
                {
                    CurrentUser = JsonSerializer.Deserialize<User>(userJson);
                    NotifyStateChanged();
                }
            }
            catch
            {
                // Игнорируем ошибку при пререндере
            }
        }

        // Метод входа (теперь называется Login, как в Login.razor)
        public async Task Login(User user)
        {
            CurrentUser = user;
            var userJson = JsonSerializer.Serialize(user);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "currentUser", userJson);
            NotifyStateChanged();
        }

        // Метод выхода (теперь называется Logout)
        public async Task Logout()
        {
            CurrentUser = null;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "currentUser");
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}