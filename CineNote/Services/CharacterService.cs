
using CineNote.Data;
using CineNote.Models;
using Microsoft.EntityFrameworkCore;

namespace CineNote.Services
{
    public class CharacterService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public CharacterService(
            IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Character> AddCharacterAsync(Character character)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Characters.Add(character);
            await context.SaveChangesAsync();

            return character;
        }

        public async Task<List<Character>> GetCharactersByProjectIdAsync(int projectId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Characters
                .Where(c => c.MovieProjectId == projectId)
                .ToListAsync();
        }

        public async Task<bool> UpdateCharacterAsync(Character character)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Characters.Update(character);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCharacterAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var character = await context.Characters.FindAsync(id);

            if (character == null)
                return false;

            context.Characters.Remove(character);
            return await context.SaveChangesAsync() > 0;
        }
    }
}
