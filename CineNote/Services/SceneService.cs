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
            await RenumberScenesAsync(scene.MovieProjectId);
            return scene;
        }

        public async Task<List<Scene>> GetScenesByProjectIdAsync(int projectId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Scenes
                .Include(s => s.SceneCharacters).ThenInclude(sc => sc.Character)
                .Where(s => s.MovieProjectId == projectId)
                .OrderBy(s => s.Act)
                .ThenBy(s => s.SceneNumber)
                .ToListAsync();
        }

        public async Task UpdateSceneAsync(Scene scene)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Scenes.Update(scene);
            await context.SaveChangesAsync();
            await RenumberScenesAsync(scene.MovieProjectId);
        }

        public async Task DeleteSceneAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var scene = await context.Scenes
                .Include(s => s.SceneCharacters)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (scene != null)
            {
                int projectId = scene.MovieProjectId;

                if (scene.SceneCharacters.Any())
                {
                    context.SceneCharacters.RemoveRange(scene.SceneCharacters);
                }

                context.Scenes.Remove(scene);
                await context.SaveChangesAsync();

                await RenumberScenesAsync(projectId);
            }
        }

        public async Task<bool> UpdateSceneStatusAsync(int sceneId, SceneStatus newStatus)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var scene = await context.Scenes.FindAsync(sceneId);
            if (scene == null) return false;

            scene.Status = newStatus;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateSceneActAsync(int sceneId, MovieAct newAct)
        {
            return await MoveSceneToActAsync(sceneId, newAct);
        }

        public async Task<bool> MoveSceneToActAsync(int sceneId, MovieAct targetAct, int? insertBeforeSceneId = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var projectId = await context.Scenes
                .Where(x => x.Id == sceneId)
                .Select(x => x.MovieProjectId)
                .FirstOrDefaultAsync();

            if (projectId == 0) return false;

            var allScenes = await context.Scenes
                .Where(s => s.MovieProjectId == projectId)
                .OrderBy(s => s.Act)
                .ThenBy(s => s.SceneNumber)
                .ThenBy(s => s.Id)
                .ToListAsync();

            var movingScene = allScenes.FirstOrDefault(s => s.Id == sceneId);
            if (movingScene == null) return false;

            allScenes.Remove(movingScene);
            movingScene.Act = targetAct;

            if (insertBeforeSceneId.HasValue)
            {
                int targetIndex = allScenes.FindIndex(s => s.Id == insertBeforeSceneId.Value);
                if (targetIndex != -1)
                {
                    allScenes.Insert(targetIndex, movingScene);
                }
                else
                {
                    allScenes.Add(movingScene);
                }
            }
            else
            {
                int lastIndexInAct = allScenes.FindLastIndex(s => s.Act == targetAct);
                if (lastIndexInAct != -1)
                {
                    allScenes.Insert(lastIndexInAct + 1, movingScene);
                }
                else
                {
                    int insertIndex = allScenes.FindIndex(s => s.Act > targetAct);
                    if (insertIndex != -1)
                        allScenes.Insert(insertIndex, movingScene);
                    else
                        allScenes.Add(movingScene);
                }
            }

            var sortedFinal = allScenes
                .OrderBy(s => s.Act)
                .ToList();

            for (int i = 0; i < sortedFinal.Count; i++)
            {
                sortedFinal[i].SceneNumber = i + 1;
            }

            await context.SaveChangesAsync();
            return true;
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
                await context.SaveChangesAsync();
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
            await RenumberScenesAsync(scene.MovieProjectId);
        }

        public async Task UpdateScenesOrderAsync(List<int> orderedSceneIds, int projectId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var scenes = await context.Scenes
                .Where(s => s.MovieProjectId == projectId)
                .ToListAsync();

            for (int i = 0; i < orderedSceneIds.Count; i++)
            {
                var sc = scenes.FirstOrDefault(s => s.Id == orderedSceneIds[i]);
                if (sc != null)
                {
                    sc.SceneNumber = i + 1;
                }
            }

            await context.SaveChangesAsync();
            await RenumberScenesAsync(projectId);
        }

        public async Task<bool> MoveSceneUpAsync(int sceneId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var currentScene = await context.Scenes.FindAsync(sceneId);
            if (currentScene == null) return false;

            var prevScene = await context.Scenes
                .Where(s => s.MovieProjectId == currentScene.MovieProjectId && s.Act == currentScene.Act && s.SceneNumber < currentScene.SceneNumber)
                .OrderByDescending(s => s.SceneNumber)
                .FirstOrDefaultAsync();

            if (prevScene == null) return false;

            int temp = currentScene.SceneNumber;
            currentScene.SceneNumber = prevScene.SceneNumber;
            prevScene.SceneNumber = temp;

            await context.SaveChangesAsync();
            await RenumberScenesAsync(currentScene.MovieProjectId);
            return true;
        }

        public async Task<bool> MoveSceneDownAsync(int sceneId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var currentScene = await context.Scenes.FindAsync(sceneId);
            if (currentScene == null) return false;

            var nextScene = await context.Scenes
                .Where(s => s.MovieProjectId == currentScene.MovieProjectId && s.Act == currentScene.Act && s.SceneNumber > currentScene.SceneNumber)
                .OrderBy(s => s.SceneNumber)
                .FirstOrDefaultAsync();

            if (nextScene == null) return false;

            int temp = currentScene.SceneNumber;
            currentScene.SceneNumber = nextScene.SceneNumber;
            nextScene.SceneNumber = temp;

            await context.SaveChangesAsync();
            await RenumberScenesAsync(currentScene.MovieProjectId);
            return true;
        }

        public async Task<Scene> InsertSceneAfterAsync(int projectId, int afterSceneNumber, Scene newScene)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var subsequentScenes = await context.Scenes
                .Where(s => s.MovieProjectId == projectId && s.SceneNumber > afterSceneNumber)
                .OrderByDescending(s => s.SceneNumber)
                .ToListAsync();

            foreach (var s in subsequentScenes)
            {
                s.SceneNumber += 1;
            }

            newScene.MovieProjectId = projectId;
            newScene.SceneNumber = afterSceneNumber + 1;
            context.Scenes.Add(newScene);

            await context.SaveChangesAsync();
            await RenumberScenesAsync(projectId);
            return newScene;
        }

        public async Task<Scene?> DuplicateSceneAsync(int sceneId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var original = await context.Scenes
                .Include(s => s.SceneCharacters)
                .FirstOrDefaultAsync(s => s.Id == sceneId);

            if (original == null) return null;

            int projectId = original.MovieProjectId;
            int insertAfter = original.SceneNumber;

            var subsequentScenes = await context.Scenes
                .Where(s => s.MovieProjectId == projectId && s.SceneNumber > insertAfter)
                .OrderByDescending(s => s.SceneNumber)
                .ToListAsync();

            foreach (var s in subsequentScenes)
            {
                s.SceneNumber += 1;
            }

            var duplicate = new Scene
            {
                MovieProjectId = projectId,
                SceneNumber = insertAfter + 1,
                Title = string.IsNullOrWhiteSpace(original.Title) ? string.Empty : $"{original.Title} (Копия)",
                LocationType = original.LocationType,
                LocationName = original.LocationName,
                TimeOfDay = original.TimeOfDay,
                ActionDescription = original.ActionDescription,
                Act = original.Act,
                Status = original.Status,
                DramaticStructure = original.DramaticStructure
            };

            context.Scenes.Add(duplicate);
            await context.SaveChangesAsync();

            if (original.SceneCharacters.Any())
            {
                foreach (var sc in original.SceneCharacters)
                {
                    context.SceneCharacters.Add(new SceneCharacter
                    {
                        SceneId = duplicate.Id,
                        CharacterId = sc.CharacterId
                    });
                }
                await context.SaveChangesAsync();
            }

            await RenumberScenesAsync(projectId);
            return duplicate;
        }

        public async Task RenumberScenesAsync(int projectId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var scenes = await context.Scenes
                .Where(s => s.MovieProjectId == projectId)
                .OrderBy(s => s.Act)
                .ThenBy(s => s.SceneNumber)
                .ThenBy(s => s.Id)
                .ToListAsync();

            bool needsSave = false;
            for (int i = 0; i < scenes.Count; i++)
            {
                int expectedNumber = i + 1;
                if (scenes[i].SceneNumber != expectedNumber)
                {
                    scenes[i].SceneNumber = expectedNumber;
                    needsSave = true;
                }
            }

            if (needsSave)
            {
                await context.SaveChangesAsync();
            }
        }
    }
}