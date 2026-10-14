using CineNote.Data;
using CineNote.Models;
using Microsoft.EntityFrameworkCore;

namespace CineNote.Services
{
    public class SceneService
    {
        private readonly IDbContextFactory<ApplicationDbContext>
            _contextFactory;

        public SceneService(
            IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Scene> CreateSceneAsync(Scene scene)
        {
            using var context =
                await _contextFactory.CreateDbContextAsync();

            int maxOrderNumber = await context.Scenes
                .Where(s => s.MovieProjectId == scene.MovieProjectId)
                .Select(s => (int?)s.SceneNumber)
                .MaxAsync() ?? 0;

            scene.SceneNumber = maxOrderNumber + 1;

            context.Scenes.Add(scene);

            await context.SaveChangesAsync();

            return scene;
        }

        public async Task<List<Scene>> GetScenesByProjectIdAsync(
            int projectId)
        {
            using var context =
                await _contextFactory.CreateDbContextAsync();

            return await context.Scenes
                .Where(s => s.MovieProjectId == projectId)
                .OrderBy(s => s.SceneNumber)
                .ToListAsync();
        }
    }
}