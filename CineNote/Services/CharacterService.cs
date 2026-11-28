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

        public CharacterWorkloadSummary BuildWorkloadSummary(
            List<Character> characters,
            List<Scene> allScenes,
            MovieAct? actFilter = null,
            CharacterRole? roleFilter = null)
        {
            var filteredScenes = actFilter.HasValue
                ? allScenes.Where(s => s.Act == actFilter.Value).ToList()
                : allScenes;

            var filteredCharacters = roleFilter.HasValue
                ? characters.Where(c => c.Role == roleFilter.Value).ToList()
                : characters;

            int totalScenes = filteredScenes.Count;

            var stats = filteredCharacters.Select(c =>
            {
                var charScenes = filteredScenes
                    .Where(s => s.SceneCharacters.Any(sc => sc.CharacterId == c.Id))
                    .ToList();
                int count = charScenes.Count;
                int percent = totalScenes > 0 ? (int)Math.Round((double)count / totalScenes * 100) : 0;

                return new CharacterStatItem
                {
                    Character = c,
                    SceneCount = count,
                    Percent = percent,
                    Act1Count = charScenes.Count(s => s.Act == MovieAct.Act1),
                    Act2Count = charScenes.Count(s => s.Act == MovieAct.Act2),
                    Act3Count = charScenes.Count(s => s.Act == MovieAct.Act3)
                };
            })
            .OrderByDescending(x => x.SceneCount)
            .ThenBy(x => x.Character.Role)
            .ToList();

            string topSummary = "Нет данных";
            if (characters.Any() && allScenes.Any())
            {
                var top = characters
                    .Select(c => new
                    {
                        c.Name,
                        Count = allScenes.Count(s => s.SceneCharacters.Any(sc => sc.CharacterId == c.Id))
                    })
                    .OrderByDescending(x => x.Count)
                    .FirstOrDefault();

                if (top == null || top.Count == 0)
                {
                    topSummary = "Не привязаны";
                }
                else
                {
                    int pct = (int)Math.Round((double)top.Count / allScenes.Count * 100);
                    topSummary = $"{top.Name} ({pct}%)";
                }
            }

            return new CharacterWorkloadSummary
            {
                FilteredScenes = filteredScenes,
                Stats = stats,
                TopCharacterSummary = topSummary,
                TotalArcStepsCount = characters.Sum(c => c.ArcSteps.Count)
            };
        }
    }

    public class CharacterWorkloadSummary
    {
        public List<Scene> FilteredScenes { get; set; } = new();
        public List<CharacterStatItem> Stats { get; set; } = new();
        public string TopCharacterSummary { get; set; } = "Нет данных";
        public int TotalArcStepsCount { get; set; }
    }

    public class CharacterStatItem
    {
        public Character Character { get; set; } = default!;
        public int SceneCount { get; set; }
        public int Percent { get; set; }
        public int Act1Count { get; set; }
        public int Act2Count { get; set; }
        public int Act3Count { get; set; }
    }
}
