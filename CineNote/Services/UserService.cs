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


        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

     
        public async Task<User> AddUserAsync(User user)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }
        public async Task<bool> UpdateUserAsync(User user)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Users.Update(user);
            return await context.SaveChangesAsync() > 0;
        }
    }
}
