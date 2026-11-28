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
                .AsNoTracking()
                .Where(c => c.MovieProjectId == projectId)
                .ToListAsync();
        }

        public async Task<List<Character>> GetCharactersWithArcsAsync(int projectId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Characters
                .AsNoTracking()
                .Include(c => c.ArcSteps.OrderBy(a => a.Act).ThenBy(a => a.OrderIndex).ThenBy(a => a.Id))
                    .ThenInclude(a => a.Scene)
                .Include(c => c.SceneCharacters)
                    .ThenInclude(sc => sc.Scene)
                .Where(c => c.MovieProjectId == projectId)
                .OrderBy(c => c.Role)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<CharacterArcStep> AddArcStepAsync(CharacterArcStep step)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            int maxOrder = await context.CharacterArcSteps
                .Where(a => a.CharacterId == step.CharacterId)
                .Select(a => (int?)a.OrderIndex)
                .MaxAsync() ?? 0;

            step.OrderIndex = maxOrder + 1;
            context.CharacterArcSteps.Add(step);
            await context.SaveChangesAsync();

            return step;
        }

        public async Task<bool> DeleteArcStepAsync(int stepId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var step = await context.CharacterArcSteps.FindAsync(stepId);
            if (step == null)
                return false;

            context.CharacterArcSteps.Remove(step);
            return await context.SaveChangesAsync() > 0;
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

            var links = await context.SceneCharacters
                .Where(sc => sc.CharacterId == id)
                .ToListAsync();

            if (links.Any())
            {
                context.SceneCharacters.RemoveRange(links);
            }

            context.Characters.Remove(character);

            return await context.SaveChangesAsync() > 0;
        }
    }
}
