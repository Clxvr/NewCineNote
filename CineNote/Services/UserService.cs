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


        public async Task<User> AddUserAsync(User user)
        {
            using var context = await _contextFactory.CreateDbContextAsync(); // Исправлено на CreateDbContextAsync

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        // Поиск по email (чтобы не было дубликатов)
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}