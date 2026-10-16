using CineNote.Data;
using CineNote.Models;
using Microsoft.EntityFrameworkCore;

namespace CineNote.Services
{
    public class UserService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public UserService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // Проверка: есть ли пользователь с таким email в базе
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // Добавление нового пользователя в базу
        public async Task<User> AddUserAsync(User user)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }
    }
}