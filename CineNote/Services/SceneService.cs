using CineNote.Data;
using CineNote.Models;
using Microsoft.EntityFrameworkCore;

namespace CineNote.Services
{
    public class SceneService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public SceneService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Scene> CreateSceneAsync(Scene scene)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            int maxOrderNumber = await context.Scenes
                .Where(s => s.MovieProjectId == scene.MovieProjectId)
                .Select(s => (int?)s.SceneNumber)
                .MaxAsync() ?? 0;

            scene.SceneNumber = maxOrderNumber + 1;
            context.Scenes.Add(scene);
            await context.SaveChangesAsync();
            return scene;
        }

        public async Task<List<Scene>> GetScenesByProjectIdAsync(int projectId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Scenes
                .Include(s => s.SceneCharacters).ThenInclude(sc => sc.Character)
                .Where(s => s.MovieProjectId == projectId)
                .OrderBy(s => s.SceneNumber)
                .ToListAsync();
        }

        public async Task UpdateSceneAsync(Scene scene)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Scenes.Update(scene);
            await context.SaveChangesAsync();
        }

        public async Task DeleteSceneAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            // Сначала находим саму сцену вместе со связанными персонажами
            var scene = await context.Scenes
                .Include(s => s.SceneCharacters)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (scene != null)
            {
                // Удаляем связи из промежуточной таблицы, чтобы не было ошибки FK
                context.SceneCharacters.RemoveRange(scene.SceneCharacters);

                // Теперь безопасно удаляем саму сцену
                context.Scenes.Remove(scene);

                await context.SaveChangesAsync();
            }
        }

        public async Task<Scene?> GetSceneByIdAsync(int sceneId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Scenes
                .Include(s => s.SceneCharacters).ThenInclude(sc => sc.Character)
                .FirstOrDefaultAsync(s => s.Id == sceneId);
        }

        public async Task UpdateSceneCharactersAsync(int sceneId, List<int> selectedCharacterIds)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var existingLinks = context.SceneCharacters.Where(sc => sc.SceneId == sceneId);
            context.SceneCharacters.RemoveRange(existingLinks);

            foreach (var charId in selectedCharacterIds)
            {
                context.SceneCharacters.Add(new SceneCharacter
                {
                    SceneId = sceneId,
                    CharacterId = charId
                });
            }

            await context.SaveChangesAsync();
        }

        // Объединенный метод для атомарного сохранения сцены и персонажей одним контекстом
        public async Task SaveSceneWithCharactersAsync(Scene scene, List<int> selectedCharacterIds)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            if (scene.Id == 0)
            {
                int maxOrderNumber = await context.Scenes
                    .Where(s => s.MovieProjectId == scene.MovieProjectId)
                    .Select(s => (int?)s.SceneNumber)
                    .MaxAsync() ?? 0;

                scene.SceneNumber = maxOrderNumber + 1;
                context.Scenes.Add(scene);
                await context.SaveChangesAsync(); // Получаем сгенерированный Id
            }
            else
            {
                context.Scenes.Update(scene);

                var existingLinks = context.SceneCharacters.Where(sc => sc.SceneId == scene.Id);
                context.SceneCharacters.RemoveRange(existingLinks);
            }

            foreach (var charId in selectedCharacterIds)
            {
                context.SceneCharacters.Add(new SceneCharacter
                {
                    SceneId = scene.Id,
                    CharacterId = charId
                });
            }

            await context.SaveChangesAsync();
        }
    }
}