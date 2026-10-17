
using CineNote.Models;

namespace CineNote.Services
{
    public class AuthService
    {

        public User? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;
   
        public event Action? OnAuthStateChanged;

        public void Login(User user)
        {
            CurrentUser = user;
            OnAuthStateChanged?.Invoke();
        }

        public void Logout()
        {
            CurrentUser = null;
            OnAuthStateChanged?.Invoke();
        }
    }
}