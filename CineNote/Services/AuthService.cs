using CineNote.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace CineNote.Services
{
    public class AuthService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly UserService _userService;

        public User? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;

        public event Action? OnChange;

        public AuthService(IJSRuntime jsRuntime, UserService userService)
        {
            _jsRuntime = jsRuntime;
            _userService = userService;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var sessionJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "currentUser");
                if (!string.IsNullOrEmpty(sessionJson))
                {
                    var session = JsonSerializer.Deserialize<UserSession>(sessionJson);
                    if (session != null)
                    {
                        CurrentUser = new User
                        {
                            Id = session.Id,
                            Name = session.Name,
                            Email = session.Email,
                            GlobalRole = session.GlobalRole,
                            IsEmailConfirmed = session.IsEmailConfirmed
                        };
                        NotifyStateChanged();
                    }
                }
            }
            catch
            {
                CurrentUser = null;
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
                return false;

            if (!user.IsEmailConfirmed)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                return false;

            await Login(user);
            return true;
        }

        public async Task Login(User user)
        {
            CurrentUser = user;
            var session = new UserSession(user.Id, user.Name, user.Email, user.GlobalRole, user.IsEmailConfirmed);
            var sessionJson = JsonSerializer.Serialize(session);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "currentUser", sessionJson);
            NotifyStateChanged();
        }

        public async Task Logout()
        {
            CurrentUser = null;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "currentUser");
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        private record UserSession(int Id, string Name, string Email, string GlobalRole, bool IsEmailConfirmed);
    }
}
